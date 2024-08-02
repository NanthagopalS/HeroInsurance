using Insurance.Domain.GoDigit;
using System.Text.Json.Serialization;

namespace Insurance.Domain.Quote
{
    public class ProposalFieldModel
    {
        public string Section { get; set; }
        public string FieldKey { get; set; }
        public string FieldText { get; set; }
        public string FieldType { get; set; }
        public bool IsMandatory { get; set; }
        public string Validation { get; set; }
        public bool IsMaster { get; set; }
        public string MasterRef { get; set; }
        public string ColumnRef { get; set; }
        public string DefaultValue { get; set; }
    }

    public class ProposalFieldMasterModel
    {
        public string Section { get; set; }
        public string FieldKey { get; set; }
        public string FieldText { get; set; }
        public string FieldType { get; set; }
        public bool IsMandatory { get; set; }
        public List<NameValueModel> Validation { get; set; }
        public bool IsMaster { get; set; }
        public IEnumerable<NameValueModel> MasterData { get; set; }
        [JsonIgnore]
        public string MasterRef { get; set; }
        [JsonIgnore]
        public string ColumnRef { get; set; }
        public string DefaultValue { get; set; }
        public string AcceptedFormat { get; set; }
    }

    public class CKYCStatusModel
    {
        public string InsurerId { get; set; }
        public string InsurerName { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string KYCStatus { get; set; }
        public string KYCID { get; set; }
        public string KYCNumber { get; set; }
        public string Logo { get; set; }

    }
}
