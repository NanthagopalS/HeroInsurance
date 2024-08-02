namespace Insurance.Domain.IFFCO;

public class IFFCOCreateCKYCResponseModel
{
    public string status { get; set; }
    public IffcoCreateCKYCResult result { get; set; }
}

public class IffcoCreateCKYCResult
{
    public string itgiUniqueReferenceId { get; set; }
    public string status { get; set; }
    public string documentStored { get; set; }
    public string recordCreated { get; set; }
}
