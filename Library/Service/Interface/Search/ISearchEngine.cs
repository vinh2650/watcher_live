using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interface.Search
{

    /// <summary>
    /// Search engine for tribo
    /// </summary>
    public interface ISearchEngine
    {
        /// <summary>
        /// create index 
        /// </summary>
        /// <param name="indexName">must in lower case</param>
        /// <param name="customAnalyzer"></param>
        /// <returns></returns>
        Task CreateIndexAsync(string indexName, string customAnalyzer);


        /// <summary>
        /// create index 
        /// </summary>
        /// <param name="indexName">must in lower case</param>
        /// <param name="customAnalyzer"></param>
        /// <returns></returns>
        void CreateIndex(string indexName, string customAnalyzer);

        /// <summary>
        /// Delete 1 index 
        /// </summary>
        /// <param name="indexName">must be in lower case</param>
        /// <returns></returns>
        Task DeleteIndexAsync(string indexName);

        /// <summary>
        /// create index 
        /// </summary>
        /// <param name="indexName">must in lower case</param>
        /// <param name="typeName"></param>
        /// <param name="mappingData"></param>
        /// <returns></returns>
        void MappingIndex(string indexName, string typeName, string mappingData);

        /// <summary>
        /// Delete 1 index 
        /// </summary>
        /// <param name="indexName">must be in lower case</param>
        /// <param name="typeName"></param>
        /// <param name="mappingData"></param>
        /// <returns></returns>
        Task MappingIndexAsync(string indexName, string typeName, string mappingData);

        /// <summary>
        /// Delete 1 index 
        /// </summary>
        /// <param name="indexName">must be in lower case</param>
        /// <returns></returns>
        void DeleteIndex(string indexName);

        /// <summary>
        /// bulk index
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="entitytype"></param>
        /// <param name="dataIndex"></param>
        void BulkIndex(string indexName, string entitytype, string dataIndex);

        /// <summary>
        /// bulk index with routing
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="entitytype"></param>
        /// <param name="dataIndex"></param>
        /// <param name="operatorId"></param>
        void BulkIndex(string indexName, string entitytype, string dataIndex, string operatorId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="entitytype"></param>
        /// <param name="dataId"></param>
        /// <param name="dataIndex"></param>
        void IndexData(string indexName, string entitytype, string dataId, string dataIndex);

        /// <summary>
        /// update index
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="entityType"></param>
        /// <param name="dataUpdate"></param>
        void UpdateBulk(string indexName, string entityType, string dataUpdate);

        /// <summary>
        /// delete by temp
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="entityType"></param>
        /// <param name="tempId"></param>
        void DeleteByTemp(string indexName, string entityType, string tempId);

        /// <summary>
        /// delete by temp for version 3
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="operatorId"></param>
        /// <param name="indexName"></param>
        /// <param name="entityType"></param>
        /// <param name="tempId"></param>
        void DeleteByTempV3(string jobId, string operatorId, string indexName, string entityType, string tempId);

        /// <summary>
        /// Create a snapshot of index
        /// </summary>
        /// <param name="repoName"></param>
        /// <param name="snapshotName"></param>
        void TakeSnapshot(string repoName, string snapshotName);

        /// <summary>
        /// Create Repository for holding backup(snapshot) file
        /// </summary>
        void CreateRepository(string path, string repoName);

        /// <summary>
        /// Restore index from a snapshot
        /// </summary>
        /// <param name="repoName"></param>
        /// <param name="snapshotName"></param>
        /// <param name="restoreName"></param>
        void RestoreFromSnapshot(string repoName, string snapshotName,string restoreName);

        /// <summary>
        /// Reindex data from one index to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>

        void ReindexData(string source, string dest);
    }

    public class BaseSearchResult<T> where T : class
    {
        public List<T> Result { get; set; }

        public int Count { get; set; }


    }
    public class BaseLocationSearchResult
    {
        public string Id { get; set; }
        public GeoPoint location { get; set; }

        public double distance { get; set; }

        //public int Type { get; set; }


    }
    public class LocationModel
    {
        public string Id { get; set; }

        // public AmsType Type { get; set; }

        public GeoPoint location { get; set; }

    }

    //public enum AmsType
    //{
    //     Bts = 1,
    //     Biji = 2,
    //     Uti = 3
    //}

    public class GeoPoint
    {
        public double lat { get; set; }
        public double lon { get; set; }

    }
}
