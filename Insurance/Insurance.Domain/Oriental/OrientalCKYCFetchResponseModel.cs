namespace Insurance.Domain.Oriental;

public class OrientalCKYCFetchResponseModel
{
    public string status { get; set; }
    public string statusCode { get; set; }
    public Result result { get; set; }
    public Error error { get; set; }

}
public class Result
{
    public string constituitonType { get; set; }
    public string accountType { get; set; }
    public string ckycNo { get; set; }
    public string preFix { get; set; }
    public string firstName { get; set; }
    public string middleName { get; set; }
    public string lastName { get; set; }
    public string fullName { get; set; }
    public string fatherOrSpouse { get; set; }
    public string fatherPrefix { get; set; }
    public string fatherFname { get; set; }
    public string fatherMname { get; set; }
    public string fatherLname { get; set; }
    public string fatherFullName { get; set; }
    public string motherPrefix { get; set; }
    public string motherFname { get; set; }
    public string motherMname { get; set; }
    public string motherLname { get; set; }
    public string motherFullName { get; set; }
    public string gender { get; set; }
    public string dob { get; set; }
    public string age { get; set; }
    public string address1 { get; set; }
    public string address2 { get; set; }
    public string address3 { get; set; }
    public string city { get; set; }
    public string district { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public string pincode { get; set; }
    public string permAndCorresAddSame { get; set; }
    public string corresAddress1 { get; set; }
    public string corresAddress2 { get; set; }
    public string corresAddress3 { get; set; }
    public string corresCity { get; set; }
    public string corresDist { get; set; }
    public string corresState { get; set; }
    public string corresCountry { get; set; }
    public string corresPin { get; set; }
    public string resiStdCode { get; set; }
    public string resiTelNo { get; set; }
    public string mobileCode { get; set; }
    public string mobileNumber { get; set; }
    public string email { get; set; }
    public string decDate { get; set; }
    public string decPlace { get; set; }
    public string kycDate { get; set; }
    public string updatedDate { get; set; }
    public string idList { get; set; }
    public string DocSub { get; set; }
    public string kycName { get; set; }
    public string kycDesignation { get; set; }
    public string kycBranch { get; set; }
    public string kycEmpCode { get; set; }
    public string numIdentity { get; set; }
    public string numRelated { get; set; }
    public string numImages { get; set; }
    public string pan { get; set; }
    public string voterId { get; set; }
    public string passport { get; set; }
    public string drivingLicense { get; set; }
    public string nregaJobCard { get; set; }
    public string nationalPopulationReg { get; set; }
    public Imagedetail[] imageDetails { get; set; }
}
public class Imagedetail
{
    public string code { get; set; }
    public string type { get; set; }
    public string imageUrl { get; set; }
}
public class Error
{
    public string code { get; set; }
    public string message { get; set; }
}
