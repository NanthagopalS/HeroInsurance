namespace Insurance.Domain.Oriental;

public class OrientalUploadCKYCResponse
{
    public int statusCode { get; set; }
    public Metadata metaData { get; set; }
    public string application_status { get; set; }
    public OrientalCKYCResult result { get; set; }
}

public class Metadata
{
    public string requestId { get; set; }
    public string transactionId { get; set; }
}

public class OrientalCKYCResult
{
    public Details details { get; set; }
}

public class Details
{
    public Front front { get; set; }
    public Back back { get; set; }
    public Dbcheck dbCheck { get; set; }
    public Namematch nameMatch { get; set; }
}

public class Front
{
    public string status { get; set; }
    public int statusCode { get; set; }
    public Metadata1 metadata { get; set; }
    public Result1 result { get; set; }
}

public class Metadata1
{
    public string requestId { get; set; }
    public string transactionId { get; set; }
}

public class Result1
{
    public Detail[] details { get; set; }
    public Summary summary { get; set; }
}

public class Summary
{
    public string action { get; set; }
    public object[] details { get; set; }
}

public class Detail
{
    public string idType { get; set; }
    public Fieldsextracted fieldsExtracted { get; set; }
    public string croppedImageUrl { get; set; }
}

public class Fieldsextracted
{
    public Firstname firstName { get; set; }
    public Middlename middleName { get; set; }
    public Lastname lastName { get; set; }
    public Fullname fullName { get; set; }
    public Dateofbirth dateOfBirth { get; set; }
    public Dateofissue dateOfIssue { get; set; }
    public Dateofexpiry dateOfExpiry { get; set; }
    public Countrycode countryCode { get; set; }
    public Type type { get; set; }
    public Address address { get; set; }
    public Gender gender { get; set; }
    public Idnumber idNumber { get; set; }
    public Placeofbirth placeOfBirth { get; set; }
    public Placeofissue placeOfIssue { get; set; }
    public Yearofbirth yearOfBirth { get; set; }
    public Age age { get; set; }
    public Fathername fatherName { get; set; }
    public Mothername motherName { get; set; }
    public Husbandname husbandName { get; set; }
    public Spousename spouseName { get; set; }
    public Nationality nationality { get; set; }
    public Mrzstring mrzString { get; set; }
    public Hometown homeTown { get; set; }
    public Relation relation { get; set; }
    public Relationname relationName { get; set; }
    public Vehicleclass vehicleClass { get; set; }
    public Qr qr { get; set; }
    public string url { get; set; }
    public string tag { get; set; }
}

public class Firstname
{
    public string value { get; set; }
}

public class Middlename
{
    public string value { get; set; }
}

public class Lastname
{
    public string value { get; set; }
}

public class Fullname
{
    public string value { get; set; }
}

public class Dateofbirth
{
    public string value { get; set; }
}

public class Dateofissue
{
    public string value { get; set; }
}

public class Dateofexpiry
{
    public string value { get; set; }
}

public class Countrycode
{
    public string value { get; set; }
}

public class Type
{
    public string value { get; set; }
}

public class Address
{
    public string line1 { get; set; }
    public string line2 { get; set; }
    public string city { get; set; }
    public string locality { get; set; }
    public string landmark { get; set; }
    public string careOf { get; set; }
    public string value { get; set; }
    public string houseNumber { get; set; }
    public string additionalInfo { get; set; }
    public string province { get; set; }
    public string district { get; set; }
    public string street { get; set; }
    public string zipCode { get; set; }
}

public class Gender
{
    public string value { get; set; }
}

public class Idnumber
{
    public string value { get; set; }
    public string ismasked { get; set; }
}

public class Placeofbirth
{
    public string value { get; set; }
}

public class Placeofissue
{
    public string value { get; set; }
}

public class Yearofbirth
{
    public string value { get; set; }
}

public class Age
{
    public string value { get; set; }
}

public class Fathername
{
    public string value { get; set; }
}

public class Mothername
{
    public string value { get; set; }
}

public class Husbandname
{
    public string value { get; set; }
}

public class Spousename
{
    public string value { get; set; }
}

public class Nationality
{
    public string value { get; set; }
}

public class Mrzstring
{
    public string value { get; set; }
    public string idNumber { get; set; }
    public string fullName { get; set; }
    public string dateOfBirth { get; set; }
    public string dateOfExpiry { get; set; }
    public string gender { get; set; }
    public string nationality { get; set; }
    public string valid { get; set; }
}

public class Hometown
{
    public string value { get; set; }
}

public class Qr
{
    public string value { get; set; }
}

public class Relation
{
    public string value { get; set; }
}
public class Relationname
{
    public string value { get; set; }
}

public class Vehicleclass
{
    public string value { get; set; }
}

public class Back
{
    public string status { get; set; }
    public int statusCode { get; set; }
    public Metadata2 metadata { get; set; }
    public Result2 result { get; set; }
}

public class Metadata2
{
    public string requestId { get; set; }
    public string transactionId { get; set; }
}

public class Result2
{
    public string error { get; set; }
    public Detail1[] details { get; set; }
    public Summary1 summary { get; set; }
}

public class Summary1
{
    public string action { get; set; }
    public object[] details { get; set; }
}

public class Detail1
{
    public string idType { get; set; }
    public Fieldsextracted1 fieldsExtracted { get; set; }
    public string croppedImageUrl { get; set; }
}

