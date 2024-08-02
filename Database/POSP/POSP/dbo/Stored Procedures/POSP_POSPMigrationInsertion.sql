
/* 
DROP PROCEDURE [dbo].[POSP_POSPMigrationInsertion] 
*/
CREATE   PROCEDURE [dbo].[POSP_POSPMigrationInsertion] (
	@TempTable AS dbo.POSPMigrationDumpTableType READONLY
	,@CreatedBy VARCHAR(50) = NULL
	)
AS
BEGIN
	DECLARE @TransactionName NVARCHAR(20) = 'DUMPPOSPTransaction';

	BEGIN TRANSACTION @TransactionName;

	BEGIN TRY
		INSERT INTO POSPMigrationDumpTable (
			posp_code
			,name
			,alias_name
			,email
			,mobile
			,pan_number
			,dob
			,father_name
			,gender
			,adhar_no
			,alternate_mobile
			,address1
			,address2
			,pincode
			,STATE
			,city
			,product_type
			,noc_available
			,bank_name
			,ifsc_code
			,account_holder_name
			,account_number
			,educational_qualification
			,select_average_premium
			,POSPBU
			,created_date
			,created_by
			,sourced_by
			,serviced_by
			,posp_source
			,general_training_start
			,general_training_end
			,life_insurance_training_start
			,life_insurance_training_end
			,exam_status
			,exam_start
			,exam_end
			,IIBUploadStatus
			,iib_upload_date
			,AgreementStatus
			,CreatedBy
			)
		SELECT posp_code
			,name
			,alias_name
			,email
			,mobile
			,pan_number
			,dob
			,father_name
			,gender
			,adhar_no
			,alternate_mobile
			,address1
			,address2
			,pincode
			,STATE
			,city
			,product_type
			,noc_available
			,bank_name
			,ifsc_code
			,account_holder_name
			,account_number
			,educational_qualification
			,select_average_premium
			,POSPBU
			,created_date
			,created_by
			,sourced_by
			,serviced_by
			,posp_source
			,general_training_start
			,general_training_end
			,life_insurance_training_start
			,life_insurance_training_end
			,exam_status
			,exam_start
			,exam_end
			,IIBUploadStatus
			,iib_upload_date
			,AgreementStatus
			,@CreatedBy
		FROM @TempTable
		
		-- EXEC  dbo.POSP_POSPMigrationUserCreation;
		COMMIT TRANSACTION @TransactionName;

		SELECT 'Insertion Successfull' AS ResponceMessage
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION @TransactionName;

		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC POSP_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList

		SELECT 'Insertion Failed' AS ResponceMessage
	END CATCH
END