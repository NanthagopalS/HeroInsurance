namespace POSP.Domain.POSP
{
    public record GetPOSPTestimonialsResponseModel
	{
		public string Name { get; set; }
		public string POSP_Id { get; set; }
		public string image { get; set; }
		public string feedback { get; set; }
		public float starcount { get; set; }
		public string imageBase64 { get; set; }
	}
}
