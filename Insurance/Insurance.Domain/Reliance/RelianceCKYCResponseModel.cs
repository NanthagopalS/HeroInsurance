using Newtonsoft.Json;

namespace Insurance.Domain.Reliance;
public class RelianceCKYCResponseModel
{
    public bool success { get; set; }
    public string message { get; set; }
    public string Unique_Id { get; set; }
    public string Endpoint_2_URL { get; set; }
    public string PAN_Verified { get; set; }
    public string KYC_Verified { get; set; }
    public KycData kyc_data { get; set; }
    public string verified_at { get; set; }
}

public class CKYC
{
    public string req_id { get; set; }
    public bool success { get; set; }
    public string error_message { get; set; }
    public string ckyc_remarks { get; set; }
    [JsonProperty("Result")]
    public CKYCResult result { get; set; }
}

public class IDENTITY
{
    public string SEQUENCE_NO { get; set; }
    public string IDENT_TYPE { get; set; }
    public string IDENT_NUM { get; set; }
    public string IDVER_STATUS { get; set; }
    public string IDENT_NAME { get; set; }
}

public class IDENTITYDETAILS
{
    public List<IDENTITY> IDENTITY { get; set; }
}

public class KycData
{
    public CKYC CKYC { get; set; }
}

public class PERSONALDETAILS
{
    public string CONSTI_TYPE { get; set; }
    public string ACC_TYPE { get; set; }
    public string CKYC_NO { get; set; }
    public string PREFIX { get; set; }
    public string FNAME { get; set; }
    public string MNAME { get; set; }
    public string LNAME { get; set; }
    public string FULLNAME { get; set; }
    public string MAIDEN_PREFIX { get; set; }
    public string MAIDEN_FNAME { get; set; }
    public string MAIDEN_MNAME { get; set; }
    public string MAIDEN_LNAME { get; set; }
    public string MAIDEN_FULLNAME { get; set; }
    public string FATHERSPOUSE_FLAG { get; set; }
    public string FATHER_PREFIX { get; set; }
    public string FATHER_FNAME { get; set; }
    public string FATHER_MNAME { get; set; }
    public string FATHER_LNAME { get; set; }
    public string FATHER_FULLNAME { get; set; }
    public string MOTHER_PREFIX { get; set; }
    public string MOTHER_FNAME { get; set; }
    public string MOTHER_MNAME { get; set; }
    public string MOTHER_LNAME { get; set; }
    public string MOTHER_FULLNAME { get; set; }
    public string GENDER { get; set; }
    public string DOB { get; set; }
    public string PAN { get; set; }
    public string FORM_SIXTY { get; set; }
    public string PERM_LINE1 { get; set; }
    public string PERM_LINE2 { get; set; }
    public string PERM_LINE3 { get; set; }
    public string PERM_CITY { get; set; }
    public string PERM_DIST { get; set; }
    public string PERM_STATE { get; set; }
    public string PERM_COUNTRY { get; set; }
    public string PERM_PIN { get; set; }
    public string PERM_POA { get; set; }
    public string PERM_CORRES_SAMEFLAG { get; set; }
    public string CORRES_LINE1 { get; set; }
    public string CORRES_LINE2 { get; set; }
    public string CORRES_LINE3 { get; set; }
    public string CORRES_CITY { get; set; }
    public string CORRES_DIST { get; set; }
    public string CORRES_STATE { get; set; }
    public string CORRES_COUNTRY { get; set; }
    public string CORRES_PIN { get; set; }
    public string CORRES_POA { get; set; }
    public string RESI_STD_CODE { get; set; }
    public string RESI_TEL_NUM { get; set; }
    public string OFF_STD_CODE { get; set; }
    public string OFF_TEL_NUM { get; set; }
    public string MOB_CODE { get; set; }
    public string MOB_NUM { get; set; }
    public string EMAIL { get; set; }
    public string REMARKS { get; set; }
    public string DEC_DATE { get; set; }
    public string DEC_PLACE { get; set; }
    public string KYC_DATE { get; set; }
    public string DOC_SUB { get; set; }
    public string KYC_NAME { get; set; }
    public string KYC_DESIGNATION { get; set; }
    public string KYC_BRANCH { get; set; }
    public string KYC_EMPCODE { get; set; }
    public string ORG_NAME { get; set; }
    public string ORG_CODE { get; set; }
    public string NUM_IDENTITY { get; set; }
    public string NUM_RELATED { get; set; }
    public string NUM_IMAGES { get; set; }
    public string UPDATED_DATE { get; set; }
    public string Error_Message { get; set; }
    public string AGE { get; set; }
    public string IMAGE_TYPE { get; set; }
    public string PHOTO { get; set; }
}

public class CKYCResult
{
    public PERSONALDETAILS PERSONAL_DETAILS { get; set; }
    public IDENTITYDETAILS IDENTITY_DETAILS { get; set; }
    public string RELATED_PERSON_DETAILS { get; set; }
}



