namespace POSP.Domain.POSP
{
    public record GetPanRejectionReasonsModel
    {
        public int Id { get; set; }
        public string RejectionMessage { get; set; }
    }
}
