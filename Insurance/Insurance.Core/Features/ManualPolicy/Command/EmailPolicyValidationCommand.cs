using AutoMapper;
using ClosedXML.Excel;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Models.ManualPolicy;

namespace Insurance.Core.Features.ManualPolicy.Command
{
    public class EmailPolicyValidationCommand : IRequest<HeroResult<bool>>
    {

    }

    public class EmailPolicyValidationCommandHandler : IRequestHandler<EmailPolicyValidationCommand, HeroResult<bool>>
    {
        private readonly IManualPolicyRepository _manualPolicyRepository;
        private readonly ILogger<EmailPolicyValidationCommandHandler> _logger;
        private readonly ICustomUtility _utility;
        private readonly IEmailService _emailService;
        private readonly IApplicationClaims _applicationClaims;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="manualPolicyRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="customUtility"></param>
        /// <param name="emailService"></param>
        /// <param name="applicationClaims"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EmailPolicyValidationCommandHandler(IManualPolicyRepository manualPolicyRepository, IMapper mapper, ILogger<EmailPolicyValidationCommandHandler> logger, ICustomUtility customUtility, IEmailService emailService, IApplicationClaims applicationClaims)
        {
            _manualPolicyRepository = manualPolicyRepository ?? throw new ArgumentNullException(nameof(manualPolicyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utility = customUtility ?? throw new ArgumentNullException(nameof(customUtility));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }

        public async Task<HeroResult<bool>> Handle(EmailPolicyValidationCommand request, CancellationToken cancellationToken)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
            string timeStampNow = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            try
            {
                var responce = await _manualPolicyRepository.GetManualPolicyValidationDetails(cancellationToken);

                var UserEmail = new List<string>();
                var MotorType = new List<string>();
                var PolicyType = new List<string>();
                var PolicyCategory = new List<string>();
                var BasicOD = new List<string>();
                var BasicTP = new List<string>();
                var TotalPremium = new List<string>();
                var NetPremium = new List<string>();
                var ServiceTax = new List<string>();
                var PolicyNo = new List<string>();
                var EngineNo = new List<string>();
                var ChasisNo = new List<string>();
                var VehicleNo = new List<string>();
                var IDV = new List<string>();
                var Insurer = new List<string>();
                var Make = new List<string>();
                var Fuel = new List<string>();
                var Variant = new List<string>();
                var ManufacturingMonth = new List<string>();
                var CustomerName = new List<string>();
                var PolicyIssueDate = new List<string>();
                var PolicyStartDate = new List<string>();
                var PolicyEndDate = new List<string>();
                var BusinessType = new List<string>();
                var NCB = new List<string>();
                var ChequeNo = new List<string>();
                var ChequeDate = new List<string>();
                var ChequeBank = new List<string>();
                var CustomerEmail = new List<string>();
                var CustomerMobile = new List<string>();
                var ManufacturingYear = new List<string>();
                var PreviousNCB = new List<string>();
                var CubicCapacity = new List<string>();
                var RTOCode = new List<string>();
                var PreviousPolicyNo = new List<string>();
                var CPA = new List<string>();
                var Period = new List<string>();
                var InsuranceType = new List<string>();
                var AddOnPremium = new List<string>();
                var NilDep = new List<string>();
                var IsPOSPProduct = new List<string>();
                var CustomerAddress = new List<string>();
                var State = new List<string>();
                var City = new List<string>();
                var PhoneNo = new List<string>();
                var PinCode = new List<string>();
                var CustomerDOB = new List<string>();
                var PANNo = new List<string>();
                var GrossDiscount = new List<string>();
                var TotalTP = new List<string>();
                var GVW = new List<string>();
                var SeatingCapacity = new List<string>();

                var workbook = new XLWorkbook();
                workbook.AddWorksheet("Bulk Upload Format");
                var ws = workbook.Worksheet("Bulk Upload Format");
                ws.ColumnWidth = 60;
                int row = 1;

                foreach (var validationdetails in responce.ManualPolicyDumpRecords)
                {
                    UserEmail.Add(!string.IsNullOrWhiteSpace(validationdetails.UserEmail) ? Convert.ToString(validationdetails.UserEmail) : string.Empty);
                    MotorType.Add(!string.IsNullOrWhiteSpace(validationdetails.MotorType) ? Convert.ToString(validationdetails.MotorType) : string.Empty);
                    PolicyType.Add(!string.IsNullOrWhiteSpace(validationdetails.PolicyType) ? Convert.ToString(validationdetails.PolicyType) : string.Empty);
                    PolicyCategory.Add(!string.IsNullOrWhiteSpace(validationdetails.PolicyCategory) ? Convert.ToString(validationdetails.PolicyCategory) : string.Empty);
                    BasicOD.Add(!string.IsNullOrWhiteSpace(validationdetails.BasicOD) ? Convert.ToString(validationdetails.BasicOD) : string.Empty);
                    BasicTP.Add(!string.IsNullOrWhiteSpace(validationdetails.BasicTP) ? Convert.ToString(validationdetails.BasicTP) : string.Empty);
                    TotalPremium.Add(!string.IsNullOrWhiteSpace(validationdetails.TotalPremium) ? Convert.ToString(validationdetails.TotalPremium) : string.Empty);
                    NetPremium.Add(!string.IsNullOrWhiteSpace(validationdetails.NetPremium) ? Convert.ToString(validationdetails.NetPremium) : string.Empty);
                    ServiceTax.Add(!string.IsNullOrWhiteSpace(validationdetails.ServiceTax) ? Convert.ToString(validationdetails.ServiceTax) : string.Empty);
                    PolicyNo.Add(!string.IsNullOrWhiteSpace(validationdetails.PolicyNo) ? Convert.ToString(validationdetails.PolicyNo) : string.Empty);
                    EngineNo.Add(!string.IsNullOrWhiteSpace(validationdetails.EngineNo) ? Convert.ToString(validationdetails.EngineNo) : string.Empty);
                    ChasisNo.Add(!string.IsNullOrWhiteSpace(validationdetails.ChasisNo) ? Convert.ToString(validationdetails.ChasisNo) : string.Empty);
                    VehicleNo.Add(!string.IsNullOrWhiteSpace(validationdetails.VehicleNo) ? Convert.ToString(validationdetails.VehicleNo) : string.Empty);
                    IDV.Add(!string.IsNullOrWhiteSpace(validationdetails.IDV) ? Convert.ToString(validationdetails.IDV) : string.Empty);
                    Insurer.Add(!string.IsNullOrWhiteSpace(validationdetails.Insurer) ? Convert.ToString(validationdetails.Insurer) : string.Empty);
                    Make.Add(!string.IsNullOrWhiteSpace(validationdetails.Make) ? Convert.ToString(validationdetails.Make) : string.Empty);
                    Fuel.Add(!string.IsNullOrWhiteSpace(validationdetails.Fuel) ? Convert.ToString(validationdetails.Fuel) : string.Empty);
                    Variant.Add(!string.IsNullOrWhiteSpace(validationdetails.Variant) ? Convert.ToString(validationdetails.Variant) : string.Empty);
                    ManufacturingMonth.Add(!string.IsNullOrWhiteSpace(validationdetails.ManufacturingMonth) ? Convert.ToString(validationdetails.ManufacturingMonth) : string.Empty);
                    CustomerName.Add(!string.IsNullOrWhiteSpace(validationdetails.CustomerName) ? Convert.ToString(validationdetails.CustomerName) : string.Empty);
                    PolicyIssueDate.Add(!string.IsNullOrWhiteSpace(validationdetails.PolicyIssueDate) ? Convert.ToString(validationdetails.PolicyIssueDate) : string.Empty);
                    PolicyStartDate.Add(!string.IsNullOrWhiteSpace(validationdetails.PolicyStartDate) ? Convert.ToString(validationdetails.PolicyStartDate) : string.Empty);
                    PolicyEndDate.Add(!string.IsNullOrWhiteSpace(validationdetails.PolicyEndDate) ? Convert.ToString(validationdetails.PolicyEndDate) : string.Empty);
                    BusinessType.Add(!string.IsNullOrWhiteSpace(validationdetails.BusinessType) ? Convert.ToString(validationdetails.BusinessType) : string.Empty);
                    NCB.Add(!string.IsNullOrWhiteSpace(validationdetails.NCB) ? Convert.ToString(validationdetails.NCB) : string.Empty);
                    ChequeNo.Add(!string.IsNullOrWhiteSpace(validationdetails.ChequeNo) ? Convert.ToString(validationdetails.ChequeNo) : string.Empty);
                    ChequeDate.Add(!string.IsNullOrWhiteSpace(validationdetails.ChequeDate) ? Convert.ToString(validationdetails.ChequeDate) : string.Empty);
                    ChequeBank.Add(!string.IsNullOrWhiteSpace(validationdetails.ChequeBank) ? Convert.ToString(validationdetails.ChequeBank) : string.Empty);
                    CustomerEmail.Add(!string.IsNullOrWhiteSpace(validationdetails.CustomerEmail) ? Convert.ToString(validationdetails.CustomerEmail) : string.Empty);
                    CustomerMobile.Add(!string.IsNullOrWhiteSpace(validationdetails.CustomerMobile) ? Convert.ToString(validationdetails.CustomerMobile) : string.Empty);
                    ManufacturingYear.Add(!string.IsNullOrWhiteSpace(validationdetails.ManufacturingYear) ? Convert.ToString(validationdetails.ManufacturingYear) : string.Empty);
                    PreviousNCB.Add(!string.IsNullOrWhiteSpace(validationdetails.PreviousNCB) ? Convert.ToString(validationdetails.PreviousNCB) : string.Empty);
                    CubicCapacity.Add(!string.IsNullOrWhiteSpace(validationdetails.CubicCapacity) ? Convert.ToString(validationdetails.CubicCapacity) : string.Empty);
                    RTOCode.Add(!string.IsNullOrWhiteSpace(validationdetails.RTOCode) ? Convert.ToString(validationdetails.RTOCode) : string.Empty);
                    PreviousPolicyNo.Add(!string.IsNullOrWhiteSpace(validationdetails.PreviousPolicyNo) ? Convert.ToString(validationdetails.PreviousPolicyNo) : string.Empty);
                    CPA.Add(!string.IsNullOrWhiteSpace(validationdetails.CPA) ? Convert.ToString(validationdetails.CPA) : string.Empty);
                    Period.Add(!string.IsNullOrWhiteSpace(validationdetails.Period) ? Convert.ToString(validationdetails.Period) : string.Empty);
                    InsuranceType.Add(!string.IsNullOrWhiteSpace(validationdetails.InsuranceType) ? Convert.ToString(validationdetails.InsuranceType) : string.Empty);
                    AddOnPremium.Add(!string.IsNullOrWhiteSpace(validationdetails.AddOnPremium) ? Convert.ToString(validationdetails.AddOnPremium) : string.Empty);
                    NilDep.Add(!string.IsNullOrWhiteSpace(validationdetails.NilDep) ? Convert.ToString(validationdetails.NilDep) : string.Empty); ;
                    IsPOSPProduct.Add(!string.IsNullOrWhiteSpace(validationdetails.IsPOSPProduct) ? Convert.ToString(validationdetails.IsPOSPProduct) : string.Empty);
                    CustomerAddress.Add(!string.IsNullOrWhiteSpace(validationdetails.CustomerAddress) ? Convert.ToString(validationdetails.CustomerAddress) : string.Empty);
                    State.Add(!string.IsNullOrWhiteSpace(validationdetails.State) ? Convert.ToString(validationdetails.State) : string.Empty);
                    City.Add(!string.IsNullOrWhiteSpace(validationdetails.City) ? Convert.ToString(validationdetails.City) : string.Empty);
                    PhoneNo.Add(!string.IsNullOrWhiteSpace(validationdetails.PhoneNo) ? Convert.ToString(validationdetails.PhoneNo) : string.Empty);
                    PinCode.Add(!string.IsNullOrWhiteSpace(validationdetails.PinCode) ? Convert.ToString(validationdetails.PinCode) : string.Empty);
                    CustomerDOB.Add(!string.IsNullOrWhiteSpace(validationdetails.CustomerDOB) ? Convert.ToString(validationdetails.CustomerDOB) : string.Empty);
                    PANNo.Add(!string.IsNullOrWhiteSpace(validationdetails.PANNo) ? Convert.ToString(validationdetails.PANNo) : string.Empty);
                    GrossDiscount.Add(!string.IsNullOrWhiteSpace(validationdetails.GrossDiscount) ? Convert.ToString(validationdetails.GrossDiscount) : string.Empty);
                    TotalTP.Add(!string.IsNullOrWhiteSpace(validationdetails.TotalTP) ? Convert.ToString(validationdetails.TotalTP) : string.Empty);
                    GVW.Add(!string.IsNullOrWhiteSpace(validationdetails.GVW) ? Convert.ToString(validationdetails.GVW) : string.Empty);
                    SeatingCapacity.Add(!string.IsNullOrWhiteSpace(validationdetails.SeatingCapacity) ? Convert.ToString(validationdetails.SeatingCapacity) : string.Empty);
                }

                ws.Cell(row, 1).InsertTable(UserEmail);
                ws.Cell(row, 2).InsertTable(MotorType);
                ws.Cell(row, 3).InsertTable(PolicyType);
                ws.Cell(row, 4).InsertTable(PolicyCategory);
                ws.Cell(row, 5).InsertTable(BasicOD);
                ws.Cell(row, 6).InsertTable(BasicTP);
                ws.Cell(row, 7).InsertTable(TotalPremium);
                ws.Cell(row, 8).InsertTable(NetPremium);
                ws.Cell(row, 9).InsertTable(ServiceTax);
                ws.Cell(row, 10).InsertTable(PolicyNo);
                ws.Cell(row, 11).InsertTable(EngineNo);
                ws.Cell(row, 12).InsertTable(ChasisNo);
                ws.Cell(row, 13).InsertTable(VehicleNo);
                ws.Cell(row, 14).InsertTable(IDV);
                ws.Cell(row, 15).InsertTable(Insurer);
                ws.Cell(row, 16).InsertTable(Make);
                ws.Cell(row, 17).InsertTable(Fuel);
                ws.Cell(row, 18).InsertTable(Variant);
                ws.Cell(row, 19).InsertTable(ManufacturingMonth);
                ws.Cell(row, 20).InsertTable(CustomerName);
                ws.Cell(row, 21).InsertTable(PolicyIssueDate);
                ws.Cell(row, 22).InsertTable(PolicyStartDate);
                ws.Cell(row, 23).InsertTable(PolicyEndDate);
                ws.Cell(row, 24).InsertTable(BusinessType);
                ws.Cell(row, 25).InsertTable(NCB);
                ws.Cell(row, 26).InsertTable(ChequeNo);
                ws.Cell(row, 27).InsertTable(ChequeDate);
                ws.Cell(row, 28).InsertTable(ChequeBank);
                ws.Cell(row, 29).InsertTable(CustomerEmail);
                ws.Cell(row, 30).InsertTable(CustomerMobile);
                ws.Cell(row, 31).InsertTable(ManufacturingYear);
                ws.Cell(row, 32).InsertTable(PreviousNCB);
                ws.Cell(row, 33).InsertTable(CubicCapacity);
                ws.Cell(row, 34).InsertTable(RTOCode);
                ws.Cell(row, 35).InsertTable(PreviousPolicyNo);
                ws.Cell(row, 36).InsertTable(CPA);
                ws.Cell(row, 37).InsertTable(Period);
                ws.Cell(row, 38).InsertTable(InsuranceType);
                ws.Cell(row, 39).InsertTable(AddOnPremium);
                ws.Cell(row, 40).InsertTable(NilDep);
                ws.Cell(row, 41).InsertTable(IsPOSPProduct);
                ws.Cell(row, 42).InsertTable(CustomerAddress);
                ws.Cell(row, 43).InsertTable(State);
                ws.Cell(row, 44).InsertTable(City);
                ws.Cell(row, 45).InsertTable(PhoneNo);
                ws.Cell(row, 46).InsertTable(PinCode);
                ws.Cell(row, 47).InsertTable(CustomerDOB);
                ws.Cell(row, 48).InsertTable(PANNo);
                ws.Cell(row, 49).InsertTable(GrossDiscount);
                ws.Cell(row, 50).InsertTable(TotalTP);
                ws.Cell(row, 51).InsertTable(GVW);
                ws.Cell(row, 52).InsertTable(SeatingCapacity);

                ws.Cell(row, 1).SetValue("UserEmail");
                ws.Cell(row, 2).SetValue("MotorType");
                ws.Cell(row, 3).SetValue("PolicyType");
                ws.Cell(row, 4).SetValue("PolicyCategory");
                ws.Cell(row, 5).SetValue("BasicOD");
                ws.Cell(row, 6).SetValue("BasicTP");
                ws.Cell(row, 7).SetValue("TotalPremium");
                ws.Cell(row, 8).SetValue("NetPremium");
                ws.Cell(row, 9).SetValue("ServiceTax");
                ws.Cell(row, 10).SetValue("PolicyNo");
                ws.Cell(row, 11).SetValue("EngineNo");
                ws.Cell(row, 12).SetValue("ChasisNo");
                ws.Cell(row, 13).SetValue("VehicleNo");
                ws.Cell(row, 14).SetValue("IDV");
                ws.Cell(row, 15).SetValue("Insurer");
                ws.Cell(row, 16).SetValue("Make");
                ws.Cell(row, 17).SetValue("Fuel");
                ws.Cell(row, 18).SetValue("Variant");
                ws.Cell(row, 19).SetValue("ManufacturingMonth");
                ws.Cell(row, 20).SetValue("CustomerName");
                ws.Cell(row, 21).SetValue("PolicyIssueDate");
                ws.Cell(row, 22).SetValue("PolicyStartDate");
                ws.Cell(row, 23).SetValue("PolicyEndDate");
                ws.Cell(row, 24).SetValue("BusinessType");
                ws.Cell(row, 25).SetValue("NCB");
                ws.Cell(row, 26).SetValue("ChequeNo");
                ws.Cell(row, 27).SetValue("ChequeDate");
                ws.Cell(row, 28).SetValue("ChequeBank");
                ws.Cell(row, 29).SetValue("CustomerEmail");
                ws.Cell(row, 30).SetValue("CustomerMobile");
                ws.Cell(row, 31).SetValue("ManufacturingYear");
                ws.Cell(row, 32).SetValue("PreviousNCB");
                ws.Cell(row, 33).SetValue("CubicCapacity");
                ws.Cell(row, 34).SetValue("RTOCode");
                ws.Cell(row, 35).SetValue("PreviousPolicyNo");
                ws.Cell(row, 36).SetValue("CPA");
                ws.Cell(row, 37).SetValue("Period");
                ws.Cell(row, 38).SetValue("InsuranceType");
                ws.Cell(row, 39).SetValue("AddOnPremium");
                ws.Cell(row, 40).SetValue("NilDep");
                ws.Cell(row, 41).SetValue("IsPOSPProduct");
                ws.Cell(row, 42).SetValue("CustomerAddress");
                ws.Cell(row, 43).SetValue("State");
                ws.Cell(row, 44).SetValue("City");
                ws.Cell(row, 45).SetValue("PhoneNo");
                ws.Cell(row, 46).SetValue("PinCode");
                ws.Cell(row, 47).SetValue("CustomerDOB");
                ws.Cell(row, 48).SetValue("PANNo");
                ws.Cell(row, 49).SetValue("GrossDiscount");
                ws.Cell(row, 50).SetValue("TotalTP");
                ws.Cell(row, 51).SetValue("GVW");
                ws.Cell(row, 52).SetValue("SeatingCapacity");

                workbook.AddWorksheet("Status Report");
                ws = workbook.Worksheet("Status Report");
                ws.ColumnWidth = 60;

                var PolicyNumber = new List<string>();
                var UploadStatus = new List<string>();
                var Date = new List<string>();

                foreach (var dumpdetails in responce.ManualPolicyDumpErrors)
                {
                    PolicyNumber.Add(!string.IsNullOrWhiteSpace(dumpdetails.PolicyId) ? Convert.ToString(dumpdetails.PolicyId) : string.Empty);
                    UploadStatus.Add(!string.IsNullOrWhiteSpace(dumpdetails.ErrorLog) ? Convert.ToString(dumpdetails.ErrorLog) : string.Empty);
                    Date.Add(!string.IsNullOrWhiteSpace(dumpdetails.CreatedOn) ? Convert.ToString(dumpdetails.CreatedOn) : string.Empty);
                }

                ws.Cell(row, 1).InsertTable(PolicyNumber);
                ws.Cell(row, 2).InsertTable(UploadStatus);
                ws.Cell(row, 3).InsertTable(Date);
                ws.Cell(row, 1).SetValue("Policy Number ");
                ws.Cell(row, 2).SetValue("Upload Status");
                ws.Cell(row, 3).SetValue("Date (Date and Time)");


                workbook.SaveAs(Path.Combine(path, "ManualPolicyUpload" + timeStampNow + ".xlsx"), new ClosedXML.Excel.SaveOptions());

                List<Attachment> attachments = new List<Attachment>();
                attachments.Add(new Attachment(path + "/ManualPolicyUpload" + timeStampNow + ".xlsx"));

                ManualPolicyEmailRequest manualPolicyEmailRequest = new ManualPolicyEmailRequest()
                {
                    dateAndTime = responce.ManualPolicyDumpErrors.Count()>0?responce.ManualPolicyDumpErrors.First().CreatedOn: DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    EmailId = responce.manualPolicyCounts.EmailId,
                    policyfailed = responce.manualPolicyCounts.policyfailed,
                    policyuploadedsuccessful = responce.manualPolicyCounts.policyuploadedsuccessful,
                    totalpolicy = responce.manualPolicyCounts.totalpolicy,
                    UserName = responce.manualPolicyCounts.UserName
                };
                await _emailService.SendManualPolicyReport(manualPolicyEmailRequest, attachments, cancellationToken);

                return HeroResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send Email Validation Mail - " + ex.Message);
            }
            return HeroResult<bool>.Success(false);
        }
    }
}

