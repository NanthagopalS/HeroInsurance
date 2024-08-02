using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote;

public class PaymentCKCYResponseModel
{
    public int InsurerStatusCode { get; set; }
    public string PaymentTransactionId { get; set; }
    public string InsurerId { get; set; }
    public string QuoteTransactionId { get; set; }
    public string ApplicationId { get; set; }
    public string LeadId { get; set; }
    public string LeadName { get; set; }
    public string ProposalNumber { get; set; }
    public string PaymentTransactionNumber { get; set; }
    public string Amount { get; set; }
    public string PaymentStatus { get; set; }
    public string CKYCStatus { get; set; }
    public string CKYCLink { get; set; }
    public string CKYCFailedReason { get; set; }
    public string PolicyDocumentLink { get; set; }
    public string DocumentId { get; set; }
    public string PolicyType { get; set; }
    public string Logo { get; set; }
    public string PolicyNumber { get; set; }
    public string CustomerId { get; set; }
    public bool IsTP { get; set; }
    public string BankName { get; set; }
    public string PaymentDate { get; set; }
    public string RequestBody { get; set; }
    public string UserId { get; set; }
}

