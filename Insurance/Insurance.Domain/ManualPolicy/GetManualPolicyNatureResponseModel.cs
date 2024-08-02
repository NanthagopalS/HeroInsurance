namespace Insurance.Domain.ManualPolicy
{
    public record GetManualPolicyNatureResponseModel
	{
        public string  PolicyNatureTypeId{ get; set; }
		public string PolicyNatureTypeName { get; set; }
	}
}
