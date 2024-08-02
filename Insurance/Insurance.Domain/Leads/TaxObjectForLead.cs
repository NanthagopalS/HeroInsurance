namespace Insurance.Domain.Leads
{
    public record TaxObjectForLead
    {
        public string cgst { get; set; }
        public string sgst { get; set; }
        public string igst { get; set; }
        public string utgst { get; set; }
        public string totalTax { get; set; }
        public string taxType { get; set; }
    }
}
