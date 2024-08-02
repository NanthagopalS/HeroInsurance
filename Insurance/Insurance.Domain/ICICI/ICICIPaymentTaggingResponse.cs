using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ICICI;

public class ICICIPaymentTaggingResponse
{
    public Paymententryresponse paymentEntryResponse { get; set; }
    public Paymentmappingresponse paymentMappingResponse { get; set; }
    public Paymenttagresponse paymentTagResponse { get; set; }
    public string cdbgResponse { get; set; }
    public string message { get; set; }
    public bool status { get; set; }
    public string statusMessage { get; set; }
    public string correlationId { get; set; }
}

public class Paymententryresponse
{
    public string paymentID { get; set; }
    public string error_ID { get; set; }
    public string errorText { get; set; }
    public string status { get; set; }
}

public class Paymentmappingresponse
{
    public Paymentmapresponselist[] paymentMapResponseList { get; set; }
}

public class Paymentmapresponselist
{
    public string policyNo { get; set; }
    public string coverNoteNo { get; set; }
    public string proposalNo { get; set; }
    public string paymentID { get; set; }
    public string error_ID { get; set; }
    public string errorText { get; set; }
    public string status { get; set; }
}

public class Paymenttagresponse
{
    public Paymenttagresponselist[] paymentTagResponseList { get; set; }
}

public class Paymenttagresponselist
{
    public string policyNo { get; set; }
    public string coverNoteNo { get; set; }
    public string proposalNo { get; set; }
    public string paymentID { get; set; }
    public string error_ID { get; set; }
    public string errorText { get; set; }
    public string status { get; set; }
}
public class ICICIPaymentTaggingResponseDto
{
    public ICICIPaymentTaggingResponse iCICIPaymentTaggingResponse { get; set; }
    public string InsurerId { get; set; }
    public string Amount { get; set; }
    public string QuoteTransactionId { get; set; }
    public string LeadId { get; set; }
    public string CustomerId { get; set; }
    public string DealId { get; set; }
    public string VehicleTypeId { get; set; }
    public string PolicyNumber { get; set; }
    public string PolicyTypeId { get; set; }
    public string ProductCode { get; set; }
}
