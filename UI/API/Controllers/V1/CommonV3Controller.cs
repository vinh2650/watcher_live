using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using API.Models;
using Common.Helpers;
using Service.Interface.Authentication;
using Service.Interface.Search;

namespace API.Controllers.V1
{
    /// <summary>
    /// common api
    /// </summary>
    [RoutePrefix("api/v1/common")]
    public class CommonV3Controller : BaseApiController
    {
        #region ES
        private readonly string _indexName = ConfigurationManager.AppSettings["EsIndexName"];
        private readonly string _indexBtsType = "bts";
        private readonly string _indexCellType = "cell";
        #endregion

        #region fields
        private readonly IAmsApplicationService _applicationService;
        private readonly IUserService _userService;
        private readonly ISearchEngine _noisSearchEngine;
        #endregion

        #region Const

        private readonly string _mediaBaseStr = ConfigurationManager.AppSettings["MediaBaseUrl"];
        private readonly string _attackFolderName = ConfigurationManager.AppSettings["AttachFolderName"];
        
        #endregion

        #region ctor

        /// <summary>
        /// constructor common
        /// </summary>
        /// <param name="applicationService"></param>
        /// <param name="workContext"></param>
        /// <param name="userService"></param>
        /// <param name="configurationProcessService"></param>
        /// <param name="alarmRecordService"></param>
        /// <param name="noisSearchEngine"></param>
        /// <param name="mediaService"></param>
        public CommonV3Controller(IAmsApplicationService applicationService,
            IUserService userService,
            ISearchEngine noisSearchEngine)
        {
            _applicationService = applicationService;
            _userService = userService;
            _noisSearchEngine = noisSearchEngine;
        }
        #endregion

        ///// <summary>
        ///// get basic code
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[AllowAnonymous]
        //[Route("{id}")]
        //public IHttpActionResult Get(string id)
        //{
        //    if (id == "Nois")
        //    {
        //        var listApplications = _applicationService.GetAllApplications();
        //        var listString = listApplications.Select(p =>
        //        {
        //            var resId = p.Id;
        //            var resEncryptSecret = p.EncryptSecret;
        //            var resDecryptSecret = CommonSecurityHelper.Decrypt(resEncryptSecret, CommonSecurityHelper.KeyEncrypt);
        //            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(resId + ":" + resDecryptSecret);
        //            var res = Convert.ToBase64String(plainTextBytes);
        //            return new
        //            {
        //                ClientId = p.Id,
        //                ClientSecret = resDecryptSecret,
        //                BasicCode = res
        //            };

        //        }).ToList();
        //        return Json(new
        //        {
        //            Apps = listString
        //        });
        //    }
        //    else
        //    {
        //        return Json(new
        //        {

        //        });
        //    }

        //}

        /// <summary>
        /// Delete existing index and create anew with mappings
        /// </summary>
        /// <returns></returns>
        [Route("indexdata")]
        [HttpGet]
        public async Task<IHttpActionResult> IndexData()
        {
            try
            {
                //Create repository for holding backup ( snapshot) files
                var repoName = "my_backup";
                var path = ConfigurationManager.AppSettings["EsRepoPath"];
                _noisSearchEngine.CreateRepository(path, repoName);

                //Take snapshot of old index
                var snapshotName = "snapshot_" + Guid.NewGuid();
                _noisSearchEngine.TakeSnapshot(repoName, snapshotName);

                //delete old index
                await _noisSearchEngine.DeleteIndexAsync(_indexName);
                //custom analyser
                var analyzerName = "lowercaseAnalyzer";
                var analyzerstr =
                "{\"settings\" : {" +
                    "\"number_of_shards\": 1000," +
                    "\"number_of_replicas\": 1," +
                    "\"analysis\" : " +
                        "{\"analyzer\":" + "{\"" + analyzerName + "\" : " +
                                "{\"type\" : \"custom\"," +
                                "\"tokenizer\" : \"keyword\"," +
                                "\"filter\" : \"lowercase\"" +
                "}}}}}";

                //Create new index
                await _noisSearchEngine.CreateIndexAsync(_indexName, analyzerstr);

                // mapping bts
                var mappingDataBts =
                    "{\"" + _indexBtsType + "\":" +
                    "{\"_routing\":" +
                    //All insert or index need routing mapping
                    "{\"required\":true }," +
                    "\"properties\":" +
                    "{\"btsid\":{\"type\":\"string\",\"index\":\"not_analyzed\"}," +
                    "\"location\":{\"type\":\"geo_point\"}," +
                    "\"tempid\":{\"type\":\"string\",\"index\":\"not_analyzed\"}," +
                    "\"btsname\":{\"type\":\"string\",\"analyzer\":\"" + analyzerName + "\"}," +
                    "\"carriername\":{\"type\":\"string\",\"analyzer\":\"" + analyzerName + "\"}," +
                    "\"networkname\":{\"type\":\"string\",\"analyzer\":\"" + analyzerName + "\"}," +
                    "\"operatorid\":{\"type\":\"string\",\"index\":\"not_analyzed\"}" +
                    "}" +
                    "}" +
                    "}";
                await _noisSearchEngine.MappingIndexAsync(_indexName, _indexBtsType, mappingDataBts);

                //mapping sites
                var mappingDataCell =
                    "{\"" + _indexCellType + "\":" +
                    "{\"_routing\":" +
                    "{\"required\":true }," +
                    "\"properties\":" +
                    "{\"btsname\":{\"type\":\"string\",\"analyzer\":\"" + analyzerName + "\"}," +
                    "\"bscname\":{\"type\":\"string\",\"analyzer\":\"" + analyzerName + "\"}," +
                    "\"antennatype\":{\"type\":\"string\",\"analyzer\":\"" + analyzerName + "\"}," +
                    "\"cellid\":{\"type\":\"string\",\"index\":\"not_analyzed\"}," +
                    "\"location\":{\"type\":\"geo_point\"}," +
                    "\"cellname\":{\"type\":\"string\",\"analyzer\":\"" + analyzerName + "\"}," +
                    "\"technology\":{\"type\":\"string\",\"analyzer\":\"" + analyzerName + "\"}," +
                    "\"frequencytechnologyband\":{\"type\":\"string\",\"index\":\"not_analyzed\"}," +
                    "\"tempid\":{\"type\":\"string\",\"index\":\"not_analyzed\"}," +
                    "\"operatorid\":{\"type\":\"string\",\"index\":\"not_analyzed\"}" +
                    "}" +
                    "}" +
                    "}";
                await _noisSearchEngine.MappingIndexAsync(_indexName, _indexCellType, mappingDataCell);

                //Restore from snapshot, create temp index to get old data
                var restoreName = "restore_" + _indexName;
                _noisSearchEngine.RestoreFromSnapshot(repoName, snapshotName, restoreName);

                //Reindex data from temp index to new one
                _noisSearchEngine.ReindexData(restoreName, _indexName);

                //Delete temp index
                _noisSearchEngine.DeleteIndex(restoreName);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return Ok();
        }
    }
}