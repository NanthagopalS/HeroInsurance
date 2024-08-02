namespace Insurance.Domain.IFFCO;

public class IFFCOPreviousPolicyDetailsModel
{
	public string FinancierCode { get; set; }
	public string Zone { get; set; }
	public string TPStartDate { get; set; }
	public string TPEndDate { get; set; }
	public string ODStartDate { get; set; }
	public string ODEndDate { get; set; }
	public string TPPreviousInsurer { get; set; }
	public string ODPreviousInsurer { get; set; }
	public string TPPreviouspolicyNumber { get; set; }
	public string ODPreviouspolicyNumber { get; set; }
	public string PolictTypeId { get; set; }
}

