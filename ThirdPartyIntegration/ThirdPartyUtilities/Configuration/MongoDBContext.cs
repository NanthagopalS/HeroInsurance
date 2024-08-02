using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ThirdPartyUtilities.Models.MongoDB;

namespace ThirdPartyUtilities.Configuration
{
    public class MongoDBContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly MongodbConnection _mongodbConnection;

        public IMongoDatabase Database;
        public GridFSBucket ImagesBucket { get; set; }
        public GridFSBucket CertifictaeBucket { get; set; }
        public GridFSBucket AgreementBucket { get; set; }
        public GridFSBucket GetAgreementBucket { get; set; }
        public GridFSBucket POSPSignature { get; set; }
		public GridFSBucket TestomonialBucket { get; set; }




		public MongoDBContext(IConfiguration configuration, IOptions<MongodbConnection> options)
        {
            _configuration = configuration;
            _mongodbConnection = options.Value;
            var settings = MongoClientSettings.FromUrl(new MongoUrl(_mongodbConnection.ConnectionString));
            var client = new MongoClient(settings);
            Database = client.GetDatabase(_mongodbConnection.DatabaseName);
            ImagesBucket = new GridFSBucket(Database, new GridFSBucketOptions { BucketName = _mongodbConnection.CollectionName});
            CertifictaeBucket = new GridFSBucket(Database, new GridFSBucketOptions { BucketName = _mongodbConnection.CollectionName1});

            GetAgreementBucket = new GridFSBucket(Database, new GridFSBucketOptions { BucketName = "Template-Detail" });

            AgreementBucket = new GridFSBucket(Database, new GridFSBucketOptions { BucketName = "Agreement" });

            POSPSignature = new GridFSBucket(Database, new GridFSBucketOptions { BucketName = "POSP-Signature" });

            TestomonialBucket = new GridFSBucket(Database, new GridFSBucketOptions { BucketName = "HeroTestomonials" });

        }
    }
}
