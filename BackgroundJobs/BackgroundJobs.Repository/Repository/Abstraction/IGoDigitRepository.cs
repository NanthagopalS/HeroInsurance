namespace BackgroundJobs.Repository.Repository.Abstraction;

public interface IGoDigitRepository
{
    Task GetPolicyStatus();
    Task GetCKYCPaymentStatus();
}