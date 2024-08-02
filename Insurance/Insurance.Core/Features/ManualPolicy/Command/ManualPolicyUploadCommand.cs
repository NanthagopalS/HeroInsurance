using AutoMapper;
using ExcelDataReader;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using ThirdPartyUtilities.Abstraction;

namespace Insurance.Core.Features.ManualPolicy.Command
{
    public class ManualPolicyUploadCommand : IRequest<HeroResult<ManualPolicyUploadCommandVm>>
    {
        public IFormFile files { get; set; }
    }

    public class ManualPolicyUploadCommandHandler : IRequestHandler<ManualPolicyUploadCommand, HeroResult<ManualPolicyUploadCommandVm>>
    {
        private readonly IManualPolicyRepository _manualPolicyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ManualPolicyUploadCommandHandler> _logger;
        private readonly ICustomUtility _utility;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="manualPolicyRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="customUtility"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ManualPolicyUploadCommandHandler(IManualPolicyRepository manualPolicyRepository, IMapper mapper, ILogger<ManualPolicyUploadCommandHandler> logger, ICustomUtility customUtility)
        {
            _manualPolicyRepository = manualPolicyRepository ?? throw new ArgumentNullException(nameof(manualPolicyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utility = customUtility ?? throw new ArgumentNullException(nameof(customUtility));
        }
        /// <summary>
        /// handle
        /// </summary>
        /// <param name="roleDetailInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<ManualPolicyUploadCommandVm>> Handle(ManualPolicyUploadCommand manualPolicyUploadCommand, CancellationToken cancellationToken)
        {
            if (manualPolicyUploadCommand.files is not null)
            {
                if (manualPolicyUploadCommand.files.FileName.Contains("xlsx"))
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
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
                                    _logger.LogError("Error In Deleting Temp File:-", oldTempFiles.ToString());
                                }
                            }
                        }
                    }
                    var fileNameWithPath = Path.Combine(path, DateTime.Now.ToFileTime().ToString() + manualPolicyUploadCommand.files.FileName);