public class Fieldsextracted1
{
    public Firstname1 firstName { get; set; }
    public Middlename1 middleName { get; set; }
    public Lastname1 lastName { get; set; }
    public Fullname1 fullName { get; set; }
    public Dateofbirth1 dateOfBirth { get; set; }
    public Dateofissue1 dateOfIssue { get; set; }
    public Dateofexpiry1 dateOfExpiry { get; set; }
    public Countrycode1 countryCode { get; set; }
    public Type1 type { get; set; }
    public Address1 address { get; set; }
    public Gender1 gender { get; set; }
    public Idnumber1 idNumber { get; set; }
    public Placeofbirth1 placeOfBirth { get; set; }
    public Placeofissue1 placeOfIssue { get; set; }
    public Yearofbirth1 yearOfBirth { get; set; }
    public Age1 age { get; set; }
    public Fathername1 fatherName { get; set; }
    public Mothername1 motherName { get; set; }
    public Husbandname1 husbandName { get; set; }
    public Spousename1 spouseName { get; set; }
    public Nationality1 nationality { get; set; }
    public Mrzstring1 mrzString { get; set; }
    public Hometown1 homeTown { get; set; }
    public File_Num file_num { get; set; }
    public Old_Doi old_doi { get; set; }
    public Old_Passport_Num old_passport_num { get; set; }
    public Old_Place_Of_Issue old_place_of_issue { get; set; }
    public Date date { get; set; }
    public string tag { get; set; }
    public Qr1 qr { get; set; }
    public string url { get; set; }
}

public class Firstname1
{
    public string value { get; set; }
}

public class Middlename1
{
    public string value { get; set; }
}

public class Lastname1
{
    public string value { get; set; }
}

public class Fullname1
{
    public string value { get; set; }
}

public class Dateofbirth1
{
    public string value { get; set; }
}

public class Dateofissue1
{
    public string value { get; set; }
}

public class Dateofexpiry1
{
    public string value { get; set; }
}

public class Countrycode1
{
    public string value { get; set; }
}

public class Type1
{
    public string value { get; set; }
}

public class Address1
{
    public string value { get; set; }
    public string line1 { get; set; }
    public string line2 { get; set; }
    public string city { get; set; }
    public string locality { get; set; }
    public string street { get; set; }
    public string district { get; set; }
    public string landmark { get; set; }
    public string houseNumber { get; set; }
    public string zipCode { get; set; }
    public string province { get; set; }
    public string additionalInfo { get; set; }
    public string care_of { get; set; }
}

public class Gender1
{
    public string value { get; set; }
}

public class Idnumber1
{
    public string value { get; set; }
    public string ismasked { get; set; }
}

public class Placeofbirth1
{
    public string value { get; set; }
}

public class Placeofissue1
{
    public string value { get; set; }
}

public class Yearofbirth1
{
    public string value { get; set; }
}

public class Age1
{
    public string value { get; set; }
}

public class Fathername1
{
    public string value { get; set; }
}

public class Mothername1
{
    public string value { get; set; }
}

public class Husbandname1
{
    public string value { get; set; }
}

public class Spousename1
{
    public string value { get; set; }
}

public class Nationality1
{
    public string value { get; set; }
}

public class Mrzstring1
{
    public string value { get; set; }
    public string idNumber { get; set; }
    public string fullName { get; set; }
    public string dateOfBirth { get; set; }
    public string dateOfExpiry { get; set; }
    public string gender { get; set; }
    public string nationality { get; set; }
}

public class Hometown1
{
    public string value { get; set; }
}

public class Qr1
{
    public string value { get; set; }
}
public class Date
{
    public string value { get; set; }
}
public class Old_Place_Of_Issue
{
    public string value { get; set; }
}
public class File_Num
{
    public string value { get; set; }
}

public class Old_Doi
{
    public string value { get; set; }
}

public class Old_Passport_Num
{
    public string value { get; set; }
}

public class Dbcheck
{
    public string status { get; set; }
    public string statusCode { get; set; }
    public Result3 result { get; set; }
}

public class Result3
{
    public Details1 details { get; set; }
    public Summary2 summary { get; set; }
}

public class Details1
{
    public string issue_date { get; set; }
    public string fatherhusband { get; set; }
    public string ac_no { get; set; }
    public string rln_name { get; set; }
    public string part_no { get; set; }
    public string name_v3 { get; set; }
    public string ps_lat_long { get; set; }
    public string st_code { get; set; }
    public string id { get; set; }
    public string district { get; set; }
    public string rln_name_v1 { get; set; }
    public string state { get; set; }
    public string slno_inpart { get; set; }
    public string section_no { get; set; }
    public string last_update { get; set; }
    public string rln_name_v2 { get; set; }
    public string rln_name_v3 { get; set; }
    public string ac_name { get; set; }
    public string ps_name { get; set; }
    public string house_no { get; set; }
    public string rln_type { get; set; }
    public string pc_name { get; set; }
    public string name { get; set; }
    public string img { get; set; }
    public string blood_group { get; set; }
    public string dob { get; set; }
    public string gender { get; set; }
    public int age { get; set; }
    public string name_v2 { get; set; }
    public string name_v1 { get; set; }
    public string part_name { get; set; }
    public string idNumber { get; set; }
    public string givenName { get; set; }
    public Validity validity { get; set; }
    public Cov_Details[] cov_details { get; set; }
    public string address { get; set; }
    public string surName { get; set; }
    public string doi { get; set; }
    public string applicationDate { get; set; }
    public string typeOfApplication { get; set; }
}
public class Validity
{
    public string transport { get; set; }
    public string nontransport { get; set; }
}

public class Cov_Details
{
    public string cov { get; set; }
    public string issue_date { get; set; }
}
public class Summary2
{
    public string action { get; set; }
    public object[] details { get; set; }
}

public class Namematch
{
    public string status { get; set; }
    public string statusCode { get; set; }
    public Result4 result { get; set; }
}

public class Result4
{
    public bool name { get; set; }
    public bool all { get; set; }
}

