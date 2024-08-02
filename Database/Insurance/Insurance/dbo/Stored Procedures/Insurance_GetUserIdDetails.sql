 --[dbo].[Insurance_GetProposalDetails]  '0788a6e1-240d-47bf-9473-992a1a33b9e5'
CREATE   procedure [dbo].[Insurance_GetUserIdDetails]  
@InsurerId VARCHAR(50),
@ProposalNumber VARCHAR(50)
AS
BEGIN 
  BEGIN TRY
	SELECT CreatedBy
	FROM Insurance_QuoteTransaction WITH(NOLOCK)
	WHERE InsurerId=@InsurerId AND TransactionId=@ProposalNumber
 END TRY      
 BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH    

END