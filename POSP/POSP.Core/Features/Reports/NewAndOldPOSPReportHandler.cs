using AutoMapper;
using ClosedXML.Excel;
using CsvHelper;
using MediatR;
using Microsoft.Extensions.Logging;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System.Globalization;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;

namespace POSP.Core.Features.Reports
{
    /// <summary>
    /// query for new and old POSP report
    /// </summary>
    public record NewAndOldPOSPReportQuery : IRequest<HeroResult<NewAndOldPOSPReportVm>>
    {
        public string SearchText { get; set; }
        public string StageId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsExportResponce { get; set; } = false;
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public string status { get; set; }
    }
    public class NewAndOldPOSPReportHandler : IRequestHandler<NewAndOldPOSPReportQuery, HeroResult<NewAndOldPOSPReportVm>>
    {
        private readonly IPOSPReportRepository _pospReportRepo;
        private readonly IMapper _mapper;
        private readonly ICustomUtility _customUtility;
        private readonly ILogger<NewAndOldPOSPReportHandler> _logger;

        public NewAndOldPOSPReportHandler(IPOSPReportRepository pospReportRepo, IMapper mapper, ICustomUtility utility, ILogger<NewAndOldPOSPReportHandler> logger)
        {
            _pospReportRepo = pospReportRepo ?? throw new ArgumentNullException(nameof(pospReportRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _customUtility = utility ?? throw new ArgumentNullException(nameof(utility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<HeroResult<NewAndOldPOSPReportVm>> Handle(NewAndOldPOSPReportQuery query, CancellationToken cancellationToken)
        {
            DateTime fromDate = _customUtility.ConvertToDateTime(query.StartDate);
            DateTime toDate = _customUtility.ConvertToDateTime(query.EndDate);
            int DateDifference = DateTimeExtensions.Compare(fromDate, toDate, DateTimeInterval.Days);
            if (DateDifference >= 60 || DateDifference < 0)
            {
                return HeroResult<NewAndOldPOSPReportVm>.Fail("Date Range Not Valid");
            }
            var modelResult = await _pospReportRepo.NewAndOldPOSPReport(query, cancellationToken);
            if (modelResult is not null)
            {
                var listInsurerModel = _mapper.Map<NewAndOldPOSPReportVm>(modelResult);
                listInsurerModel.FileName = CreateNewAndOldPOSPReportFile(query, listInsurerModel);
                if (query.IsExportResponce)
                {
                    listInsurerModel.ReportRecords = null;
                }
                return HeroResult<NewAndOldPOSPReportVm>.Success(listInsurerModel);
            }
            return HeroResult<NewAndOldPOSPReportVm>.Fail("No Record Found");
        }
        public string CreateNewAndOldPOSPReportFile(NewAndOldPOSPReportQuery query, NewAndOldPOSPReportVm listInsurerModel)
        {
            string timeStampNow = null;
            if (query.IsExportResponce == true && listInsurerModel is not null && listInsurerModel.ReportRecords.Count() > 0)
            {

                timeStampNow = "NewAndOldPOSP" + " - " + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
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

                var UserID = new List<string>();
                var UserName = new List<string>();
                var RoleName = new List<string>();
                var EmailAddress = new List<string>();
                var MobileNo = new List<string>();
                var CreatedDate = new List<string>();
                var Created = new List<string>();
                var Sourced = new List<string>();
                var Serviced = new List<string>();
                var PosCode = new List<string>();
                var Stage = new List<string>();
                var RmCode = new List<string>();
                var Lead = new List<string>();
                var Creation = new List<string>();
                var TrainingStatus = new List<string>();
                var Exam = new List<string>();
                var Document = new List<string>();
                var TrainingStart = new List<string>();
                var TrainingEnd = new List<string>();
                var ExamStart = new List<string>();
                var ExamEnd = new List<string>();
                var IIBUploadStatus = new List<string>();
                var IIBUploadDate = new List<string>();
                var AgreementStatus = new List<string>();
                var AgreementStart = new List<string>();
                var AgreementEnd = new List<string>();
                var Pincode = new List<string>();
                var City = new List<string>();
                var State = new List<string>();
                var Address1 = new List<string>();
                var Address2 = new List<string>();
                var Address3 = new List<string>();
                var Aadhaarnumber = new List<string>();
                var Pannumber = new List<string>();
                var BankName = new List<string>();
                var BankAccountNo = new List<string>();
                var IFSC_Code = new List<string>();
                var Adhaar_Front = new List<string>();
                var Adhaar_Back = new List<string>();
                var PAN_Doc = new List<string>();
                var QualificationCertificate_Doc = new List<string>();
                var CancelCheque_Doc = new List<string>();
                var POS_Training_Certificate_Doc = new List<string>();
                var GST_CERTIFICATE_Doc = new List<string>();
                var TearnAndCondition_Doc = new List<string>();
                var GSTNumber = new List<string>();
                var PosSource = new List<string>();
                var DOB = new List<string>();
                var POSPActivationDate = new List<string>();
                var BeneficiaryName = new List<string>();
                var ActiveForBusiness = new List<string>();

                var workbook = new XLWorkbook();
                workbook.AddWorksheet("New And Old POSP");
                var ws = workbook.Worksheet("New And Old POSP");
                ws.ColumnWidth = 60;
                int row = 1;
                foreach (var pospDetail in listInsurerModel.ReportRecords)
                {
                    UserID.Add(!string.IsNullOrWhiteSpace(pospDetail.UserID) ? Convert.ToString(pospDetail.UserID) : string.Empty);
                    UserName.Add(!string.IsNullOrWhiteSpace(pospDetail.UserName) ? Convert.ToString(pospDetail.UserName) : string.Empty);
                    RoleName.Add(!string.IsNullOrWhiteSpace(pospDetail.RoleName) ? Convert.ToString(pospDetail.RoleName) : string.Empty);
                    EmailAddress.Add(!string.IsNullOrWhiteSpace(pospDetail.EmailId) ? Convert.ToString(pospDetail.EmailId) : string.Empty);
                    MobileNo.Add(!string.IsNullOrWhiteSpace(pospDetail.MobileNo) ? Convert.ToString(pospDetail.MobileNo) : string.Empty);
                    CreatedDate.Add(!string.IsNullOrWhiteSpace(pospDetail.CreatedOn) ? Convert.ToString(pospDetail.CreatedOn) : string.Empty);
                    Created.Add(!string.IsNullOrWhiteSpace(pospDetail.Created_By) ? Convert.ToString(pospDetail.Created_By) : string.Empty);
                    Sourced.Add(!string.IsNullOrWhiteSpace(pospDetail.Sourced_By) ? Convert.ToString(pospDetail.Sourced_By) : string.Empty);
                    Serviced.Add(!string.IsNullOrWhiteSpace(pospDetail.Serviced_By) ? Convert.ToString(pospDetail.Serviced_By) : string.Empty);
                    PosCode.Add(!string.IsNullOrWhiteSpace(pospDetail.PosCode) ? Convert.ToString(pospDetail.PosCode) : string.Empty);
                    Stage.Add(!string.IsNullOrWhiteSpace(pospDetail.Stage) ? Convert.ToString(pospDetail.Stage) : string.Empty);
                    RmCode.Add(!string.IsNullOrWhiteSpace(pospDetail.Serviced_By) ? Convert.ToString(pospDetail.Serviced_By) : string.Empty);
                    Lead.Add(!string.IsNullOrWhiteSpace(pospDetail.POS_Lead_ID) ? Convert.ToString(pospDetail.POS_Lead_ID) : string.Empty);
                    Creation.Add(!string.IsNullOrWhiteSpace(pospDetail.CreatedOn) ? Convert.ToString(pospDetail.CreatedOn) : string.Empty);
                    TrainingStatus.Add(!string.IsNullOrWhiteSpace(pospDetail.TrainingStatus) ? Convert.ToString(pospDetail.TrainingStatus) : string.Empty);
                    Exam.Add(!string.IsNullOrWhiteSpace(pospDetail.Exam_Status) ? Convert.ToString(pospDetail.Exam_Status) : string.Empty);
                    Document.Add(!string.IsNullOrWhiteSpace(pospDetail.Document_QC_Status) ? Convert.ToString(pospDetail.Document_QC_Status) : string.Empty);
                    TrainingStart.Add(!string.IsNullOrWhiteSpace(pospDetail.TrainingStart) ? Convert.ToString(pospDetail.TrainingStart) : string.Empty);
                    TrainingEnd.Add(!string.IsNullOrWhiteSpace(pospDetail.TrainingEnd) ? Convert.ToString(pospDetail.TrainingEnd) : string.Empty);
                    ExamStart.Add(!string.IsNullOrWhiteSpace(pospDetail.ExamStart) ? Convert.ToString(pospDetail.ExamStart) : string.Empty);
                    ExamEnd.Add(!string.IsNullOrWhiteSpace(pospDetail.ExamEnd) ? Convert.ToString(pospDetail.ExamEnd) : string.Empty);
                    IIBUploadStatus.Add(!string.IsNullOrWhiteSpace(pospDetail.IIBUploadStatus) ? Convert.ToString(pospDetail.IIBUploadStatus) : string.Empty);
                    IIBUploadDate.Add(!string.IsNullOrWhiteSpace(pospDetail.IIBUploadDate) ? Convert.ToString(pospDetail.IIBUploadDate) : string.Empty);
                    AgreementStatus.Add(!string.IsNullOrWhiteSpace(pospDetail.AgreementStatus) ? Convert.ToString(pospDetail.AgreementStatus) : string.Empty);
                    AgreementStart.Add(!string.IsNullOrWhiteSpace(pospDetail.AgreementStart) ? Convert.ToString(pospDetail.AgreementStart) : string.Empty);
                    AgreementEnd.Add(!string.IsNullOrWhiteSpace(pospDetail.AgreementEnd) ? Convert.ToString(pospDetail.AgreementEnd) : string.Empty);
                    Pincode.Add(!string.IsNullOrWhiteSpace(pospDetail.Pincode) ? Convert.ToString(pospDetail.Pincode) : string.Empty);
                    City.Add(!string.IsNullOrWhiteSpace(pospDetail.City) ? Convert.ToString(pospDetail.City) : string.Empty);
                    State.Add(!string.IsNullOrWhiteSpace(pospDetail.State) ? Convert.ToString(pospDetail.State) : string.Empty);
                    Address1.Add(!string.IsNullOrWhiteSpace(pospDetail.AddressLine1) ? Convert.ToString(pospDetail.AddressLine1) : string.Empty);
                    Address2.Add(!string.IsNullOrWhiteSpace(pospDetail.AddressLine2) ? Convert.ToString(pospDetail.AddressLine2) : string.Empty);
                    Address3.Add(!string.IsNullOrWhiteSpace(pospDetail.AddressLine3) ? Convert.ToString(pospDetail.AddressLine3) : string.Empty);
                    Aadhaarnumber.Add(!string.IsNullOrWhiteSpace(pospDetail.Aadhaarnumber) ? Convert.ToString(pospDetail.Aadhaarnumber) : string.Empty);
                    Pannumber.Add(!string.IsNullOrWhiteSpace(pospDetail.Pannumber) ? Convert.ToString(pospDetail.Pannumber) : string.Empty);
                    BankName.Add(!string.IsNullOrWhiteSpace(pospDetail.BankName) ? Convert.ToString(pospDetail.BankName) : string.Empty);
                    BankAccountNo.Add(!string.IsNullOrWhiteSpace(pospDetail.BankAccountNo) ? Convert.ToString(pospDetail.BankAccountNo) : string.Empty);
                    IFSC_Code.Add(!string.IsNullOrWhiteSpace(pospDetail.IFSC_Code) ? Convert.ToString(pospDetail.IFSC_Code) : string.Empty);
                    Adhaar_Front.Add(!string.IsNullOrWhiteSpace(pospDetail.Adhaar_Front) ? Convert.ToString(pospDetail.Adhaar_Front) : string.Empty);
                    Adhaar_Back.Add(!string.IsNullOrWhiteSpace(pospDetail.Adhaar_Back) ? Convert.ToString(pospDetail.Adhaar_Back) : string.Empty);
                    PAN_Doc.Add(!string.IsNullOrWhiteSpace(pospDetail.PAN_Doc) ? Convert.ToString(pospDetail.PAN_Doc) : string.Empty);
                    QualificationCertificate_Doc.Add(!string.IsNullOrWhiteSpace(pospDetail.QualificationCertificate_Doc) ? Convert.ToString(pospDetail.QualificationCertificate_Doc) : string.Empty);
                    CancelCheque_Doc.Add(!string.IsNullOrWhiteSpace(pospDetail.CancelCheque_Doc) ? Convert.ToString(pospDetail.CancelCheque_Doc) : string.Empty);
                    POS_Training_Certificate_Doc.Add(!string.IsNullOrWhiteSpace(pospDetail.POS_Training_Certificate_Doc) ? Convert.ToString(pospDetail.POS_Training_Certificate_Doc) : string.Empty);
                    GST_CERTIFICATE_Doc.Add(!string.IsNullOrWhiteSpace(pospDetail.GST_CERTIFICATE_Doc) ? Convert.ToString(pospDetail.GST_CERTIFICATE_Doc) : string.Empty);
                    TearnAndCondition_Doc.Add(!string.IsNullOrWhiteSpace(pospDetail.TearnAndCondition_Doc) ? Convert.ToString(pospDetail.TearnAndCondition_Doc) : string.Empty);
                    GSTNumber.Add(!string.IsNullOrWhiteSpace(pospDetail.GSTNumber) ? Convert.ToString(pospDetail.GSTNumber) : string.Empty);
                    PosSource.Add(!string.IsNullOrWhiteSpace(pospDetail.PosSource) ? Convert.ToString(pospDetail.PosSource) : string.Empty);
                    DOB.Add(!string.IsNullOrWhiteSpace(pospDetail.DateofBirth) ? Convert.ToString(pospDetail.DateofBirth) : string.Empty);
                    POSPActivationDate.Add(!string.IsNullOrWhiteSpace(pospDetail.POSPActivationDate) ? Convert.ToString(pospDetail.POSPActivationDate) : string.Empty);
                    BeneficiaryName.Add(!string.IsNullOrWhiteSpace(pospDetail.BeneficiaryName) ? Convert.ToString(pospDetail.BeneficiaryName) : string.Empty);
                    ActiveForBusiness.Add(!string.IsNullOrWhiteSpace(pospDetail.ActiveForBusiness) ? Convert.ToString(pospDetail.ActiveForBusiness) : string.Empty);

                }
                // map column data with column numbers starts
                ws.Cell(row, 1).InsertTable(UserID);
                ws.Cell(row, 2).InsertTable(UserName);
                ws.Cell(row, 3).InsertTable(RoleName);
                ws.Cell(row, 4).InsertTable(EmailAddress);
                ws.Cell(row, 5).InsertTable(MobileNo);
                ws.Cell(row, 6).InsertTable(CreatedDate);
                ws.Cell(row, 7).InsertTable(Created);
                ws.Cell(row, 8).InsertTable(Sourced);
                ws.Cell(row, 9).InsertTable(Serviced);
                ws.Cell(row, 10).InsertTable(PosCode);
                ws.Cell(row, 11).InsertTable(Stage);
                ws.Cell(row, 12).InsertTable(RmCode);
                ws.Cell(row, 13).InsertTable(Lead);
                ws.Cell(row, 14).InsertTable(Creation);
                ws.Cell(row, 15).InsertTable(TrainingStatus);
                ws.Cell(row, 16).InsertTable(Exam);
                ws.Cell(row, 17).InsertTable(Document);
                ws.Cell(row, 18).InsertTable(TrainingStart);
                ws.Cell(row, 19).InsertTable(TrainingEnd);
                ws.Cell(row, 20).InsertTable(ExamStart);
                ws.Cell(row, 21).InsertTable(ExamEnd);
                ws.Cell(row, 22).InsertTable(IIBUploadStatus);
                ws.Cell(row, 23).InsertTable(IIBUploadDate);
                ws.Cell(row, 24).InsertTable(AgreementStatus);
                ws.Cell(row, 25).InsertTable(AgreementStart);
                ws.Cell(row, 26).InsertTable(AgreementEnd);
                ws.Cell(row, 27).InsertTable(Pincode);
                ws.Cell(row, 28).InsertTable(City);
                ws.Cell(row, 29).InsertTable(State);
                ws.Cell(row, 30).InsertTable(Address1);
                ws.Cell(row, 31).InsertTable(Address2);
                ws.Cell(row, 32).InsertTable(Address3);
                ws.Cell(row, 33).InsertTable(Aadhaarnumber);
                ws.Cell(row, 34).InsertTable(Pannumber);
                ws.Cell(row, 35).InsertTable(BankName);
                ws.Cell(row, 36).InsertTable(BankAccountNo);
                ws.Cell(row, 37).InsertTable(IFSC_Code);
                ws.Cell(row, 38).InsertTable(Adhaar_Front);
                ws.Cell(row, 39).InsertTable(Adhaar_Back);
                ws.Cell(row, 40).InsertTable(PAN_Doc);
                ws.Cell(row, 41).InsertTable(QualificationCertificate_Doc);
                ws.Cell(row, 42).InsertTable(CancelCheque_Doc);
                ws.Cell(row, 43).InsertTable(POS_Training_Certificate_Doc);
                ws.Cell(row, 44).InsertTable(GST_CERTIFICATE_Doc);
                ws.Cell(row, 45).InsertTable(TearnAndCondition_Doc);
                ws.Cell(row, 46).InsertTable(GSTNumber);
                ws.Cell(row, 47).InsertTable(PosSource);
                ws.Cell(row, 48).InsertTable(DOB);
                ws.Cell(row, 49).InsertTable(POSPActivationDate);
                ws.Cell(row, 50).InsertTable(BeneficiaryName);
                ws.Cell(row, 51).InsertTable(ActiveForBusiness);
                // map column data with column numbers ends


                // code for Header value starts

                ws.Cell(row, 1).SetValue("UserID");
                ws.Cell(row, 2).SetValue("UserName");
                ws.Cell(row, 3).SetValue("RoleName");
                ws.Cell(row, 4).SetValue("EmailAddress");
                ws.Cell(row, 5).SetValue("MobileNo");
                ws.Cell(row, 6).SetValue("CreatedDate");
                ws.Cell(row, 7).SetValue("Created By");
                ws.Cell(row, 8).SetValue("Sourced By");
                ws.Cell(row, 9).SetValue("Serviced By");
                ws.Cell(row, 10).SetValue("PosCode");
                ws.Cell(row, 11).SetValue("Stage");
                ws.Cell(row, 12).SetValue("RmCode / Serviced By");
                ws.Cell(row, 13).SetValue("POSP Lead ID");
                ws.Cell(row, 14).SetValue("Creation Status");
                ws.Cell(row, 15).SetValue("TrainingStatus");
                ws.Cell(row, 16).SetValue("Exam Status");
                ws.Cell(row, 17).SetValue("Document QC Status");
                ws.Cell(row, 18).SetValue("TrainingStart");
                ws.Cell(row, 19).SetValue("TrainingEnd");
                ws.Cell(row, 20).SetValue("ExamStart");
                ws.Cell(row, 21).SetValue("ExamEnd");
                ws.Cell(row, 22).SetValue("IIBUploadStatus");
                ws.Cell(row, 23).SetValue("IIBUploadDate");
                ws.Cell(row, 24).SetValue("AgreementStatus");
                ws.Cell(row, 25).SetValue("AgreementStart");
                ws.Cell(row, 26).SetValue("AgreementEnd");
                ws.Cell(row, 27).SetValue("Pincode");
                ws.Cell(row, 28).SetValue("City");
                ws.Cell(row, 29).SetValue("State");
                ws.Cell(row, 30).SetValue("Address Line 1");
                ws.Cell(row, 31).SetValue("Address Line 2");
                ws.Cell(row, 32).SetValue("Address Line 3");
                ws.Cell(row, 33).SetValue("Aadhaarnumber");
                ws.Cell(row, 34).SetValue("Pannumber");
                ws.Cell(row, 35).SetValue("BankName");
                ws.Cell(row, 36).SetValue("BankAccountNo");
                ws.Cell(row, 37).SetValue("IFSC_Code");
                ws.Cell(row, 38).SetValue("Adhaar_Front");
                ws.Cell(row, 39).SetValue("Adhaar_Back");
                ws.Cell(row, 40).SetValue("PAN_Doc");
                ws.Cell(row, 41).SetValue("QualificationCertificate_Doc");
                ws.Cell(row, 42).SetValue("CancelCheque_Doc");
                ws.Cell(row, 43).SetValue("POS_Training_Certificate_Doc");
                ws.Cell(row, 44).SetValue("GST_CERTIFICATE_Doc");
                ws.Cell(row, 45).SetValue("TearnAndCondition_Doc");
                ws.Cell(row, 46).SetValue("GSTNumber");
                ws.Cell(row, 47).SetValue("PosSource");
                ws.Cell(row, 48).SetValue("DOB");
                ws.Cell(row, 49).SetValue("POSPActivationDate(Agreement Sign Date)");
                ws.Cell(row, 50).SetValue("BeneficiaryName");
                ws.Cell(row, 51).SetValue("ActiveForBusiness");

                // Code End For Set Header Value
                workbook.SaveAs(Path.Combine(path, timeStampNow + ".xlsx"), new ClosedXML.Excel.SaveOptions());
                // CSV 
                //var csvPath = Path.Combine(path, $"POSPManagement-{DateTime.Now.ToFileTime()}.csv");
                var csvPath = Path.Combine(path, timeStampNow + ".csv");
                using (var streamWriter = new StreamWriter(csvPath))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {
                        var listWithoutCol = listInsurerModel.ReportRecords.Select(x => new {
                            x.UserID,
                            x.UserName,
                            x.RoleName,
                            x.EmailId,
                            x.MobileNo,
                            x.CreatedDate,
                            x.Created_By,
                            x.Sourced_By,
                            x.Serviced_By,
                            x.PosCode,
                            x.Stage,
                            x.RmCode,
                            x.POS_Lead_ID,
                            x.TrainingStatus,
                            x.Exam_Status,
                            x.Document_QC_Status,
                            x.TrainingStart,
                            x.TrainingEnd,
                            x.ExamStart,
                            x.ExamEnd,
                            x.IIBUploadStatus,
                            x.IIBUploadDate,
                            x.AgreementStatus,
                            x.AgreementStart,
                            x.AgreementEnd,
                            x.Pincode,
                            x.City,
                            x.State,
                            x.AddressLine1,
                            x.AddressLine2,
                            x.AddressLine3,
                            x.Aadhaarnumber,
                            x.Pannumber,
                            x.BankName,
                            x.BankAccountNo,
                            x.IFSC_Code,
                            x.Adhaar_Front,
                            x.Adhaar_Back,
                            x.PAN_Doc,
                            x.QualificationCertificate_Doc,
                            x.CancelCheque_Doc,
                            x.POS_Training_Certificate_Doc,
                            x.GST_CERTIFICATE_Doc,
                            x.TearnAndCondition_Doc,
                            x.GSTNumber,
                            x.PosSource,
                            x.DateofBirth,
                            x.POSPActivationDate,
                            x.BeneficiaryName,
                            x.ActiveForBusiness

                        }).ToList();

                        csvWriter.WriteRecords(listWithoutCol);

                    }
                }
            }
            return timeStampNow;
        }
    }
}
