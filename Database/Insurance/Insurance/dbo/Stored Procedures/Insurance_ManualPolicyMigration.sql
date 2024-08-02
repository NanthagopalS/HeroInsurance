
-- select * from Insurance_PolicyDumpTable     
-- delete  from Insurance_PolicyDumpTable       
CREATE PROCEDURE [dbo].[Insurance_ManualPolicyMigration] (
	@TempTable AS dbo.PolicyDumpTableType READONLY
	,@CreatedBy NVARCHAR(50) = NULL
	)
AS
BEGIN
	DECLARE @TransactionName NVARCHAR(20) = 'DUMPPOSPTransaction';

	BEGIN TRANSACTION @TransactionName;

	BEGIN TRY
		INSERT INTO Insurance_PolicyDumpTable (
			UserEmail
			,MotorType
			,PolicyType
			,PolicyCategory
			,BasicOD
			,BasicTP
			,TotalPremium
			,NetPremium
			,ServiceTax
			,PolicyNo
			,EngineNo
			,ChasisNo
			,VehicleNo
			,IDV
			,Insurer
			,Make
			,Fuel
			,Variant
			,ManufacturingMonth
			,CustomerName
			,PolicyIssueDate
			,PolicyStartDate
			,PolicyEndDate
			,BusinessType
			,NCB
			,ChequeNo
			,ChequeDate
			,ChequeBank
			,CustomerEmail
			,CustomerMobile
			,ManufacturingYear
			,PreviousNCB
			,CubicCapacity
			,RTOCode
			,PreviousPolicyNo
			,CPA
			,[Period]
			,InsuranceType
			,AddOnPremium
			,NilDep
			,IsPOSPProduct
			,CustomerAddress
			,[STATE]
			,City
			,PhoneNo
			,PinCode
			,CustomerDOB
			,PANNo
			,GrossDiscount
			,TotalTP
			,GVW
			,SeatingCapacity
			,UserId
			)
		SELECT UserEmail
			,MotorType
			,PolicyType
			,PolicyCategory
			,BasicOD
			,BasicTP
			,TotalPremium
			,NetPremium
			,ServiceTax
			,PolicyNo
			,EngineNo
			,ChasisNo
			,VehicleNo
			,IDV
			,Insurer
			,Make
			,Fuel
			,Variant
			,ManufacturingMonth
			,CustomerName
			,PolicyIssueDate
			,PolicyStartDate
			,PolicyEndDate
			,BusinessType
			,NCB
			,ChequeNo
			,ChequeDate
			,ChequeBank
			,CustomerEmail
			,CustomerMobile
			,ManufacturingYear
			,PreviousNCB
			,CubicCapacity
			,RTOCode
			,PreviousPolicyNo
			,CPA
			,[Period]
			,InsuranceType
			,AddOnPremium
			,NilDep
			,IsPOSPProduct
			,CustomerAddress
			,[STATE]
			,City
			,PhoneNo
			,PinCode
			,CustomerDOB
			,PANNo
			,GrossDiscount
			,TotalTP
			,GVW
			,SeatingCapacity
			,@CreatedBy
		FROM @TempTable
		WHERE Insurer IS NOT NULL
			AND Insurer IS NOT NULL

		EXEC dbo.Insurance_ManualPolicyValidation @CreatedBy;

		EXEC dbo.Insurance_ManualPolicyCreation @CreatedBy;

		COMMIT TRANSACTION @TransactionName;

		SELECT *
		INTO 
		#Insurance_PolicyDumpTable
		FROM Insurance_PolicyDumpTable
		WHERE UserId = @CreatedBy

		DECLARE @TotalRecords INT = (
				SELECT COUNT(*)
				FROM #Insurance_PolicyDumpTable
				WHERE UserId = @CreatedBy
				)
		DECLARE @FailedRecords INT = (
				SELECT COUNT(*)
				FROM #Insurance_PolicyDumpTable
				WHERE GeneratedLeadId IS NULL
				)
		DECLARE @SuccessRecords INT = (
				SELECT COUNT(*)
				FROM #Insurance_PolicyDumpTable
				WHERE GeneratedLeadId IS NOT NULL
				)

		SELECT @TotalRecords AS totalpolicy
			,@FailedRecords AS policyfailed
			,@SuccessRecords AS policyuploadedsuccessful
			,1 AS isUploaded

	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION @TransactionName;

		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList

			SELECT 0 AS totalpolicy
				,0 AS policyfailed
				,0 AS policyuploadedsuccessful
				,0 AS isUploaded
				END CATCH
END