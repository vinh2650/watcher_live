using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Domain.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Utilities;
using RestSharp;
using Service.Interface.Search;

namespace Service.Implement.Search
{
    public class NoisSearchEngine : INoisSearchEngine
    {
        private RestClient _client = new RestClient(ConfigurationManager.AppSettings["ElasticSearchUrl"]);
        private readonly string _indexName = ConfigurationManager.AppSettings["EsIndexName"];

        public async Task CreateIndexAsync(string indexName, string customAnalyzer = null )
        {
            if (string.IsNullOrEmpty(indexName))
            {
                return;
            }
            indexName = indexName.ToLower();
            //create request
            var request = new RestRequest(indexName + "?pretty", Method.PUT);
            //Add custom Analyer
            if (customAnalyzer != null)
                request.AddParameter("", customAnalyzer, ParameterType.RequestBody);

            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CreateIndex(string indexName, string customAnalyzer = null)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                return;
            }
            indexName = indexName.ToLower();
            //create request
            var request = new RestRequest(indexName + "?pretty", Method.PUT);
            //Add custom Analyer
            if (customAnalyzer != null)
                request.AddParameter("", customAnalyzer, ParameterType.RequestBody);
            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task DeleteIndexAsync(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                return;
            }
            indexName = indexName.ToLower();
            //create request
            var request = new RestRequest(indexName + "?pretty", Method.DELETE);

            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteIndex(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                return;
            }
            indexName = indexName.ToLower();
            //create request
            var request = new RestRequest(indexName + "?pretty", Method.DELETE);

            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void MappingIndex(string indexName, string typeName, string mappingData)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                return;
            }
            indexName = indexName.ToLower();
            //create request
            var request = new RestRequest(indexName + "/_mapping/" + typeName, Method.PUT);
            //add to request
            request.AddParameter("", mappingData, ParameterType.RequestBody);
            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task MappingIndexAsync(string indexName, string typeName, string mappingData)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                return;
            }
            indexName = indexName.ToLower();
            //create request
            var request = new RestRequest(indexName + "/_mapping/" + typeName, Method.PUT);
            //add to request
            request.AddParameter("", mappingData, ParameterType.RequestBody);
            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void BulkIndex(string indexName, string entitytype, string dataIndex)
        {
            //create request
            var request = new RestRequest("/" + indexName + "/" + entitytype + "/_bulk?pretty&refresh=true", Method.POST);

            try
            {
                //prepare body string
                var body = dataIndex;

                //add to request
                request.AddParameter("", body, ParameterType.RequestBody);
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void BulkIndex(string indexName, string entitytype, string dataIndex, string operatorId)
        {
            //create request
            var request = new RestRequest("/" + indexName + "/" + entitytype + 
                "/_bulk?pretty&refresh=true" +
                "&routing=" + operatorId, Method.POST);

            try
            {
                //prepare body string
                var body = dataIndex;

                //add to request
                request.AddParameter("", body, ParameterType.RequestBody);
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void IndexData(string indexName, string entitytype, string dataId, string dataIndex)
        {
            //create request
            var request = new RestRequest(indexName + "/" + entitytype + "/" + dataId, Method.PUT);

            //prepare body for message

            //add to request
            request.AddParameter("", dataIndex, ParameterType.RequestBody);
            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateBulk(string indexName, string entityType, string dataUpdate)
        {
            //update request
            var request = new RestRequest("/" + indexName + "/" + entityType + "/_bulk?pretty&refresh=true", Method.POST);

            try
            {
                //prepare body string
                var body = dataUpdate;

                //add to request
                request.AddParameter("", body, ParameterType.RequestBody);
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteByTemp(string indexName, string entityType, string tempId)
        {
            //update request
            var request = new RestRequest("/" + indexName + "/" + entityType + "/_query?q=tempid:" + tempId, Method.DELETE);

            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteByTempV3(string jobId, string operatorId, string indexName, string entityType, string tempId)
        {
            //update request
            var request = new RestRequest("/" + indexName + "/" + entityType + 
                "/_query?routing=" + operatorId, Method.DELETE);

            var body =
                "{\"query\":"+ 
                    "{\"bool\":"+ 
                        "{\"must\":[" + 
                            "{\"term\":{\"tempid\":\"" + tempId + "\"}},"+
                            "{\"term\":{\"jobid\":\"" + jobId + "\"}}"+ 
                        "]}" + 
                    "}" + 
                "}";

            try
            {
                //execute
                var response = _client.Execute(request);
                var res = response.Content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CreateRepository(string path, string repoName)
        {
            //Check repository if exist ?
            var chkRequest = new RestRequest("/_snapshot/"+ repoName, Method.GET);
            var chkResponse = _client.Execute(chkRequest);
            var res = chkResponse.StatusCode;
            if (res == HttpStatusCode.NotFound)
            {
                //Map repository
                var crRquest = new RestRequest("/_snapshot/" + repoName, Method.PUT);

                var body = "{\"type\": \"fs\",\"settings\": {\"location\": \"" + path + "\",\"compress\": true}}";

                crRquest.AddParameter("", body, ParameterType.RequestBody);
                try
                {
                    var crReponse = _client.Execute(crRquest);
                    if (crReponse.StatusCode != HttpStatusCode.OK)
                        throw new Exception("Error at create new repository for backup file");
                }
                catch (Exception)
                {
                        
                    throw;
                }
            }
        }

        public void TakeSnapshot(string repoName, string snapshotName)
        {
            //wait for finish snapshot
            var request = new RestRequest("/_snapshot/" + repoName + "/" + snapshotName + "?wait_for_completion=true", Method.PUT);

            var body = "{\"indices\":\"" + _indexName + "\"," +
                       "\"ignore_unavailable\":true }";

            request.AddParameter("", body, ParameterType.RequestBody);
            try
            {
                var response = _client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Error at taking Snapshot of old Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RestoreFromSnapshot(string repoName, string snapshotName, string restoreName)
        {
            var request = new RestRequest("/_snapshot/" + repoName + "/" + snapshotName + "/_restore?wait_for_completion=true", Method.POST);
            
            var body = "{\"indices\":\"" + _indexName + "\"," +
                       "\"ignore_unavailable\":true," +
                       "\"rename_pattern\":\"" + _indexName + "\"," +
                       "\"rename_replacement\":\"" + restoreName + "\"}";

            request.AddParameter("", body, ParameterType.RequestBody);

            try
            {
                var response = _client.Execute(request);
                if ( response.StatusCode != HttpStatusCode.OK )
                    throw new Exception("Error at Restore From Snapshot to screate Temp Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ReindexData(string source, string dest)
        {
            var request = new RestRequest("/_reindex/", Method.POST);

            string body = "{\"source\":{\"index\":\"" + source + "\"}," +
                          "\"dest\":{\"index\":\"" + dest + "\"}}";

            request.AddParameter("", body, ParameterType.RequestBody);

            try
            {
                var res = _client.Execute(request);
                if (res.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Error at reindexing data from Temp Index to new Index");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class EsSearchSortLocation<T> where T : class
    {
        public string _scroll_id { get; set; }

        public EsHit<T> hits { get; set; }

        public EsSearchSortLocation()
        {
            hits = new EsHit<T>();
        }
    }

    public class EsSearchLocation<T> where T : class
    {
        public EsSearchLocation()
        {
            hits = new EsHit<T>();
        }

        public EsHit<T> hits { get; set; }
    }
    public class EsHit<T> where T : class
    {
        public int total { get; set; }

        public List<eshit<T>> hits { get; set; }

        public EsHit()
        {
            hits = new List<eshit<T>>();
        }
    }

    public class eshit<T> where T : class
    {
        public string _id { get; set; }

        // public List<double> sort { get; set; }

        public T _source { get; set; }

        public eshit()
        {
            // sort = new List<double>();
        }
    }

    public class essource
    {
        public eslocation location { get; set; }


    }

    public class eslocation
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }
}
