namespace ThirdPartyUtilities.Abstraction;
public interface IMongoDBService
{

    Task<string> MongoUpload(string filename, Stream stream, byte[] ImageStream);
    Task<string> MongoCertificateUpload(string filename, Stream stream, byte[] ImageStream);

    Task<string> MongoDownload(string id);
    Task<string> MongoCertificataeDownload(string id);

    Task<string> MongoAgreementUpload(string filename, Stream stream, byte[] SignatureImage);
    Task<string> MongoAgreementDownload(string id);
    Task<string> MongoSignatureUpload(string filename, Stream stream, byte[] SignatureImage);

    Task<string> MongoAgreementSignatureDownload(string id);
    Task<string> MongoDownloadForPOSP(string id);

    Task<string> MongoDownloadTestomonialDoc(string id);

    void DeletePOSPDocument(string id);
    void DeletePOSPExamCertificate(string id);

}
