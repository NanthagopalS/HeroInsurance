namespace BackgroundJobs.Repository.Repository.Abstraction;

public interface IICICIRepository
{
    Task GetPolicyStatus();
    Task GetPaymentStatus();
    Task CreateIMBroker();
}