CREATE PROCEDURE [dbo].[Insurance_UpdateGodigitPolicyDetails]     
@QuoteTransactionID VARCHAR(50) = NULL,
@PolicyNumber VARCHAR(50) = NULL,
@PaymentLink NVARCHAR(MAX) = NULL,
@Stage VARCHAR(50) = NULL,
@BreakInStatus Bit = NULL
AS      
BEGIN      
	BEGIN TRY      
		
		UPDATE Insurance_LeadDetails SET PolicyNumber = @PolicyNumber,
		PaymentLink = @PaymentLink,
		StageId = (SELECT StageID FROM Insurance_StageMaster WITH(NOLOCK) WHERE Stage = @Stage),
		IsBreakinApproved = @BreakInStatus, UpdatedOn = GETDATE()
		WHERE QuoteTransactionId=@QuoteTransactionID

		SELECT LeadId FROM Insurance_LeadDetails WITH(NOLOCK) WHERE QuoteTransactionId=@QuoteTransactionID

	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END 