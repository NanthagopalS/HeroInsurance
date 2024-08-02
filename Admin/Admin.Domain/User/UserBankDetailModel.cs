namespace Admin.Domain.UserBankDetail;
public class UserBankDetailModel
{
    /// <summary>
    /// User Id
    /// </summary>
    public string UserId { get; set; }
    public string Id { get; set; }


    /// <summary>
    /// Bank Id
    /// </summary>
    public string BankId { get; set; }   

    /// <summary>
    /// IFSC Code
    /// </summary>
    public string IFSC { get; set; }

    /// <summary>
    /// AccountHolderName
    /// </summary>
    public string AccountHolderName { get; set; }

    /// <summary>
    /// AccountNumber
    /// </summary>
    public string AccountNumber { get; set; }
    /// <summary>
    /// BankName
    /// </summary>
    public string BankName { get; set; }
}
