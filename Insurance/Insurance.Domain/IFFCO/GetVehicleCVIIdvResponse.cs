using System.Xml.Serialization;

namespace Insurance.Domain.IFFCO;

public class GetVehicleCVIIdvReturn
{
    [XmlElement(ElementName = "curShowroomPrice", Namespace = "http://service.CVI.itgi.com")]
    public CurShowroomPrice CurShowroomPrice { get; set; }
    [XmlElement(ElementName = "erorMessage", Namespace = "http://service.CVI.itgi.com")]
    public string ErorMessage { get; set; }
    [XmlElement(ElementName = "idv", Namespace = "http://service.CVI.itgi.com")]
    public string Idv { get; set; }
    [XmlElement(ElementName = "maximumIdvAllowed", Namespace = "http://service.CVI.itgi.com")]
    public string MaximumIdvAllowed { get; set; }
    [XmlElement(ElementName = "minimumIdvAllowed", Namespace = "http://service.CVI.itgi.com")]
    public string MinimumIdvAllowed { get; set; }
    [XmlAttribute(AttributeName = "ns1", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string Ns1 { get; set; }
}


public class GetVehicleCVIIdvResponse
{
    [XmlElement(ElementName = "getVehicleCVIIdvReturn", Namespace = "http://service.CVI.itgi.com")]
    public GetVehicleCVIIdvReturn GetVehicleCVIIdvReturn { get; set; }
    [XmlAttribute(AttributeName = "xmlns")]
    public string Xmlns { get; set; }
}
[XmlRoot(ElementName = "curShowroomPrice", Namespace = "http://service.CVI.itgi.com")]
public class CurShowroomPrice
{
    [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string Nil { get; set; }
}

//[XmlRoot(ElementName = "erorMessage", Namespace = "http://service.CVI.itgi.com")]
//public class ErorMessage
//{
//    [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
//    public string Nil { get; set; }
//}