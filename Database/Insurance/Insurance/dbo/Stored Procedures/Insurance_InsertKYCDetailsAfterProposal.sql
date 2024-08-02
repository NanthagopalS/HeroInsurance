-- =============================================  
-- Author:  <Firoz S>  
-- Create date: <12-10-2023>  
-- Description: <To Insert and Update KYC Details, For IC where KYC has to perform after Proposal> 
-- =============================================  
CREATE PROCEDURE [dbo].[Insurance_InsertKYCDetailsAfterProposal]    
@LeadId VARCHAR(50) = NULL,
@InsurerId VARCHAR(50) = NULL,
@QuoteTransactionId VARCHAR(50) = NULL,
@RequestBody NVARCHAR(MAX) = NULL,
@ResponseBody NVARCHAR(MAX) = NULL,
@PhotoId VARCHAR(MAX) = NULL,
@Stage VARCHAR(10) = NULL,
@KYCId VARCHAR(20) = NULL,
@CKYCNumber VARCHAR(20) = NULL,
@CKYCStatus VARCHAR(20) = NULL,
@UserId VARCHAR(50) = NULL
AS      
BEGIN
	BEGIN TRY
		
		INSERT INTO Insurance_CKYCTransaction(InsurerId, QuoteTransactionId, LeadId, CKYCRequestBody, CKYCResponseBody, PhotoId, Stage, KYCId, CKYCNumber, CKYCStatus, CreatedBy, CreatedOn)
		VALUES(@InsurerId, @QuoteTransactionId, @LeadId, @RequestBody, @ResponseBody, @PhotoId, @Stage, @KYCId, @CKYCNumber, @CKYCStatus, @UserId, GETDATE())
		
		SELECT InsurerId, QuoteTransactionId
		FROM Insurance_CKYCTransaction WITH(NOLOCK) WHERE LeadId = @LeadId

	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END