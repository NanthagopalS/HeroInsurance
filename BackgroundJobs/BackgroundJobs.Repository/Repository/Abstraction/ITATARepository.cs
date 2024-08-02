namespace BackgroundJobs.Repository.Repository.Abstraction;

    public interface ITATARepository
    {
		Task GetBreakInStatus();

        Task GetPaymentStatus();
}

