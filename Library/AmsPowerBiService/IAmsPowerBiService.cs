using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmsPowerBiService.Models;
using Microsoft.PowerBI.Api.Beta.Models;

namespace AmsPowerBiService
{
    public interface IAmsPowerBiService
    {
        /// <summary>
        /// Get all report with token from Power BI
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="includeTokens"></param>
        /// <returns></returns>
        Task<List<ReportWithToken>> GetAllReports(string workspaceId,bool includeTokens = false);

        /// <summary>
        /// Get report by id from Power BI
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workspaceId"></param>
        /// <returns></returns>
        Task<ReportWithToken> GetReportById(string id, string workspaceId);

        /// <summary>
        /// Search report with name from Power BI
        /// </summary>
        /// <param name="name"></param>
        /// <param name="workspaceId"></param>
        /// <param name="includeTokens"></param>
        /// <returns></returns>
        Task<List<ReportWithToken>> SearchReportByName(string name, string workspaceId,bool includeTokens =false);

        /// <summary>
        /// Creates a new Power BI Embedded workspace within the specified collection
        /// </summary>
        /// <returns></returns>
        Task<Workspace> CreateWorkspaceAsync();

        /// <summary>
        /// Imports a Power BI Desktop file (pbix) into the Power BI Embedded service
        /// </summary>
        /// <param name="workspaceId">The target Power BI workspace id</param>
        /// <param name="datasetName">The dataset name to apply to the uploaded dataset</param>
        /// <param name="stream">FileStream</param>
        /// <returns></returns>
        Task<Import> ImportPbixAsync(string workspaceId, string datasetName, FileStream stream);

        /// <summary>
        /// Updates the Power BI dataset connection info for datasets with direct query connections
        /// </summary>
        /// <param name="workspaceId">The Power BI workspace id that contains the dataset</param>
        /// <returns></returns>
        Task UpdateConnectionAsync(string workspaceId);

    }
}
