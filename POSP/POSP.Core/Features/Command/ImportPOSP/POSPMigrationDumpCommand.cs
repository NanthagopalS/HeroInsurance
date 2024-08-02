
using AutoMapper;
using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Http;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System.Data;
using ThirdPartyUtilities.Abstraction;

namespace POSP.Core.Features.Command.ImportPOSP
{
    public class POSPMigrationDumpCommand : IRequest<HeroResult<POSPMigrationResponceModalVm>>
    {
        public IFormFile files { get; set; }
    }

    public class POSPMigrationDumpCommandHandler : IRequestHandler<POSPMigrationDumpCommand, HeroResult<POSPMigrationResponceModalVm>>
    {
        private readonly IPOSPMigrationRepository _migrationRepository;
        private readonly IMapper _mapper;
        private readonly ICustomUtility _utility;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="buRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="utility"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public POSPMigrationDumpCommandHandler(IPOSPMigrationRepository buRepository, IMapper mapper, ICustomUtility utility)
        {
            _migrationRepository = buRepository ?? throw new ArgumentNullException(nameof(buRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _utility = utility ?? throw new ArgumentNullException(nameof(_utility));
        }
        /// <summary>
        /// handle
        /// </summary>
        /// <param name="roleDetailInsertCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<POSPMigrationResponceModalVm>> Handle(POSPMigrationDumpCommand pOSPMigrationDumpCommand, CancellationToken cancellationToken)
        {
            if (pOSPMigrationDumpCommand.files is not null)
            {
                if (pOSPMigrationDumpCommand.files.FileName.Contains("xlsm") || pOSPMigrationDumpCommand.files.FileName.Contains("xlsx"))
                {

                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Services/Files");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileNameWithPath = Path.Combine(path, pOSPMigrationDumpCommand.files.FileName);

                    using (var streamCopy = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        pOSPMigrationDumpCommand.files.CopyTo(streamCopy);
                    }
                    DataTable migrationTable = CreateMigrationDatatable();
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
                                row["posp_code"] = reader.GetValue(0).ToString();
                                row["name"] = reader.GetValue(1).ToString();
                                row["alias_name"] = reader.GetValue(2)?.ToString();
                                row["email"] = reader.GetValue(3)?.ToString();
                                row["mobile"] = reader.GetValue(4)?.ToString();
                                row["pan_number"] = reader.GetValue(5)?.ToString();
                                row["dob"] = _utility.ConvertToDateTime(reader.GetValue(6)?.ToString()).ToString("yyyy-MM-dd");
                                row["father_name"] = reader.GetValue(7)?.ToString();
                                row["gender"] = reader.GetValue(8)?.ToString();
                                row["adhar_no"] = reader.GetValue(9)?.ToString();
                                row["alternate_mobile"] = reader.GetValue(10)?.ToString();
                                row["address1"] = reader.GetValue(11)?.ToString();
                                row["address2"] = reader.GetValue(12)?.ToString();
                                row["pincode"] = reader.GetValue(13)?.ToString();
                                row["state"] = reader.GetValue(14)?.ToString();
                                row["city"] = reader.GetValue(15)?.ToString();
                                row["product_type"] = reader.GetValue(16)?.ToString();
                                row["noc_available"] = reader.GetValue(17)?.ToString();
                                row["bank_name"] = reader.GetValue(18)?.ToString();
                                row["ifsc_code"] = reader.GetValue(19)?.ToString();
                                row["account_holder_name"] = reader.GetValue(20)?.ToString();
                                row["account_number"] = reader.GetValue(21)?.ToString().Replace("'", "");
                                row["educational_qualification"] = reader.GetValue(22)?.ToString();
                                row["select_average_premium"] = reader.GetValue(23)?.ToString();
                                row["POSPBU"] = reader.GetValue(24)?.ToString();
                                row["created_date"] = _utility.ConvertToDateTime(reader.GetValue(25)?.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                row["created_by"] = reader.GetValue(26)?.ToString();
                                row["sourced_by"] = reader.GetValue(27)?.ToString();
                                row["serviced_by"] = reader.GetValue(28)?.ToString();
                                row["posp_source"] = reader.GetValue(29)?.ToString();
                                row["general_training_start"] = _utility.ConvertToDateTime(reader.GetValue(30)?.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                row["general_training_end"] = _utility.ConvertToDateTime(reader.GetValue(31)?.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                row["life_insurance_training_start"] = _utility.ConvertToDateTime(reader.GetValue(32)?.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                row["life_insurance_training_end"] = _utility.ConvertToDateTime(reader.GetValue(33)?.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                row["exam_status"] = reader.GetValue(34)?.ToString();
                                row["exam_start"] = _utility.ConvertToDateTime(reader.GetValue(35)?.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                row["exam_end"] = _utility.ConvertToDateTime(reader.GetValue(36)?.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                row["IIBUploadStatus"] = reader.GetValue(37)?.ToString();
                                row["iib_upload_date"] = _utility.ConvertToDateTime(reader.GetValue(38)?.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                row["AgreementStatus"] = reader.GetValue(39)?.ToString();
                                migrationTable.Rows.Add(row);
                            }
                            var responce = await _migrationRepository.DumpExcelRecordsToDatabase(migrationTable, cancellationToken);
                            var result = _mapper.Map<POSPMigrationResponceModalVm>(responce);
                            System.IO.File.Delete(fileNameWithPath);
                            return HeroResult<POSPMigrationResponceModalVm>.Success(result);
                        }
                    }
                }
            }
            return HeroResult<POSPMigrationResponceModalVm>.Fail("Failed To import");
        }
        static DataTable CreateMigrationDatatable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("posp_code", typeof(string));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("alias_name", typeof(string));
            dt.Columns.Add("email", typeof(string));
            dt.Columns.Add("mobile", typeof(string));
            dt.Columns.Add("pan_number", typeof(string));
            dt.Columns.Add("dob", typeof(string));
            dt.Columns.Add("father_name", typeof(string));
            dt.Columns.Add("gender", typeof(string));
            dt.Columns.Add("adhar_no", typeof(string));
            dt.Columns.Add("alternate_mobile", typeof(string));
            dt.Columns.Add("address1", typeof(string));
            dt.Columns.Add("address2", typeof(string));
            dt.Columns.Add("pincode", typeof(string));
            dt.Columns.Add("state", typeof(string));
            dt.Columns.Add("city", typeof(string));
            dt.Columns.Add("product_type", typeof(string));
            dt.Columns.Add("noc_available", typeof(string));
            dt.Columns.Add("bank_name", typeof(string));
            dt.Columns.Add("ifsc_code", typeof(string));
            dt.Columns.Add("account_holder_name", typeof(string));
            dt.Columns.Add("account_number", typeof(string));
            dt.Columns.Add("educational_qualification", typeof(string));
            dt.Columns.Add("select_average_premium", typeof(string));
            dt.Columns.Add("POSPBU", typeof(string));
            dt.Columns.Add("created_date", typeof(string));
            dt.Columns.Add("created_by", typeof(string));
            dt.Columns.Add("sourced_by", typeof(string));
            dt.Columns.Add("serviced_by", typeof(string));
            dt.Columns.Add("posp_source", typeof(string));
            dt.Columns.Add("general_training_start", typeof(string));
            dt.Columns.Add("general_training_end", typeof(string));
            dt.Columns.Add("life_insurance_training_start", typeof(string));
            dt.Columns.Add("life_insurance_training_end", typeof(string));
            dt.Columns.Add("exam_status", typeof(string));
            dt.Columns.Add("exam_start", typeof(string));
            dt.Columns.Add("exam_end", typeof(string));
            dt.Columns.Add("IIBUploadStatus", typeof(string));
            dt.Columns.Add("iib_upload_date", typeof(string));
            dt.Columns.Add("AgreementStatus", typeof(string));
            return dt;
        }

    }

}
