namespace Insurance.API.Models
{
    public class CreatePOSPRequestModel
    {
        public string PospId { get; set; }
        public string AadharNumber { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string PanNumber { get; set; }
        public string State { get; set; }
    }
}
