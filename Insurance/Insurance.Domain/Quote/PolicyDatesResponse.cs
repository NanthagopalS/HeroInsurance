using System.Globalization;

namespace Insurance.Domain.Quote
{
    public class PolicyDatesResponse
    {
        public string PolicyStartDate { get; set; }
        public string PolicyEndDate { get; set; }
        public string ODPolicyStartDate { get; set; }
        public string ODPolicyEndDate { get; set; }
        public string TPPolicyStartDate { get; set; }
        public string TPPolicyEndDate { get; set; }
        public string PreviousPolicyTypeId { get; set; }
        public string RegistrationDate { get; set; }
        public int VehicleTPTenure { get; set; }
        public int VehicleODTenure { get; set; }
        public string ManufacturingDate { get; set; }
        public bool IsTwoWheeler { get; set; }
        public bool IsFourWheeler { get; set; }
        public bool IsDefaultPACoverRequired { get; set; }
        public bool IsCommercial { get; set; }
    }
}
