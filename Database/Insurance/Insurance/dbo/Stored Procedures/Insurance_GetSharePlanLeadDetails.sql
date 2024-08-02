--[dbo].[Insurance_GetSharePlanLeadDetails] 'HERO/ENQ/107270'
CREATE   PROCEDURE [dbo].[Insurance_GetSharePlanLeadDetails]     
@LeadId VARCHAR(50) = NULL,
@UserId VARCHAR(50) = NULL,
@EmailId VARCHAR(50) = NULL,
@MobileNumber VARCHAR(50) = NULL,
@Link NVARCHAR(MAX) = NULL,
@Type VARCHAR(50) = NULL,
@Insurer VARCHAR(50) = NULL
AS      
BEGIN      
	BEGIN TRY      
		
		DECLARE @VariantId VARCHAR(50), @InsurerId VARCHAR(50), @InsurerName VARCHAR(100) = null, @PolicyNumber VARCHAR(100), @DocumentId VARCHAR(100), @TransactionId VARCHAR(50), @StageId VARCHAR(50), @URL NVARCHAR(MAX),  @QuotetransactionId VARCHAR(50), @TempURL NVARCHAR(MAX)



		SET @TransactionId = NEWID()

		SELECT  @VariantId = VariantId,@InsurerId = InsurerId, @QuotetransactionId = QuoteTransactionID
		FROM Insurance_LeadDetails WITH(NOLOCK) 
		WHERE LeadId = @LeadId
		IF @Type = 'QuoteConfirm'
			BEGIN
				UPDATE Insurance_LeadDetails SET StageId = 
				(SELECT StageID FROM Insurance_StageMaster WITH(NOLOCK) WHERE Stage = @Type), InsurerId = @Insurer
			END

		SELECT @StageId = StageID FROM Insurance_StageMaster WITH(NOLOCK) WHERE Stage = @Type

		IF @StageId = 'D07216AF-ACAD-4EEA-8CFF-2910BA77E5EE'
		BEGIN
			SET @TempURL = REPLACE(REPLACE(@Link,'InsurerId',@InsurerId), 'QuoteId',@QuotetransactionId)
			SET @URL = @TempURL + @StageId + '/' + @TransactionId + '?leadid='+ @LeadId
		END
		ELSE
		BEGIN
			SET @URL = @Link + @StageId + '/' + @TransactionId + '?leadid='+ @LeadId
		END

		INSERT INTO Insurance_SharePlanDetails (Id,UserId, LeadId, MobileNumber, EmailId, Type, Link, CreatedBy, StageId)
		VALUES (@TransactionId,@UserId,@LeadId,@MobileNumber,@EmailId,@Type,@URL,@UserId,@StageId)

		SELECT @PolicyNumber = PolicyNumber, @DocumentId = DocumentId
		FROM Insurance_PaymentTransaction Payment WITH(NOLOCK) 
        WHERE LeadId = @LeadId

		SELECT * FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadId

		SELECT @InsurerName = InsurerName from Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId

		SELECT MakeName, ModelName, VariantName, @InsurerName AS InsurerName, @PolicyNumber AS PolicyNumber, @DocumentId AS DocumentId,
		@URL AS Link, @TransactionId AS TransactionId
		FROM Insurance_Make MAKE WITH(NOLOCK)
		INNER JOIN Insurance_Model MODEL WITH(NOLOCK) ON MAKE.MakeId = MODEL.MakeId
		INNER JOIN Insurance_Variant VARIANT WITH(NOLOCK) ON MODEL.ModelId = VARIANT.ModelId
		WHERE VARIANT.VariantId = @VariantId
	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END