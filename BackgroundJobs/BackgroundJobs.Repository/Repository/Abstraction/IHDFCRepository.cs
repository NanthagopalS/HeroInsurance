namespace BackgroundJobs.Repository.Repository.Abstraction;

public interface IHDFCRepository
{
    Task GetCKYCStatus();
    Task CreatePOSP();
}