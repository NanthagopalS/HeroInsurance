using Insurance.Domain.GoDigit.Request;

namespace Insurance.Domain.GoDigit;
public class GoDigitProposalDto : GoDigitRequestDto
{
    public dynamic[] persons { get; set; }
    public Nominee nominee { get; set; }
    public Pospinfo pospInfo { get; set; }
    public Dealer dealer { get; set; }
    public Motorquestions motorQuestions { get; set; }
    public kyc kyc { get; set; }
    public Motorbreakin motorBreakin { get; set; }
}

public class Nominee
{
    public string firstName { get; set; }
    public string middleName { get; set; }
    public string lastName { get; set; }
    public string dateOfBirth { get; set; }
    public string relation { get; set; }
    public string personType { get; set; }
}

public class Pospinfo
{
    public bool isPOSP { get; set; }
    public string pospName { get; set; }
    public string pospUniqueNumber { get; set; }
    public string pospLocation { get; set; }
    public string pospPanNumber { get; set; }
    public string pospAadhaarNumber { get; set; }
    public string pospContactNumber { get; set; }
}

public class Dealer
{
    public string dealerName { get; set; }
    public string city { get; set; }
    public object deliveryDate { get; set; }
}

public class Motorquestions
{
    public string furtherAgreement { get; set; }
    public bool selfInspection { get; set; }
    public string financer { get; set; }
}

public class IndividualPerson
{
    public string personType { get; set; }
    public string partyId { get; set; }
    public Address[] addresses { get; set; }
    public Communication[] communications { get; set; }
    public Identificationdocument[] identificationDocuments { get; set; }
    public bool isPolicyHolder { get; set; }
    public bool isVehicleOwner { get; set; }
    public string firstName { get; set; }
    public object middleName { get; set; }
    public string lastName { get; set; }
    public string dateOfBirth { get; set; }
    public string gender { get; set; }
    public bool isDriver { get; set; }
    public bool isInsuredPerson { get; set; }
}
public class CompanyPerson
{
    public string personType { get; set; }
    public string partyId { get; set; }
    public Address[] addresses { get; set; }
    public Communication[] communications { get; set; }
    public Identificationdocument[] identificationDocuments { get; set; }
    public bool isPolicyHolder { get; set; }
    public bool isVehicleOwner { get; set; }
    public string isPayer { get; set; }
    public string companyName { get; set; }
}

public class Address
{
    public string addressType { get; set; }
    public string flatNumber { get; set; }
    public string streetNumber { get; set; }
    public string street { get; set; }
    public string district { get; set; }
    public string state { get; set; }
    public string city { get; set; }
    public string country { get; set; }
    public string pincode { get; set; }
}

public class Communication
{
    public string communicationType { get; set; }
    public string communicationId { get; set; }
    public bool isPrefferedCommunication { get; set; }
}

public class Identificationdocument
{
    public string identificationDocumentId { get; set; }
    public string documentType { get; set; }
    public string documentId { get; set; }
    public string issuingAuthority { get; set; }
    public string issuingPlace { get; set; }
    public string issueDate { get; set; }
    public string expiryDate { get; set; }
}
public class kyc
{
    public bool isKYCDone { get; set; }
    public string ckycReferenceDocId { get; set; }
    public string ckycReferenceNumber { get; set; }
    public string dateOfBirth { get; set; }
    public string photo { get; set; }
}