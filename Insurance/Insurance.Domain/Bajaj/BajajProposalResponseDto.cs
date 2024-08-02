namespace Insurance.Domain.Bajaj;

public class BajajProposalResponseDto
{
    public List<object> rcptlist { get; set; }
    public string policyref { get; set; }
    public string policyissuedate { get; set; }
    public string partid { get; set; }
    public List<Errorlist> errorlist { get; set; }
    public string errorcode { get; set; }
    public MotextracoverResponse motextracover { get; set; }

}

public class MotextracoverResponse
{
    public string extrafield2 { get; set; }
}

public class Errorlist1
{
    public string ERR_NUMBER { get; set; }
    public string PAR_NAME { get; set; }
    public string PAR_INDEX { get; set; }
    public string PROPERTY { get; set; }
    public string ERR_TEXT { get; set; }
    public string ERR_LEVEL { get; set; }
}
public class Errorlist
{
    public string errnumber { get; set; }
    public string parname { get; set; }
    public string parindex { get; set; }
    public string property { get; set; }
    public string errtext { get; set; }
    public string errlevel { get; set; }
}