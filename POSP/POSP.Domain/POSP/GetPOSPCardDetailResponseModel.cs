namespace POSP.Domain.POSP
{
    public record GetPOSPCardDetailResponseModel
    {
        public string POSP_Id { get; set; }
        public string GWP { get; set; }
        public string Category { get; set; }
        public int Result { get; set; }
    }
}
