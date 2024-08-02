namespace Insurance.Domain.Quote;

public class LogsModel
{
    public string InsurerId { get; set; }
    public string Headers { get; set; }
    public string RequestBody { get; set; }
    public string Token { get; set; }
    public string API { get; set; }
    public string ResponseBody { get; set; }
    public DateTime ResponseTime { get; set; }
    public int Id { get; set; }
    public string UserId { get; set; }
    public string LeadId { get; set; }
    public string ApplicationId { get; set; }
    public string Stage { get; set; }
}
