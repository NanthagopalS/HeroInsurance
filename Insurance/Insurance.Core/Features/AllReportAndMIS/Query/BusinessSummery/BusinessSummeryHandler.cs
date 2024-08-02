using AutoMapper;
using ClosedXML.Excel;
using CsvHelper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.AllReportAndMIS.Query.BusinessSummery
{
    /// <summary>
    /// Query object for business summery report query
    /// searchtext - free text
    /// startdate - enddate
    /// insurers ID's
    /// InsuranceType -- insurance nature
    /// page index and size
    /// IsExportExcel -- bool
    /// </summary>
    public record BusunessSummeryQuery : IRequest<HeroResult<BusinessSummeryVm>>
    {
        /// <summary>
        /// SearchText free text
        /// startDate date
        /// endData date
        /// insuranceType ID
        /// currectPageIndex int
        /// currentPageSize int
        /// isExportExcel bool
        /// </summary>
        public string SearchText { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Insurers { get; set; }
        public string InsuranceType { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public bool IsExportExcel { get; set; } = false;

    }
    public class BusinessSummeryHandler : IRequestHandler<BusunessSummeryQuery, HeroResult<BusinessSummeryVm>>
    {
        public readonly IInsuranceReportRepository _insuranceRepo;
        private readonly IMapper _mapper;
        private readonly ICustomUtility _customUtility;
        private readonly ILogger<BusinessSummeryHandler> _logger;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="insuranceRepo"></param>
        /// <param name="mapper"></param>
        /// <param name="customUtility"></param>
        public BusinessSummeryHandler(IInsuranceReportRepository insuranceRepo, IMapper mapper, ICustomUtility customUtility, ILogger<BusinessSummeryHandler> logger)
        {
            _insuranceRepo = insuranceRepo ?? throw new ArgumentNullException(nameof(insuranceRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _customUtility = customUtility ?? throw new ArgumentNullException(nameof(customUtility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// handle business summery report request
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<BusinessSummeryVm>> Handle(BusunessSummeryQuery query, CancellationToken cToken)
        {
            DateTime fromDate = _customUtility.ConvertToDateTime(query.StartDate);
            DateTime toDate = _customUtility.ConvertToDateTime(query.EndDate);
            int DateDifference = DateTimeExtensions.Compare(fromDate, toDate, DateTimeInterval.Days);
            if (DateDifference > 60 || DateDifference < 0)
            {
                return HeroResult<BusinessSummeryVm>.Fail("Date Range Not Valid");
            }
            var result = await _insuranceRepo.BusinessSummeryReport(query, cToken);
            if (result is not null)
            {
                var listReport = _mapper.Map<BusinessSummeryVm>(result);
                string timeStampNow = null;
                if (query.IsExportExcel == true && listReport is not null && listReport.BusinessSummeryRecords.Count() > 0)
                {

                    timeStampNow = "Business Summery - " + DateTime.Now.ToFileTime().ToString();
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    // Delete Old Generated Files
                    string[] OldFiles = Directory.GetFiles(path);
                    foreach (string oldTempFiles in OldFiles)
                    {
                        string[] resultArray = oldTempFiles.Split(".");
                        if (resultArray.Length > 0 && (resultArray[resultArray.Length - 1].ToLower() == "xlsx" || resultArray[resultArray.Length - 1].ToLower() == "csv"))
                        {
                            DateTime creationTime = System.IO.File.GetCreationTimeUtc(oldTempFiles);
                            TimeSpan timeDiff = DateTime.Now - creationTime;
                            if (timeDiff.TotalMinutes > 3600)
                            {
                                try
                                {
                                    System.IO.File.Delete(oldTempFiles);
                                }
                                catch (Exception exC)
                                {
                                    _logger.LogInformation("Error In Deleting Temp File:-", oldTempFiles.ToString());
                                }
                            }
                        }
                    }

                    var Sno = new List<string>();
                    var BrokerBranchCode = new List<string>();
                    var BrokerBranch = new List<string>();
                    var CustName = new List<string>();
                    var Address = new List<string>();
                    var City = new List<string>();
                    var State = new List<string>();
                    var Country = new List<string>();
                    var Pin = new List<string>();
                    var PhoneNo = new List<string>();
                    var MobileNo = new List<string>();
                    var DOB = new List<string>();
                    var Fax = new List<string>();
                    var Email = new List<string>();
                    var PANNo = new List<string>();
                    var RMCode = new List<string>();
                    var RMName = new List<string>();
                    var ReportingEmail = new List<string>();
                    var ReferCode = new List<string>();
                    var POSPCode = new List<string>();
                    var POSPName = new List<string>();
                    var CSCCode = new List<string>();
                    var Ref = new List<string>();
                    var Insurer = new List<string>();
                    var InsurerBranchAutoCode = new List<string>();
                    var PolicyType = new List<string>();
                    var OD = new List<string>();
                    var TP = new List<string>();
                    var VehicleRegnStatus = new List<string>();
                    var VehicleNo = new List<string>();
                    var Make = new List<string>();
                    var Variant = new List<string>();
                    var YearofManf = new List<string>();
                    var ChassisNo = new List<string>();
                    var EngineNo = new List<string>();
                    var CubicCapicaty = new List<string>();
                    var Fuel = new List<string>();
                    var RTOCode = new List<string>();
                    var RTOName = new List<string>();
                    var CurrentNCB = new List<string>();
                    var PreviousNCB = new List<string>();
                    var ODD = new List<string>();
                    var PCV = new List<string>();
                    var Passenger = new List<string>();
                    var BusPropDate = new List<string>();
                    var OD_start = new List<string>();
                    var OD_end = new List<string>();
                    var TP_start = new List<string>();
                    var TP_end = new List<string>();
                    var CoverNoteNo = new List<string>();
                    var PolicyNo = new List<string>();
                    var PolicyIssueDate = new List<string>();
                    var PolicyReceiveDate = new List<string>();
                    var PolRecvdFormat = new List<string>();
                    var BizType = new List<string>();
                    var SumInsured = new List<string>();
                    var ODNetPremium = new List<string>();
                    var TpPrem = new List<string>();
                    var TotalTp = new List<string>();
                    var NetPremium = new List<string>();
                    var GST = new List<string>();
                    var GrossPremium = new List<string>();
                    var BankName = new List<string>();
                    var POS = new List<string>();
                    var TPPOS = new List<string>();
                    var PolicyStatus = new List<string>();
                    var PrevPolicyNO = new List<string>();
                    var UserName = new List<string>();
                    var OrderNumber = new List<string>();
                    var Uploadtime = new List<string>();
                    var Product = new List<string>();
                    var POSP = new List<string>();
                    var SeatingCapacity = new List<string>();
                    var CPA = new List<string>();
                    var Discount = new List<string>();
                    var NillDep = new List<string>();
                    var GVW = new List<string>();
                    var MotorType = new List<string>();
                    var BusinessFrom = new List<string>();
                    var UserId = new List<string>();

                    var workbook = new XLWorkbook();
                    workbook.AddWorksheet("Business Summery");
                    var ws = workbook.Worksheet("Business Summery");
                    ws.ColumnWidth = 60;
                    int row = 1;
                    int CountSno = 0;
                    foreach (var pospDetail in listReport.BusinessSummeryRecords)
                    {
                        ++CountSno;
                        Sno.Add(CountSno.ToString());
                        pospDetail.Sno = CountSno.ToString();
                        BrokerBranchCode.Add("--");
                        pospDetail.BrokerBranchCode = "--";
                        BrokerBranch.Add("--");
                        pospDetail.BrokerBranch = "--";
                        CustName.Add(!string.IsNullOrWhiteSpace(pospDetail.UserName) ? Convert.ToString(pospDetail.UserName) : string.Empty);
                        pospDetail.CustName = !string.IsNullOrWhiteSpace(pospDetail.UserName) ? Convert.ToString(pospDetail.UserName) : string.Empty;
                        pospDetail.Address = (!string.IsNullOrWhiteSpace(pospDetail.Address1) ? pospDetail.Address1 : String.Empty) +
                            (!string.IsNullOrWhiteSpace(pospDetail.Address2) ? pospDetail.Address2 : String.Empty) +
                            (!string.IsNullOrWhiteSpace(pospDetail.Address3) ? pospDetail.Address3 : String.Empty);
                        Address.Add(pospDetail.Address);
                        pospDetail.City = !string.IsNullOrWhiteSpace(pospDetail.City) ? Convert.ToString(pospDetail.City) : string.Empty;
                        City.Add(pospDetail.City);
                        pospDetail.AState = !string.IsNullOrWhiteSpace(pospDetail.AState) ? Convert.ToString(pospDetail.AState) : string.Empty;
                        State.Add(pospDetail.AState);
                        Country.Add(!string.IsNullOrWhiteSpace(pospDetail.Country) ? Convert.ToString(pospDetail.Country) : string.Empty);
                        Pin.Add(!string.IsNullOrWhiteSpace(pospDetail.Pincode) ? Convert.ToString(pospDetail.Pincode) : string.Empty);
                        PhoneNo.Add(!string.IsNullOrWhiteSpace(pospDetail.MobileNo) ? Convert.ToString(pospDetail.MobileNo) : string.Empty);
                        MobileNo.Add(!string.IsNullOrWhiteSpace(pospDetail.MobileNo) ? Convert.ToString(pospDetail.MobileNo) : string.Empty);
                        DOB.Add(!string.IsNullOrWhiteSpace(pospDetail.DOB) ? Convert.ToString(pospDetail.DOB) : string.Empty);
                        Fax.Add("");
                        Email.Add(!string.IsNullOrWhiteSpace(pospDetail.EmailId) ? Convert.ToString(pospDetail.EmailId) : string.Empty);
                        PANNo.Add(!string.IsNullOrWhiteSpace(pospDetail.PANNumber) ? Convert.ToString(pospDetail.PANNumber) : string.Empty);
                        RMCode.Add(!string.IsNullOrWhiteSpace(pospDetail.RMCode) ? Convert.ToString(pospDetail.RMCode) : string.Empty);
                        RMName.Add(!string.IsNullOrWhiteSpace(pospDetail.RMNAme) ? Convert.ToString(pospDetail.RMNAme) : string.Empty);
                        ReportingEmail.Add(!string.IsNullOrWhiteSpace(pospDetail.ReportingEmail) ? Convert.ToString(pospDetail.ReportingEmail) : string.Empty);
                        POSPCode.Add(!string.IsNullOrWhiteSpace(pospDetail.POSPCode) ? Convert.ToString(pospDetail.POSPCode) : string.Empty);
                        POSPName.Add(!string.IsNullOrWhiteSpace(pospDetail.POSPName) ? Convert.ToString(pospDetail.POSPName) : string.Empty);
                        CSCCode.Add("--");
                        Ref.Add(!string.IsNullOrWhiteSpace(pospDetail.ReferalCode) ? Convert.ToString(pospDetail.ReferalCode) : string.Empty);
                        Insurer.Add(!string.IsNullOrWhiteSpace(pospDetail.InsurerName) ? Convert.ToString(pospDetail.InsurerName) : string.Empty);
                        InsurerBranchAutoCode.Add("--");
                        PolicyType.Add(!string.IsNullOrWhiteSpace(pospDetail.PolicyType) ? Convert.ToString(pospDetail.PolicyType) : string.Empty);
                        OD.Add(!string.IsNullOrWhiteSpace(pospDetail.OD_Coverage) ? Convert.ToString(pospDetail.OD_Coverage) : string.Empty);
                        TP.Add(!string.IsNullOrWhiteSpace(pospDetail.TP_Coverage) ? Convert.ToString(pospDetail.TP_Coverage) : string.Empty);
                        VehicleRegnStatus.Add(!string.IsNullOrWhiteSpace(pospDetail.VehicleRegnStatus) ? Convert.ToString(pospDetail.VehicleRegnStatus) : string.Empty);
                        VehicleNo.Add(!string.IsNullOrWhiteSpace(pospDetail.RegNo) ? Convert.ToString(pospDetail.RegNo) : string.Empty);
                        Make.Add(!string.IsNullOrWhiteSpace(pospDetail.VehicleManufacturerName) ? Convert.ToString(pospDetail.VehicleManufacturerName) : string.Empty);
                        Variant.Add(!string.IsNullOrWhiteSpace(pospDetail.Variant) ? Convert.ToString(pospDetail.Variant) : string.Empty);
                        YearofManf.Add(!string.IsNullOrWhiteSpace(pospDetail.YearofManf) ? Convert.ToString(pospDetail.YearofManf) : string.Empty);
                        ChassisNo.Add(!string.IsNullOrWhiteSpace(pospDetail.ChassisNo) ? Convert.ToString(pospDetail.ChassisNo) : string.Empty);
                        EngineNo.Add(!string.IsNullOrWhiteSpace(pospDetail.EngineNo) ? Convert.ToString(pospDetail.EngineNo) : string.Empty);
                        CubicCapicaty.Add(!string.IsNullOrWhiteSpace(pospDetail.CubicCapacity) ? Convert.ToString(pospDetail.CubicCapacity) : string.Empty);
                        Fuel.Add(!string.IsNullOrWhiteSpace(pospDetail.Fuel) ? Convert.ToString(pospDetail.Fuel) : string.Empty);
                        RTOCode.Add(!string.IsNullOrWhiteSpace(pospDetail.RTOCode) ? Convert.ToString(pospDetail.RTOCode) : string.Empty);
                        RTOName.Add(!string.IsNullOrWhiteSpace(pospDetail.RTOCode) ? Convert.ToString(pospDetail.RTOName) : string.Empty);
                        CurrentNCB.Add(!string.IsNullOrWhiteSpace(pospDetail.CurrentNCB) ? Convert.ToString(pospDetail.CurrentNCB) : string.Empty);
                        PreviousNCB.Add(!string.IsNullOrWhiteSpace(pospDetail.PreviousNCB) ? Convert.ToString(pospDetail.PreviousNCB) : string.Empty);
                        ODD.Add(!string.IsNullOrWhiteSpace(pospDetail.ODD) ? Convert.ToString(pospDetail.ODD) : string.Empty);
                        PCV.Add("--");
                        Passenger.Add("--");
                        BusPropDate.Add("--");
                        OD_start.Add(!string.IsNullOrWhiteSpace(pospDetail.OD_PolicyStartDate) ? Convert.ToString(pospDetail.OD_PolicyStartDate) : string.Empty);
                        OD_end.Add(!string.IsNullOrWhiteSpace(pospDetail.OD_PolicyEndDate) ? Convert.ToString(pospDetail.OD_PolicyEndDate) : string.Empty);
                        TP_start.Add(!string.IsNullOrWhiteSpace(pospDetail.TP_PolicyStartDate) ? Convert.ToString(pospDetail.TP_PolicyStartDate) : string.Empty);
                        TP_end.Add(!string.IsNullOrWhiteSpace(pospDetail.TP_PolicyEndDate) ? Convert.ToString(pospDetail.TP_PolicyEndDate) : string.Empty);
                        CoverNoteNo.Add("--");
                        PolicyNo.Add(!string.IsNullOrWhiteSpace(pospDetail.PolicyNo) ? Convert.ToString(pospDetail.PolicyNo) : string.Empty);
                        PolicyIssueDate.Add(!string.IsNullOrWhiteSpace(pospDetail.PolicyIssueDate) ? Convert.ToString(pospDetail.PolicyIssueDate) : string.Empty);
                        PolicyReceiveDate.Add(!string.IsNullOrWhiteSpace(pospDetail.PolicyReceiveDate) ? Convert.ToString(pospDetail.PolicyReceiveDate) : string.Empty);
                        PolRecvdFormat.Add("--");
                        BizType.Add(!string.IsNullOrWhiteSpace(pospDetail.BizType) ? Convert.ToString(pospDetail.BizType) : string.Empty);
                        SumInsured.Add(!string.IsNullOrWhiteSpace(pospDetail.SumInsured) ? Convert.ToString(pospDetail.SumInsured) : string.Empty);
                        ODNetPremium.Add(!string.IsNullOrWhiteSpace(pospDetail.ODNetPremium) ? Convert.ToString(pospDetail.ODNetPremium) : string.Empty);
                        TpPrem.Add(!string.IsNullOrWhiteSpace(pospDetail.TpPrem) ? Convert.ToString(pospDetail.TpPrem) : string.Empty);
                        TotalTp.Add(!string.IsNullOrWhiteSpace(pospDetail.TotalTp) ? Convert.ToString(pospDetail.TotalTp) : string.Empty);
                        NetPremium.Add(!string.IsNullOrWhiteSpace(pospDetail.NetPremium) ? Convert.ToString(pospDetail.NetPremium) : string.Empty);
                        GST.Add(!string.IsNullOrWhiteSpace(pospDetail.GST) ? Convert.ToString(pospDetail.GST) : string.Empty);
                        GrossPremium.Add(!string.IsNullOrWhiteSpace(pospDetail.GrossPremium) ? Convert.ToString(pospDetail.GrossPremium) : string.Empty);
                        BankName.Add("--");
                        POS.Add("--");
                        TPPOS.Add("--");
                        PolicyStatus.Add(!string.IsNullOrWhiteSpace(pospDetail.PolicyStatus) ? Convert.ToString(pospDetail.PolicyStatus) : string.Empty);
                        PrevPolicyNO.Add(!string.IsNullOrWhiteSpace(pospDetail.PrevPolicyNO) ? Convert.ToString(pospDetail.PrevPolicyNO) : string.Empty);
                        UserName.Add(!string.IsNullOrWhiteSpace(pospDetail.UserName) ? Convert.ToString(pospDetail.UserName) : string.Empty);
                        OrderNumber.Add(!string.IsNullOrWhiteSpace(pospDetail.OrderNumber) ? Convert.ToString(pospDetail.OrderNumber) : string.Empty);
                        Uploadtime.Add(!string.IsNullOrWhiteSpace(pospDetail.Uploadtime) ? Convert.ToString(pospDetail.Uploadtime) : string.Empty);
                        Product.Add(!string.IsNullOrWhiteSpace(pospDetail.Product) ? Convert.ToString(pospDetail.Product) : string.Empty);
                        POSP.Add(!string.IsNullOrWhiteSpace(pospDetail.POSPName) ? Convert.ToString(pospDetail.POSPName) : string.Empty);
                        SeatingCapacity.Add(!string.IsNullOrWhiteSpace(pospDetail.SeatingCapacity) ? Convert.ToString(pospDetail.SeatingCapacity) : string.Empty);
                        CPA.Add(!string.IsNullOrWhiteSpace(pospDetail.CPA) ? Convert.ToString(pospDetail.CPA) : string.Empty);
                        Discount.Add("0");
                        NillDep.Add(!string.IsNullOrWhiteSpace(pospDetail.NillDep) ? Convert.ToString(pospDetail.NillDep) : string.Empty);
                        GVW.Add(!string.IsNullOrWhiteSpace(pospDetail.ODNetPremium) ? Convert.ToString(pospDetail.ODNetPremium) : string.Empty);
                        MotorType.Add(!string.IsNullOrWhiteSpace(pospDetail.MotorType) ? Convert.ToString(pospDetail.MotorType) : string.Empty);
                        BusinessFrom.Add(!string.IsNullOrWhiteSpace(pospDetail.BusinessFrom) ? Convert.ToString(pospDetail.BusinessFrom) : string.Empty);
                        UserId.Add(!string.IsNullOrWhiteSpace(pospDetail.UserId) ? Convert.ToString(pospDetail.UserId) : string.Empty);

                    }
                    // map column data with column numbers starts
                    ws.Cell(row, 1).InsertTable(Sno);
                    ws.Cell(row, 2).InsertTable(BrokerBranchCode);
                    ws.Cell(row, 3).InsertTable(BrokerBranch);
                    ws.Cell(row, 4).InsertTable(CustName);
                    ws.Cell(row, 5).InsertTable(Address);
                    ws.Cell(row, 6).InsertTable(City);
                    ws.Cell(row, 7).InsertTable(State);
                    ws.Cell(row, 8).InsertTable(Country);
                    ws.Cell(row, 9).InsertTable(Pin);
                    ws.Cell(row, 10).InsertTable(PhoneNo);
                    ws.Cell(row, 11).InsertTable(MobileNo);
                    ws.Cell(row, 12).InsertTable(DOB);
                    ws.Cell(row, 13).InsertTable(Fax);
                    ws.Cell(row, 14).InsertTable(Email);
                    ws.Cell(row, 15).InsertTable(PANNo);
                    ws.Cell(row, 16).InsertTable(RMCode);
                    ws.Cell(row, 17).InsertTable(RMName);
                    ws.Cell(row, 18).InsertTable(ReportingEmail);
                    ws.Cell(row, 19).InsertTable(ReferCode);
                    ws.Cell(row, 20).InsertTable(POSPCode);
                    ws.Cell(row, 21).InsertTable(POSPName);
                    ws.Cell(row, 22).InsertTable(CSCCode);
                    ws.Cell(row, 23).InsertTable(Ref);
                    ws.Cell(row, 24).InsertTable(Insurer);
                    ws.Cell(row, 25).InsertTable(InsurerBranchAutoCode);
                    ws.Cell(row, 26).InsertTable(PolicyType);
                    ws.Cell(row, 27).InsertTable(OD);
                    ws.Cell(row, 28).InsertTable(TP);
                    ws.Cell(row, 29).InsertTable(VehicleRegnStatus);
                    ws.Cell(row, 30).InsertTable(VehicleNo);
                    ws.Cell(row, 31).InsertTable(Make);
                    ws.Cell(row, 32).InsertTable(Variant);
                    ws.Cell(row, 33).InsertTable(YearofManf);
                    ws.Cell(row, 34).InsertTable(ChassisNo);
                    ws.Cell(row, 35).InsertTable(EngineNo);
                    ws.Cell(row, 36).InsertTable(CubicCapicaty);
                    ws.Cell(row, 37).InsertTable(Fuel);
                    ws.Cell(row, 38).InsertTable(RTOCode);
                    ws.Cell(row, 39).InsertTable(RTOName);
                    ws.Cell(row, 40).InsertTable(CurrentNCB);
                    ws.Cell(row, 41).InsertTable(PreviousNCB);
                    ws.Cell(row, 42).InsertTable(ODD);
                    ws.Cell(row, 43).InsertTable(PCV);
                    ws.Cell(row, 44).InsertTable(Passenger);
                    ws.Cell(row, 45).InsertTable(BusPropDate);
                    ws.Cell(row, 46).InsertTable(OD_start);
                    ws.Cell(row, 47).InsertTable(OD_end);
                    ws.Cell(row, 48).InsertTable(TP_start);
                    ws.Cell(row, 49).InsertTable(TP_end);
                    ws.Cell(row, 50).InsertTable(CoverNoteNo);
                    ws.Cell(row, 51).InsertTable(PolicyNo);
                    ws.Cell(row, 52).InsertTable(PolicyIssueDate);
                    ws.Cell(row, 53).InsertTable(PolicyReceiveDate);
                    ws.Cell(row, 54).InsertTable(PolRecvdFormat);
                    ws.Cell(row, 55).InsertTable(BizType);
                    ws.Cell(row, 56).InsertTable(SumInsured);
                    ws.Cell(row, 57).InsertTable(ODNetPremium);
                    ws.Cell(row, 58).InsertTable(TpPrem);
                    ws.Cell(row, 59).InsertTable(TotalTp);
                    ws.Cell(row, 60).InsertTable(NetPremium);
                    ws.Cell(row, 61).InsertTable(GST);
                    ws.Cell(row, 62).InsertTable(GrossPremium);
                    ws.Cell(row, 63).InsertTable(BankName);
                    ws.Cell(row, 64).InsertTable(POS);
                    ws.Cell(row, 65).InsertTable(TPPOS);
                    ws.Cell(row, 66).InsertTable(PolicyStatus);
                    ws.Cell(row, 67).InsertTable(PrevPolicyNO);
                    ws.Cell(row, 68).InsertTable(UserName);
                    ws.Cell(row, 69).InsertTable(OrderNumber);
                    ws.Cell(row, 70).InsertTable(Uploadtime);
                    ws.Cell(row, 71).InsertTable(Product);
                    ws.Cell(row, 72).InsertTable(POSP);
                    ws.Cell(row, 73).InsertTable(SeatingCapacity);
                    ws.Cell(row, 74).InsertTable(CPA);
                    ws.Cell(row, 75).InsertTable(Discount);
                    ws.Cell(row, 76).InsertTable(NillDep);
                    ws.Cell(row, 77).InsertTable(GVW);
                    ws.Cell(row, 78).InsertTable(MotorType);
                    ws.Cell(row, 79).InsertTable(BusinessFrom);
                    ws.Cell(row, 80).InsertTable(UserId);
                    // map column data with column numbers ends


                    // code for Header value starts
                    ws.Cell(row, 1).SetValue("Sno");
                    ws.Cell(row, 2).SetValue("BrokerBranchCode");
                    ws.Cell(row, 3).SetValue("BrokerBranch");
                    ws.Cell(row, 4).SetValue("CustName");
                    ws.Cell(row, 5).SetValue("Address");
                    ws.Cell(row, 6).SetValue("City");
                    ws.Cell(row, 7).SetValue("State");
                    ws.Cell(row, 8).SetValue("Country");
                    ws.Cell(row, 9).SetValue("Pin");
                    ws.Cell(row, 10).SetValue("PhoneNo");
                    ws.Cell(row, 11).SetValue("MobileNo");
                    ws.Cell(row, 12).SetValue("DOB");
                    ws.Cell(row, 13).SetValue("Fax");
                    ws.Cell(row, 14).SetValue("Email");
                    ws.Cell(row, 15).SetValue("PANNo");
                    ws.Cell(row, 16).SetValue("RMCode(Serviced By)");
                    ws.Cell(row, 17).SetValue("RMName (Serviced By)");
                    ws.Cell(row, 18).SetValue("ReportingEmail(RM)");
                    ws.Cell(row, 19).SetValue("ReferCode (Customer Reference)");
                    ws.Cell(row, 20).SetValue("POSPCode");
                    ws.Cell(row, 21).SetValue("POSPName");
                    ws.Cell(row, 22).SetValue("CSCCode");
                    ws.Cell(row, 23).SetValue("Ref/ParentID");
                    ws.Cell(row, 24).SetValue("Insurer");
                    ws.Cell(row, 25).SetValue("InsurerBranchAutoCode");
                    ws.Cell(row, 26).SetValue("PolicyType");
                    ws.Cell(row, 27).SetValue("OD Coverage (Tenure)");
                    ws.Cell(row, 28).SetValue("TP Coverage (Tenure)");
                    ws.Cell(row, 29).SetValue("VehicleRegnStatus");
                    ws.Cell(row, 30).SetValue("VehicleNo");
                    ws.Cell(row, 31).SetValue("Make and Model");
                    ws.Cell(row, 32).SetValue("Variant");
                    ws.Cell(row, 33).SetValue("YearofManf");
                    ws.Cell(row, 34).SetValue("ChassisNo");
                    ws.Cell(row, 35).SetValue("EngineNo");
                    ws.Cell(row, 36).SetValue("CC");
                    ws.Cell(row, 37).SetValue("Fuel");
                    ws.Cell(row, 38).SetValue("RTOCode");
                    ws.Cell(row, 39).SetValue("RTOName");
                    ws.Cell(row, 40).SetValue("CurrentNCB (%)");
                    ws.Cell(row, 41).SetValue("PreviousNCB (%)");
                    ws.Cell(row, 42).SetValue("ODD (OD Discount %)");
                    ws.Cell(row, 43).SetValue("PCV/GCV/Misc");
                    ws.Cell(row, 44).SetValue("Passenger/GVW");
                    ws.Cell(row, 45).SetValue("BusPropDate");
                    ws.Cell(row, 46).SetValue("OD PolicyStartDate");
                    ws.Cell(row, 47).SetValue("OD PolicyEndDate");
                    ws.Cell(row, 48).SetValue("TP PolicyStartDate");
                    ws.Cell(row, 49).SetValue("TP PolicyEndDate");
                    ws.Cell(row, 50).SetValue("CoverNoteNo");
                    ws.Cell(row, 51).SetValue("PolicyNo");
                    ws.Cell(row, 52).SetValue("PolicyIssueDate (Policy Created Date and Time)");
                    ws.Cell(row, 53).SetValue("PolicyReceiveDate (Policy Created Date and Time)");
                    ws.Cell(row, 54).SetValue("PolRecvdFormat");
                    ws.Cell(row, 55).SetValue("BizType");
                    ws.Cell(row, 56).SetValue("SumInsured (IDV)");
                    ws.Cell(row, 57).SetValue("ODNetPremium");
                    ws.Cell(row, 58).SetValue("TpPrem");
                    ws.Cell(row, 59).SetValue("TotalTp");
                    ws.Cell(row, 60).SetValue("NetPremium (ODNetPremium + TotalTP)");
                    ws.Cell(row, 61).SetValue("GST");
                    ws.Cell(row, 62).SetValue("GrossPremium (Net Premium + GST)");
                    ws.Cell(row, 63).SetValue("BankName");
                    ws.Cell(row, 64).SetValue("POS/MISPRate");
                    ws.Cell(row, 65).SetValue("TPPOS/MISPRate");
                    ws.Cell(row, 66).SetValue("PolicyStatus");
                    ws.Cell(row, 67).SetValue("PrevPolicyNO");
                    ws.Cell(row, 68).SetValue("UserName");
                    ws.Cell(row, 69).SetValue("OrderNumber");
                    ws.Cell(row, 70).SetValue("Uploadtime (for offline cases)");
                    ws.Cell(row, 71).SetValue("Product");
                    ws.Cell(row, 72).SetValue("POSP Product");
                    ws.Cell(row, 73).SetValue("SeatingCapacity");
                    ws.Cell(row, 74).SetValue("CPA");
                    ws.Cell(row, 75).SetValue("Discount Applied");
                    ws.Cell(row, 76).SetValue("NillDep");
                    ws.Cell(row, 77).SetValue("GVW");
                    ws.Cell(row, 78).SetValue("MotorType");
                    ws.Cell(row, 79).SetValue("BusinessFrom");
                    ws.Cell(row, 80).SetValue("UserId");
                    // Code End For Set Header Value
                    workbook.SaveAs(Path.Combine(path, timeStampNow + ".xlsx"), new ClosedXML.Excel.SaveOptions());
                    // CSV 
                    //var csvPath = Path.Combine(path, $"POSPManagement-{DateTime.Now.ToFileTime()}.csv");
                    var csvPath = Path.Combine(path, timeStampNow + ".csv");
                    using (var streamWriter = new StreamWriter(csvPath))
                    {
                        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                        {
                            var listWithoutCol = listReport.BusinessSummeryRecords.Select(x => new
                            {
                                x.Sno,
                                x.BrokerBranchCode,
                                x.BrokerBranch,
                                x.CustName,
                                x.Address,
                                x.City,
                                x.AState,
                                x.Country,
                                x.Pincode,
                                x.PhoneNo,
                                x.MobileNo,
                                x.DOB,
                                x.Fax,
                                x.EmailId,
                                x.PANNumber,
                                x.RMCode,
                                x.RMNAme,
                                x.ReportingEmail,
                                x.ReferCode,
                                x.POSPCode,
                                x.POSPName,
                                x.CSCCode,
                                x.Ref,
                                x.InsurerName,
                                x.InsurerBranchAutoCode,
                                x.PolicyType,
                                x.OD_Coverage,
                                x.TP_Coverage,
                                x.VehicleRegnStatus,
                                x.RegNo,
                                x.VehicleManufacturerName,
                                x.Variant,
                                x.YearofManf,
                                x.ChassisNo,
                                x.EngineNo,
                                x.CubicCapacity,
                                x.Fuel,
                                x.RTOCode,
                                x.RTOName,
                                x.CurrentNCB,
                                x.PreviousNCB,
                                x.ODD,
                                x.PCV,
                                x.BusPropDate,
                                x.OD_PolicyStartDate,
                                x.OD_PolicyEndDate,
                                x.TP_PolicyStartDate,
                                x.TP_PolicyEndDate,
                                x.CoverNoteNo,
                                x.PolicyNo,
                                x.PolicyIssueDate,
                                x.PolicyReceiveDate,
                                x.PolRecvdFormat,
                                x.BizType,
                                x.SumInsured,
                                x.ODNetPremium,
                                x.TpPrem,
                                x.TotalTp,
                                x.NetPremium,
                                x.GST,
                                x.GrossPremium,
                                x.BankName,
                                x.POS,
                                x.TPPOS,
                                x.PolicyStatus,
                                x.PrevPolicyNO,
                                x.UserName,
                                x.OrderNumber,
                                x.Uploadtime,
                                x.Product,
                                x.POSP,
                                x.SeatingCapacity,
                                x.CPA,
                                x.Discount_Applied,
                                x.NillDep,
                                x.GVW,
                                x.MotorType,
                                x.BusinessFrom,
                                x.UserId,
                            }).ToList();
                            csvWriter.WriteRecords(listWithoutCol);

                        }
                    }
                }
                if (query.IsExportExcel)
                {
                    listReport.BusinessSummeryRecords = null;
                }
                listReport.FileName = timeStampNow;
                return HeroResult<BusinessSummeryVm>.Success(listReport);
            }
            return HeroResult<BusinessSummeryVm>.Fail("No Record Found");
        }
    }
}
