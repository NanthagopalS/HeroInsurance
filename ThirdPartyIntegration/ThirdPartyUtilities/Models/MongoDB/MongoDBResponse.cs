using MongoDB.Driver.GridFS;

namespace ThirdPartyUtilities.Models.MongoDB
{
    public class MongoDBResponse
    {
        //public GridFSDownloadStream GridFSDownloadStream { get; set; }

        public string Image { get; set; }
    }


    public class MongodbConnection
    {
        public string CollectionName { get; set; }
        public string CollectionName1 { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }

}
