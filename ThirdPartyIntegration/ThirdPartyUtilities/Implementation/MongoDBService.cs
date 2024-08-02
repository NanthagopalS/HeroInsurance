using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Configuration;
using ThirdPartyUtilities.Models.MongoDB;

namespace ThirdPartyUtilities.Implementation;
public class MongoDBService : IMongoDBService
{
    private readonly ILogger<MongoDBService> _logger;
    public readonly MongoDBContext _mongoDBContext;
    private readonly MongodbConnection _mongodbConnection;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="logger"></param>
    public MongoDBService(ILogger<MongoDBService> logger, MongoDBContext mongoDBContext, IOptions<MongodbConnection> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mongoDBContext = mongoDBContext ?? throw new ArgumentNullException(nameof(mongoDBContext));
        _mongodbConnection = options.Value;
    }


    public async Task<string> MongoUpload(string filename, Stream stream, byte[] ImageStream)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
                {
                    {"ContentType","image/png"},
                    //{"ContentType","image/jpeg"},
                    //{"ContentType","image/jpg"}
                }
        };

        var id = await _mongoDBContext.ImagesBucket.UploadFromStreamAsync(filename, stream, options);

        return id.ToString();
    }


    public async Task<string> MongoCertificateUpload(string filename, Stream stream, byte[] ImageStream)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
                {
                    {"ContentType","html"},
                    //{"ContentType","image/jpeg"},
                    //{"ContentType","image/jpg"}
                }
        };

        var id = await _mongoDBContext.CertifictaeBucket.UploadFromStreamAsync(filename, stream, options);

        return id.ToString();
    }

    public async Task<string> MongoDownload(string id)
    {
        try
        {
            byte[] fileByte = await _mongoDBContext.ImagesBucket.DownloadAsBytesAsync(new ObjectId(id));

            var imageData = Convert.ToBase64String(fileByte);

            return imageData;
        }
        catch (Exception ex)
        {
            _logger.LogError("MongoDB Download File Error {Message}", ex.Message);
            return default;
        }
    }
    /// <summary>
    /// MongoDownloadTestomonialDoc
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<string> MongoDownloadTestomonialDoc(string id)
    {
        try
        {
            byte[] fileByte = await _mongoDBContext.TestomonialBucket.DownloadAsBytesAsync(new ObjectId(id));

            var imageData = Convert.ToBase64String(fileByte);

            return imageData;
        }
        catch (Exception ex)
        {
            _logger.LogError("MongoDB Download File Error {Message}", ex.Message);
            return default;
        }
    }

    public async Task<string> MongoCertificataeDownload(string id)
    {
        try
        {
            MongoDBResponse mongodbresponse = new MongoDBResponse();
            var mongoUrl = new MongoDB.Driver.MongoUrl(_mongodbConnection.ConnectionString);
            var client = new MongoClient(mongoUrl);
            var database = client.GetDatabase(_mongodbConnection.DatabaseName);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", new ObjectId(id));
            var fileInfo = _mongoDBContext.CertifictaeBucket.Find(filter);
            var file = fileInfo.FirstOrDefault();

            byte[] fileByte = await _mongoDBContext.CertifictaeBucket.DownloadAsBytesAsync(new ObjectId(id));

            var imageData = Convert.ToBase64String(fileByte);

            return imageData;
        }
        catch (Exception ex)
        {
            _logger.LogError("MongoDB Download File Error {Message}", ex.Message);
            return default;
        }
    }

    public async Task<string> MongoAgreementUpload(string filename, Stream stream, byte[] SignatureImage)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
                {
                    {"ContentType","html"},
                }
        };

        var id = await _mongoDBContext.AgreementBucket.UploadFromStreamAsync(filename, stream, options);

        return id.ToString();
    }

    public async Task<string> MongoAgreementDownload(string id)
    {
        try
        {
            MongoDBResponse mongodbresponse = new MongoDBResponse();
            var mongoUrl = new MongoDB.Driver.MongoUrl(_mongodbConnection.ConnectionString);
            var client = new MongoClient(mongoUrl);
            var database = client.GetDatabase(_mongodbConnection.DatabaseName);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", new ObjectId(id));
            var fileInfo = _mongoDBContext.AgreementBucket.Find(filter);
            var file = fileInfo.FirstOrDefault();

            byte[] fileByte = await _mongoDBContext.AgreementBucket.DownloadAsBytesAsync(new ObjectId(id));

            var imageData = Convert.ToBase64String(fileByte);

            return imageData;
        }
        catch (Exception ex)
        {
            _logger.LogError("MongoDB Download File Error {Message}", ex.Message);
            return default;
        }
    }

    public async Task<string> MongoSignatureUpload(string filename, Stream stream, byte[] SignatureImage)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
                {
                    {"ContentType","html"},
                }
        };

        var id = await _mongoDBContext.POSPSignature.UploadFromStreamAsync(filename, stream, options);

        return id.ToString();
    }


    public async Task<string> MongoAgreementSignatureDownload(string id)
    {
        try
        {
            MongoDBResponse mongodbresponse = new MongoDBResponse();
            var mongoUrl = new MongoDB.Driver.MongoUrl(_mongodbConnection.ConnectionString);
            var client = new MongoClient(mongoUrl);
            var database = client.GetDatabase(_mongodbConnection.DatabaseName);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", new ObjectId(id));
            var fileInfo = _mongoDBContext.AgreementBucket.Find(filter);
            var file = fileInfo.FirstOrDefault();

            byte[] fileByte = await _mongoDBContext.POSPSignature.DownloadAsBytesAsync(new ObjectId(id));

            var imageData = Convert.ToBase64String(fileByte);

            return imageData;
        }
        catch (Exception ex)
        {
            _logger.LogError("MongoDB Download File Error {Message}", ex.Message);
            return default;
        }
    }

    public async Task<string> MongoDownloadForPOSP(string id)
    {
        try
        {
            MongoDBResponse mongodbresponse = new MongoDBResponse();
            var mongoUrl = new MongoDB.Driver.MongoUrl(_mongodbConnection.ConnectionString);
            var client = new MongoClient(mongoUrl);
            var database = client.GetDatabase(_mongodbConnection.DatabaseName);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", new ObjectId(id));
            var fileInfo = _mongoDBContext.ImagesBucket.Find(filter);
            var file = fileInfo.FirstOrDefault();

            byte[] fileByte = await _mongoDBContext.ImagesBucket.DownloadAsBytesAsync(new ObjectId(id));

            var imageData = Convert.ToBase64String(fileByte);

            return imageData;
        }
        catch (Exception ex)
        {
            _logger.LogError("MongoDB Download File Error {Message}", ex.Message);
            return default;
        }
    }

    public async void DeletePOSPDocument(string id)
    {
        try
        {
            await _mongoDBContext.ImagesBucket.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed To delete POSP Document :"+ id, ex.Message);
        }

    }

    public async void DeletePOSPExamCertificate(string id)
    {
        try
        {
            await _mongoDBContext.CertifictaeBucket.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed To delete POSP Exam Certificate :" + id, ex.Message);
        }

    }
}


