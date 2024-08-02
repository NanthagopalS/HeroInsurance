namespace Insurance.Domain.Leads
{
    public record PaymentStatusListResponceModel
    {
        public string PaymentId { get; set; }
        public string PaymentStatus { get; set; }
    }
}
