using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AmsPowerBiService.Models;
using Microsoft.PowerBI.Api.Beta;
using Microsoft.PowerBI.Api.Beta.Models;
using Microsoft.PowerBI.Security;
using Microsoft.Rest;

namespace AmsPowerBiService
{
    public class AmsPowerBiService:IAmsPowerBiService
    {
        private readonly string _workspaceCollection;
        //private readonly string _workspaceId;
        private readonly string _signingKey;
        private readonly string _apiUrl;
        private readonly string _connectionString;
        private readonly string _username;
        private readonly string _password;

        public AmsPowerBiService()
        {
            _workspaceCollection = ConfigurationManager.AppSettings["powerbi:WorkspaceCollection"];
           // _workspaceId = ConfigurationManager.AppSettings["powerbi:WorkspaceId"];
            _signingKey = ConfigurationManager.AppSettings["powerbi:AccessKey"];
            _apiUrl = ConfigurationManager.AppSettings["powerbi:ApiUrl"];
            _connectionString = ConfigurationManager.AppSettings["powerbi:ConnectionString"];
            _username = ConfigurationManager.AppSettings["powerbi:Username"];
            _password = ConfigurationManager.AppSettings["powerbi:Password"];
        }

        public async Task<List<ReportWithToken>> GetAllReports(string workspaceId,bool includeTokens = false)
        {
            var devToken = PowerBIToken.CreateDevToken(_workspaceCollection, workspaceId);
            using (var client = CreatePowerBiClient(devToken))
            {
                var reportsResponse = await client.Reports.GetReportsAsync(_workspaceCollection, workspaceId);
                var reportsWithTokens = reportsResponse.Value
                    .Select(report =>
                    {
                        string accessToken = null;
                        if (includeTokens)
                        {
                            var embedToken = PowerBIToken.CreateReportEmbedToken(_workspaceCollection, workspaceId, report.Id);
                            accessToken = embedToken.Generate(_signingKey);
                        }

                        return new ReportWithToken(report, accessToken);
                    })
                    .ToList();

                return reportsWithTokens;
            }
        }

        public async Task<ReportWithToken> GetReportById(string id, string workspaceId)
        {
            var devToken = PowerBIToken.CreateDevToken(_workspaceCollection, workspaceId);
            using (var client = CreatePowerBiClient(devToken))
            {
                var reportsResponse = await client.Reports.GetReportsAsync(_workspaceCollection, workspaceId);
                var report = reportsResponse.Value.FirstOrDefault(r => r.Id == id);
                if (report == null)
                {
                    return null;
                }

                var embedToken = PowerBIToken.CreateReportEmbedToken(_workspaceCollection, workspaceId, report.Id);
                var accessToken = embedToken.Generate(_signingKey);
                var reportWithToken = new ReportWithToken(report, accessToken);

                return reportWithToken;
            }
        }

        public async Task<List<ReportWithToken>> SearchReportByName(string name, string workspaceId, bool includeTokens = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<ReportWithToken>();
            }

            var devToken = PowerBIToken.CreateDevToken(_workspaceCollection, workspaceId);
            using (var client = CreatePowerBiClient(devToken))
            {
                var reportsResponse = await client.Reports.GetReportsAsync(_workspaceCollection, workspaceId);
                var reports = reportsResponse.Value.Where(r => r.Name.ToLower().StartsWith(name.ToLower()));

                var reportsWithTokens = reports
                    .Select(report =>
                    {
                        string accessToken = null;
                        if (includeTokens)
                        {
                            var embedToken = PowerBIToken.CreateReportEmbedToken(_workspaceCollection, workspaceId, report.Id);
                            accessToken = embedToken.Generate(_signingKey);
                        }

                        return new ReportWithToken(report, accessToken);
                    })
                    .ToList();

                return reportsWithTokens;
            }
        }


        public async Task<Workspace> CreateWorkspaceAsync()
        {
            var devToken = PowerBIToken.CreateProvisionToken(_workspaceCollection);
            using (var client =  CreatePowerBiClient(devToken))
            {
                // Create a new workspace witin the specified collection
                return await client.Workspaces.PostWorkspaceAsync(_workspaceCollection);
            }
        }

        public async Task<Import> ImportPbixAsync(string workspaceId, string datasetName, FileStream stream)
        {
            var devToken = PowerBIToken.CreateDevToken(_workspaceCollection, workspaceId);
            using (var client =  CreatePowerBiClient(devToken))
                {
                
                // Import PBIX file from the file stream
                var import = await client.Imports.PostImportWithFileAsync(_workspaceCollection, workspaceId, stream, datasetName);

                    // Example of polling the import to check when the import has succeeded.
                    while (import.ImportState != "Succeeded" && import.ImportState != "Failed")
                    {
                        import = await client.Imports.GetImportByIdAsync(_workspaceCollection, workspaceId, import.Id);
                        Thread.Sleep(1000);
                    }
                    return import;
                }
        }

        public async Task UpdateConnectionAsync(string workspaceId)
        {
            var devToken = PowerBIToken.CreateDevToken(_workspaceCollection, workspaceId);
            using (var client = CreatePowerBiClient(devToken))
            {
                // Get the newly created dataset from the previous import process
                var datasets = await client.Datasets.GetDatasetsAsync(_workspaceCollection, workspaceId);
                var connectionString = _connectionString;
                // Optionally udpate the connectionstring details if preent
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    var connectionParameters = new Dictionary<string, object>
                    {
                        { "connectionString", connectionString }
                    };
                    await client.Datasets.SetAllConnectionsAsync(_workspaceCollection, workspaceId, datasets.Value[datasets.Value.Count - 1].Id, connectionParameters);
                }

                // Get the datasources from the dataset
                var datasources = await client.Datasets.GetGatewayDatasourcesAsync(_workspaceCollection, workspaceId, datasets.Value[datasets.Value.Count - 1].Id);

                // Reset your connection credentials
                var delta = new GatewayDatasource
                {
                    CredentialType = "Basic",
                    BasicCredentials = new BasicCredentials
                    {
                        Username = _username,
                        Password = _password
                    }
                };

                // Update the datasource with the specified credentials
                await client.Gateways.PatchDatasourceAsync(_workspaceCollection, workspaceId, datasources.Value[datasources.Value.Count - 1].GatewayId, datasources.Value[datasources.Value.Count - 1].Id, delta);
            }
        }


        /// <summary>
        /// Create power bi client
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private IPowerBIClient CreatePowerBiClient(PowerBIToken token)
        {
            var jwt = token.Generate(_signingKey);
            var credentials = new TokenCredentials(jwt, "AppToken");
            var client = new PowerBIClient(credentials)
            {
                BaseUri = new Uri(_apiUrl)
            };
            // Set request timeout to support uploading large PBIX files
            client.HttpClient.Timeout = TimeSpan.FromMinutes(60);
            return client;
        }



    }
}
