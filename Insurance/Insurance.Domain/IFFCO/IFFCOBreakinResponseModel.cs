namespace Insurance.Domain.IFFCO;

public class IFFCOBreakinResponseModel
{
    public int ID { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhoneNumber { get; set; }
    public string CustomerEmail { get; set; }
    public string VehicleNumber { get; set; }
    public string FuelType { get; set; }
    public string QuoteNumber { get; set; }
    public bool InspectionByWimwisure { get; set; }
    public bool CommunicationByWimwisure { get; set; }
    public string Status { get; set; }
    public string CreationTime { get; set; }
    public string InspectionTime { get; set; }
    public string InspectionNumber { get; set; }
    public string ChassisNumber { get; set; }
    public string EngineNumber { get; set; }
    public object Remarks { get; set; }
    public object Comment { get; set; }
    public object InspectionLink { get; set; }
    public string DownloadKey { get; set; }
    public string callbackURL { get; set; }
    public Inspectionby InspectionBy { get; set; }
    public Inspector[] Inspectors { get; set; }
    public string AppVersion { get; set; }
    public object QcTime { get; set; }
    public Latestcomment LatestComment { get; set; }
    public Requiredphotos RequiredPhotos { get; set; }
    public object[] NotifyEmail { get; set; }
    public object[] NotifyPhone { get; set; }
    public string ReportLink { get; set; }
    public string PhotosDownloadLink { get; set; }
}

public class Inspectionby
{
    public string PhoneNumber { get; set; }
    public string InspectionLink { get; set; }
}

public class Latestcomment
{
    public string Comment { get; set; }
    public string Timestamp { get; set; }
}

public class Requiredphotos
{
    public string TransactionId { get; set; }
    public Photo[] Photos { get; set; }
}

public class Photo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string GroupName { get; set; }
    public string InputType { get; set; }
    public bool Multiple { get; set; }
    public string Comment { get; set; }
}

public class Inspector
{
    public string PhoneNumber { get; set; }
    public string InspectionLink { get; set; }
}
