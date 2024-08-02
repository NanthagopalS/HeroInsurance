namespace POSP.Domain.Reports
{
    /// <summary>
    /// responce model for new and old POSP report
    /// </summary>
    public record NewAndOldPOSPReportResponceModel
    {
        public IEnumerable<NewAndOldPOSPReportRecords> ReportRecords { get; set; }
        public int TotalRecords { get; set; } = 0;
        public string FileName { get; set; }
    }
    public record NewAndOldPOSPReportRecords
    {
        public int TotalRecords { get; set; } = 0;
        public string UserID { get; set; }

        public string UserName { get; set; }

        public string RoleName { get; set; }

        public string EmailId { get; set; }

        public string MobileNo { get; set; }

        public string CreatedDate { get; set; }

        public string Created_By { get; set; }

        public string Sourced_By { get; set; }

        public string Serviced_By { get; set; }

        public string PosCode { get; set; }

        public string Stage { get; set; }
        public string POS_Lead_ID { get; set; }

        public string Profile_Creation_Status { get; set; }

        public string TrainingStatus { get; set; }

        public string Exam_Status { get; set; }

        public string Document_QC_Status { get; set; }

        public string TrainingStart { get; set; }

        public string TrainingEnd { get; set; }

        public string ExamStart { get; set; }

        public string ExamEnd { get; set; }

        public string IIBUploadStatus { get; set; }

        public string IIBUploadDate { get; set; }

        public string AgreementStatus { get; set; }

        public string AgreementStart { get; set; }

        public string AgreementEnd { get; set; }

        public string Pincode { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string Aadhaarnumber { get; set; }

        public string Pannumber { get; set; }

        public string BankName { get; set; }

        public string BankAccountNo { get; set; }

        public string IFSC_Code { get; set; }

        public string Adhaar_Front { get; set; }

        public string Adhaar_Back { get; set; }

        public string PAN_Doc { get; set; }

        public string QualificationCertificate_Doc { get; set; }

        public string CancelCheque_Doc { get; set; }

        public string POS_Training_Certificate_Doc { get; set; }

        public string GST_CERTIFICATE_Doc { get; set; }

        public string TearnAndCondition_Doc { get; set; }

        public string GSTNumber { get; set; }

        public string PosSource { get; set; }
        public string DateofBirth { get; set; }

        public string POSPActivationDate { get; set; }

        public string BeneficiaryName { get; set; }

        public string ActiveForBusiness { get; set; }
        public string Status { get; set; }
        public string CreatedOn { get; set; }
        public string RmCode { get; set; }
    }
}
