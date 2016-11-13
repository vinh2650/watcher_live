using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Core.Domain.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Interface.Search;
using Service.Interface.SearchModel;

namespace Service.Implement.Search
{
    public class UserSearchService : IUserSearchService
    {
        private readonly RestClient _client = new RestClient(ConfigurationManager.AppSettings["ElasticSearchUrl"]);
        private readonly string _indexName = ConfigurationManager.AppSettings["EsIndexName"];
        private readonly string _indexType = "user";

        /// <summary>
        /// Ctor
        /// </summary>
        public UserSearchService()
        {

        }

        public void IndexUser(User user)
        {
            try
            {
                var esUser = new JObject()
                {
                    ["id"] = user.Id,
                    ["username"] = user.UserName,
                    ["lastname"] = user.LastName,
                    ["firstname"] = user.FirstName,
                    ["email"] = user.Email,
                    ["phone"] = user.Phone,
                    ["birthday"] = user.BirthDate
                };

                var request = new RestRequest(_indexName + "/" + _indexType + "/" + user.Id, Method.POST);
                request.AddParameter("", esUser, ParameterType.RequestBody);

                _client.Execute(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public esResultModel<esUserData> SearchUserByKeyword(List<string> keywords)
        {
            try
            {
                var listKeyworkStr = keywords.Select(key => "{\"wildcard\":{\"firstname\":\"*" + key + "*\"}}," +
                                                            "{\"wildcard\":{\"lastname\":\"*" + key + "*\"}}").ToList();

                var keyworkStr = String.Join(",", listKeyworkStr);

                //prepair search body
                var body = "{\"size\":" + 10000 + "," +
                            "\"query\":" +
                                "{" +
                                    "\"filtered\":" +
                                        "{" +
                                            "\"query\":" +
                                                "{" +
                                                    "\"bool\":" +
                                                        "{" +
                                                            "\"should\":" +
                                                                "[" +
                                                                    keyworkStr +
                                                                "]" +
                                                            "}" +
                                                        "}" +
                                                    "}" +
                                                "}" +
                                            "}";

                var request = new RestRequest(_indexName + "/" + _indexType + "/_search", Method.POST);
                request.AddParameter("", body, ParameterType.RequestBody);

                var content = _client.Execute(request).Content;
                var preConent = JsonConvert.DeserializeObject<esResultModel<esUserData>>(content);

                return preConent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
