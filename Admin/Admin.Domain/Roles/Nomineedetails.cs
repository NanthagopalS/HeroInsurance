namespace Admin.Domain.Roles;

public class ProposaDetails
{
    public NomineeDetails nomineeDetails { get; set; }
}
public class NomineeDetails
{
    public string nomineeName { get; set; }
    public string nomineeFirstName { get; set; }
    public string nomineeLastName { get; set; }
    public string nomineeAge { get; set; }
    public string nomineeRelation { get; set; }
}
