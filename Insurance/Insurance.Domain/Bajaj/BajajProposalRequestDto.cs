namespace Insurance.Domain.Bajaj;


public class BajajProposalRequestDto
{
    public string userid { get; set; }
    public string password { get; set; }
    public string transactionid { get; set; }
    public Rcptlist[] rcptlist { get; set; }
    public Custdetails custdetails { get; set; }
    public Weomotpolicyin weomotpolicyin { get; set; }
    public object[] accessorieslist { get; set; }
    public Paddoncoverlist[] paddoncoverlist { get; set; }
    public Motextracover motextracover { get; set; }
    public Premiumdetails premiumdetails { get; set; }
    public Premiumsummerylist[] premiumsummerylist { get; set; }
    public Questlist[] questlist { get; set; }
    public Detariffobj detariffobj { get; set; }
    public Potherdetails potherdetails { get; set; }
    public string premiumpayerid { get; set; }
    public string paymentmode { get; set; }
}

public class Custdetails
{
    public string parttempid { get; set; }
    public string firstname { get; set; }
    public string middlename { get; set; }
    public string surname { get; set; }
    public string addline1 { get; set; }
    public string addline2 { get; set; }
    public string addline3 { get; set; }
    public string addline5 { get; set; }
    public string pincode { get; set; }
    public string email { get; set; }
    public string telephone1 { get; set; }
    public string telephone2 { get; set; }
    public string mobile { get; set; }
    public string delivaryoption { get; set; }
    public string poladdline1 { get; set; }
    public string poladdline2 { get; set; }
    public string poladdline3 { get; set; }
    public string poladdline5 { get; set; }
    public string polpincode { get; set; }
    public object password { get; set; }
    public string cptype { get; set; }
    public string profession { get; set; }
    public string dateofbirth { get; set; }
    public string availabletime { get; set; }
    public string institutionname { get; set; }
    public string existingyn { get; set; }
    public string loggedin { get; set; }
    public string mobilealerts { get; set; }
    public string emailalerts { get; set; }
    public string title { get; set; }
    public string partid { get; set; }
    public string status1 { get; set; }
    public string status2 { get; set; }
    public string status3 { get; set; }
}
public class Potherdetails
{
    public string imdcode { get; set; }
    public string covernoteno { get; set; }
    public string leadno { get; set; }
    public string ccecode { get; set; }
    public string runnercode { get; set; }
    public string extra1 { get; set; }
    public string extra2 { get; set; }
    public string extra3 { get; set; }
    public string extra4 { get; set; }
    public string extra5 { get; set; }
}
public class Rcptlist
{
}
