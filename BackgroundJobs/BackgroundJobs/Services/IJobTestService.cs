namespace BackgroundJobs.Services;

public interface IJobTestService
{
    void ContinuationJob();
    void DelayedJob();
    void FireAndForgetJob();
    void ReccuringJob();
}