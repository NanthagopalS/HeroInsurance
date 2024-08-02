using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.ICICI.Command.Payment;

public class PaymentCommand : IRequest<HeroResult<PaymentDetailsVm>>
{
    public string TransactionId { get; set; }
    public string Amount { get; set; }
    public string MerchantId { get; set; }
    public string GatewayId { get; set; }
    public string GatewayName { get; set; }
    public string Success { get; set; }
    public string AdditionalInfo1 { get; set; }
    public string AdditionalInfo2 { get; set; }
    public string AdditionalInfo3 { get; set; }
    public string AdditionalInfo4 { get; set; }
    public string AdditionalInfo5 { get; set; }
    public string AdditionalInfo6 { get; set; }
    public string AdditionalInfo7 { get; set; }
    public string AdditionalInfo8 { get; set; }
    public string AdditionalInfo9 { get; set; }
    public string AdditionalInfo10 { get; set; }
    public string AdditionalInfo11 { get; set; }
    public string AdditionalInfo12 { get; set; }
    public string AdditionalInfo13 { get; set; }
    public string AdditionalInfo14 { get; set; }
    public string AdditionalInfo15 { get; set; }
    public string GatewayErrorCode { get; set; }
    public string GatewayErrorText { get; set; }
    public string PGIMasterErrorCode { get; set; }
    public string PGIUserErrorCode { get; set; }
    public string AuthCode { get; set; }
    public string PGTransactionId { get; set; }
    public string PGTransactionDate { get; set; }
    public string PGPaymentId { get; set; }
    public string OrderId { get; set; }
    public string CorrelationId { get; set; }
}
