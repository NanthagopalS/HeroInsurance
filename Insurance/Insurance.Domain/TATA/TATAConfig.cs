using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA;
public class TATAConfig
{
    public string BaseURL { get; set; }
    public string FWQuoteURL { get; set; }
    public string TWQuoteURL { get; set; }
    public string FWProposalURL { get; set; }
    public string TWProposalURL { get; set; }
    public string TokenURL { get; set; }
    public string InsurerId { get; set; }
    public string InsurerName { get; set; }
    public string InsurerLogo { get; set; }
    public TokenHeader TokenHeader { get; set; }
    public string ProducerEmail { get; set; }
    public string ProducerCode { get; set; }
    public string FWProductId { get; set; }
    public string TWProductId { get; set; }
    public string FWProductCode { get; set; }
    public string TWProductCode { get; set; }
    public string VerifyCKYCURL { get; set; }
    public string PGStatusRedirectionURL { get; set; }
    public string FWVerifyPaymentStatusURL { get; set; }
    public string TWVerifyPaymentStatusURL { get; set; }
    public string PGSubmitPayment { get; set; }
    public string FWPaymentLinkURL { get; set; }
    public string TWPaymentLinkURL { get; set; }
    public string FWPolicyDocumentURL { get; set; }
    public string TWPolicyDocumentURL { get; set; }
    public string VerifyBreakInUrl { get; set; }
    public string TataDocumentUploadURL { get; set; }
	public string TataKycOtpSubmitURL { get; set; }
    public string POSPOfficeLocationCode { get; set; }
}

public class TokenHeader
{
    public string grant_type { get; set; }
    public string scope { get; set; }
    public string client_id { get; set; }
    public string client_secret { get; set; }
    public string apiKey { get; set; }
}