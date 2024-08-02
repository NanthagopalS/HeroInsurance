using AutoMapper;
using ClosedXML.Excel;
using CsvHelper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.AllReportAndMIS.RequestandResponseH
{
    public class RequestandResponseQuery : IRequest<HeroResult<RequestandResponseVM>>
    {
        public string LeadId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string InsurerId { get; set; }
        public string ProductId { get; set; }
        public string StageId { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public bool IsExportExcel { get; set; } = false;

    }
    public class RequestandResponseQueryHandler : IRequestHandler<RequestandResponseQuery, HeroResult<RequestandResponseVM>>
    {
        public readonly IInsuranceReportRepository _insuranceRepo;
        private readonly IMapper _mapper;
        private readonly ICustomUtility _customUtility;
        private readonly ILogger<RequestandResponseQuery> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="insuranceRepo"></param>
        /// <param name="mapper"></param>
        /// <param name="customUtility"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RequestandResponseQueryHandler(IInsuranceReportRepository insuranceRepo, IMapper mapper, ICustomUtility customUtility, ILogger<RequestandResponseQuery> logger)
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
        public async Task<HeroResult<RequestandResponseVM>> Handle(RequestandResponseQuery requestandResponseQuery, CancellationToken cToken)
        {
            DateTime fromDate = _customUtility.ConvertToDateTime(requestandResponseQuery.StartDate);
            DateTime toDate = _customUtility.ConvertToDateTime(requestandResponseQuery.EndDate);
            int DateDifference = DateTimeExtensions.Compare(fromDate, toDate, DateTimeInterval.Days);
            if (DateDifference > 60 || DateDifference < 0)
            {
                return HeroResult<RequestandResponseVM>.Fail("LeadId or Date Range Not Valid");
            }
            var result = await _insuranceRepo.RequestandResponseReport(requestandResponseQuery, cToken);
            if (result is not null)
            {
                var listReport = _mapper.Map<RequestandResponseVM>(result);
                string timeStampNow = ExcelExportWork(listReport, requestandResponseQuery);
                if (requestandResponseQuery.IsExportExcel)
                {
                    listReport.RequestandResponseRecord = null;
                }
                listReport.FileName = timeStampNow;
                return HeroResult<RequestandResponseVM>.Success(listReport);
            }
            return HeroResult<RequestandResponseVM>.Fail("No Record Found");
        }

        public string ExcelExportWork(RequestandResponseVM listReport, RequestandResponseQuery requestandResponseQuery)
        {
            string timeStampNow = null;
            if (requestandResponseQuery.IsExportExcel == true && listReport is not null && listReport.RequestandResponseRecord.Count() > 0)
            {
                timeStampNow = "Request Response Report - " + DateTime.Now.ToFileTime().ToString();
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
                var EnquiryId = new List<string>();
                var InsurerName = new List<string>();
                var StageID = new List<string>();
                var Product = new List<string>();
                var RequestURL = new List<string>();
                var RequestBody = new List<string>();
                var RequestTime = new List<string>();
                var ResponseBody = new List<string>();
                var ResponseTime = new List<string>();
                var Responnsestatuscode = new List<string>();
                var ResponseError = new List<string>();
                var workbook = new XLWorkbook();
                workbook.AddWorksheet("RequestandResponse Summery");
                var ws = workbook.Worksheet("RequestandResponse Summery");
                ws.ColumnWidth = 60;
                int row = 1;
                int CountSno = 0;
                foreach (var pospDetail in listReport.RequestandResponseRecord)
                {
                    ++CountSno;
                    Sno.Add(CountSno.ToString());
                    EnquiryId.Add(!string.IsNullOrWhiteSpace(pospDetail.LeadId) ? Convert.ToString(pospDetail.LeadId) : string.Empty);
                    InsurerName.Add(!string.IsNullOrWhiteSpace(pospDetail.InsurerName) ? Convert.ToString(pospDetail.InsurerName) : string.Empty);
                    StageID.Add(!string.IsNullOrWhiteSpace(pospDetail.StageID) ? Convert.ToString(pospDetail.StageID) : string.Empty);
                    Product.Add(!string.IsNullOrWhiteSpace(pospDetail.Product) ? Convert.ToString(pospDetail.Product) : string.Empty);
                    RequestURL.Add(!string.IsNullOrWhiteSpace(pospDetail.ApiURL) ? Convert.ToString(pospDetail.ApiURL) : string.Empty);
                    RequestBody.Add(!string.IsNullOrWhiteSpace(pospDetail.RequestBody) ? Convert.ToString(pospDetail.RequestBody) : string.Empty);
                    RequestTime.Add(!string.IsNullOrWhiteSpace(pospDetail.RequestTime) ? Convert.ToString(pospDetail.RequestTime) : string.Empty);
                    ResponseBody.Add(!string.IsNullOrWhiteSpace(pospDetail.ResponseBody) ? Convert.ToString(pospDetail.ResponseBody) : string.Empty);
                    ResponseTime.Add(!string.IsNullOrWhiteSpace(pospDetail.ResponseTime) ? Convert.ToString(pospDetail.ResponseTime) : string.Empty);
                    Responnsestatuscode.Add(!string.IsNullOrWhiteSpace(pospDetail.Responnsestatuscode) ? Convert.ToString(pospDetail.Responnsestatuscode) : string.Empty);
                    ResponseError.Add(!string.IsNullOrWhiteSpace(pospDetail.ResponseError) ? Convert.ToString(pospDetail.ResponseError) : string.Empty);
                }
                // map column data with column numbers starts
                ws.Cell(row, 1).InsertTable(Sno);
                ws.Cell(row, 2).InsertTable(EnquiryId);
                ws.Cell(row, 3).InsertTable(InsurerName);
                ws.Cell(row, 4).InsertTable(StageID);
                ws.Cell(row, 5).InsertTable(Product);
                ws.Cell(row, 6).InsertTable(RequestURL);
                ws.Cell(row, 7).InsertTable(RequestBody);
                ws.Cell(row, 8).InsertTable(RequestTime);
                ws.Cell(row, 9).InsertTable(ResponseBody);
                ws.Cell(row, 10).InsertTable(ResponseTime);
                ws.Cell(row, 11).InsertTable(Responnsestatuscode);
                ws.Cell(row, 12).InsertTable(ResponseError);
                // map column data with column numbers ends
                // code for Header value starts
                ws.Cell(row, 1).SetValue("Sno");
                ws.Cell(row, 2).SetValue("EnquiryId");
                ws.Cell(row, 3).SetValue("InsurerName");
                ws.Cell(row, 4).SetValue("Stage");
                ws.Cell(row, 5).SetValue("Product");
                ws.Cell(row, 6).SetValue("RequestURL");
                ws.Cell(row, 7).SetValue("RequestBody");
                ws.Cell(row, 8).SetValue("RequestTime");
                ws.Cell(row, 9).SetValue("ResponseBody");
                ws.Cell(row, 10).SetValue("ResponseTime");
                ws.Cell(row, 11).SetValue("Responnsestatuscode");
                ws.Cell(row, 12).SetValue("ResponseError");
                // Code End For Set Header Value
                workbook.SaveAs(Path.Combine(path, timeStampNow + ".xlsx"), new ClosedXML.Excel.SaveOptions());
                // CSV 
                //var csvPath = Path.Combine(path, $"POSPManagement-{DateTime.Now.ToFileTime()}.csv");
                var csvPath = Path.Combine(path, timeStampNow + ".csv");
                using (var streamWriter = new StreamWriter(csvPath))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {
                        var listWithoutCol = listReport.RequestandResponseRecord.Select(x => new
                        {
                            x.Sno,
                            x.LeadId,
                            x.InsurerName,
                            x.StageID,
                            x.Product,
                            x.ApiURL,
                            x.RequestBody,
                            x.RequestTime,
                            x.ResponseBody,
                            x.ResponseTime,
                            x.Responnsestatuscode,
                            x.ResponseError
                        }).ToList();
                        csvWriter.WriteRecords(listWithoutCol);
                    }
                }
            }
            return timeStampNow;
        }
    }
}