                    using (var streamCopy = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        manualPolicyUploadCommand.files.CopyTo(streamCopy);
                    }
                    DataTable migrationTable = CreatePolicyMigrationDatatable();
                    using (var stream = File.Open(fileNameWithPath, FileMode.Open, FileAccess.Read))
                    {
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                        using (var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                        {
                            FallbackEncoding = System.Text.Encoding.GetEncoding(1252)
                        }))
                        {
                            // Choose the specific sheet (0-based index) to read data from
                            reader.Read(); // Move to the first row (header)
                            while (reader.Read())
                            {
                                // Read data from each cell in the current row
                                DataRow row = migrationTable.NewRow();
                                row["userEmail"] = reader.GetValue(0)?.ToString();
                                row["MotorType"] = reader.GetValue(1)?.ToString();
                                row["PolicyType"] = reader.GetValue(2)?.ToString();
                                row["PolicyCategory"] = reader.GetValue(3)?.ToString();
                                row["BasicOD"] = reader.GetValue(4)?.ToString();
                                row["BasicTP"] = reader.GetValue(5)?.ToString();
                                row["TotalPremium"] = reader.GetValue(6)?.ToString();
                                row["NetPremium"] = reader.GetValue(7)?.ToString();
                                row["ServiceTax"] = reader.GetValue(8)?.ToString();
                                row["PolicyNo"] = reader.GetValue(9)?.ToString();
                                row["EngineNo"] = reader.GetValue(10)?.ToString();
                                row["ChasisNo"] = reader.GetValue(11)?.ToString();
                                row["VehicleNo"] = reader.GetValue(12)?.ToString();
                                row["IDV"] = reader.GetValue(13)?.ToString();
                                row["Insurer"] = reader.GetValue(14)?.ToString();
                                row["Make"] = reader.GetValue(15)?.ToString();
                                row["Fuel"] = reader.GetValue(16)?.ToString();
                                row["Variant"] = reader.GetValue(17)?.ToString();
                                row["ManufacturingMonth"] = reader.GetValue(18)?.ToString();
                                row["CustomerName"] = reader.GetValue(19)?.ToString();
                                row["PolicyIssueDate"] =_utility.ConvertToDateTimeReturnFalse(Convert.ToString(reader.GetValue(20)));
                                row["PolicyStartDate"] = _utility.ConvertToDateTimeReturnFalse(Convert.ToString(reader.GetValue(21)));
                                row["PolicyEndDate"] = _utility.ConvertToDateTimeReturnFalse(Convert.ToString(reader.GetValue(22)));
                                row["BusinessType"] = reader.GetValue(23)?.ToString();
                                row["NCB"] = reader.GetValue(24)?.ToString();
                                row["ChequeNo"] = reader.GetValue(25)?.ToString();
                                row["ChequeDate"] = _utility.ConvertToDateTimeReturnFalse(Convert.ToString(reader.GetValue(26)));
                                row["ChequeBank"] = reader.GetValue(27)?.ToString();
                                row["CustomerEmail"] = reader.GetValue(28)?.ToString();
                                row["CustomerMobile"] = reader.GetValue(29)?.ToString();
                                row["ManufacturingYear"] = reader.GetValue(30)?.ToString();
                                row["PreviousNCB"] = reader.GetValue(31)?.ToString();
                                row["CubicCapacity"] = reader.GetValue(32)?.ToString();
                                row["RTOCode"] = reader.GetValue(33)?.ToString();
                                row["PreviousPolicyNo"] = reader.GetValue(34)?.ToString();
                                row["CPA"] = reader.GetValue(35)?.ToString();
                                row["Period"] = reader.GetValue(36)?.ToString();
                                row["InsuranceType"] = reader.GetValue(37)?.ToString();
                                row["AddOnPremium"] = reader.GetValue(38)?.ToString();
                                row["NilDep"] = reader.GetValue(39)?.ToString();
                                row["IsPOSPProduct"] = reader.GetValue(40)?.ToString();
                                row["CustomerAddress"] = reader.GetValue(41)?.ToString();
                                row["State"] = reader.GetValue(42)?.ToString();
                                row["City"] = reader.GetValue(43)?.ToString();
                                row["PhoneNo"] = reader.GetValue(44)?.ToString();
                                row["PinCode"] = reader.GetValue(45)?.ToString();
                                row["CustomerDOB"] = _utility.ConvertToDateTimeReturnFalse(Convert.ToString(reader.GetValue(46)));
                                row["PANNo"] = reader.GetValue(47)?.ToString();
                                row["GrossDiscount"] = reader.GetValue(48)?.ToString();
                                row["TotalTP"] = reader.GetValue(49)?.ToString();
                                row["GVW"] = reader.GetValue(50)?.ToString();
                                row["SeatingCapacity"] = reader.GetValue(51)?.ToString();

                                migrationTable.Rows.Add(row);
                            }
                        }
                    }
                    var responce = await _manualPolicyRepository.DumpPolicyExcelRecordsToDatabase(migrationTable, cancellationToken);
                    var result = _mapper.Map<ManualPolicyUploadCommandVm>(responce);
                    return HeroResult<ManualPolicyUploadCommandVm>.Success(result);
                }
            }
            return HeroResult<ManualPolicyUploadCommandVm>.Fail("Failed To import");
        }
        static DataTable CreatePolicyMigrationDatatable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("userEmail", typeof(string));
            dt.Columns.Add("MotorType", typeof(string));
            dt.Columns.Add("PolicyType", typeof(string));
            dt.Columns.Add("PolicyCategory", typeof(string));
            dt.Columns.Add("BasicOD", typeof(string));
            dt.Columns.Add("BasicTP", typeof(string));
            dt.Columns.Add("TotalPremium", typeof(string));
            dt.Columns.Add("NetPremium", typeof(string));
            dt.Columns.Add("ServiceTax", typeof(string));
            dt.Columns.Add("PolicyNo", typeof(string));
            dt.Columns.Add("EngineNo", typeof(string));
            dt.Columns.Add("ChasisNo", typeof(string));
            dt.Columns.Add("VehicleNo", typeof(string));
            dt.Columns.Add("IDV", typeof(string));
            dt.Columns.Add("Insurer", typeof(string));
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Fuel", typeof(string));
            dt.Columns.Add("Variant", typeof(string));
            dt.Columns.Add("ManufacturingMonth", typeof(string));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("PolicyIssueDate", typeof(string));
            dt.Columns.Add("PolicyStartDate", typeof(string));
            dt.Columns.Add("PolicyEndDate", typeof(string));
            dt.Columns.Add("BusinessType", typeof(string));
            dt.Columns.Add("NCB", typeof(string));
            dt.Columns.Add("ChequeNo", typeof(string));
            dt.Columns.Add("ChequeDate", typeof(string));
            dt.Columns.Add("ChequeBank", typeof(string));
            dt.Columns.Add("CustomerEmail", typeof(string));
            dt.Columns.Add("CustomerMobile", typeof(string));
            dt.Columns.Add("ManufacturingYear", typeof(string));
            dt.Columns.Add("PreviousNCB", typeof(string));
            dt.Columns.Add("CubicCapacity", typeof(string));
            dt.Columns.Add("RTOCode", typeof(string));
            dt.Columns.Add("PreviousPolicyNo", typeof(string));
            dt.Columns.Add("CPA", typeof(string));
            dt.Columns.Add("Period", typeof(string));
            dt.Columns.Add("InsuranceType", typeof(string));
            dt.Columns.Add("AddOnPremium", typeof(string));
            dt.Columns.Add("NilDep", typeof(string));
            dt.Columns.Add("IsPOSPProduct", typeof(string));
            dt.Columns.Add("CustomerAddress", typeof(string));
            dt.Columns.Add("State", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("PhoneNo", typeof(string));
            dt.Columns.Add("PinCode", typeof(string));
            dt.Columns.Add("CustomerDOB", typeof(string));
            dt.Columns.Add("PANNo", typeof(string));
            dt.Columns.Add("GrossDiscount", typeof(string));
            dt.Columns.Add("TotalTP", typeof(string));
            dt.Columns.Add("GVW", typeof(string));
            dt.Columns.Add("SeatingCapacity", typeof(string));
            return dt;
        }

    }
}
