namespace BackgroundJobs.Repository.Repository.Abstraction;

public interface ICholaRepository
{
    Task GetCKYCStatus();
    Task GetBreakInStatus();

    Task GetPaymentStatus();
}

