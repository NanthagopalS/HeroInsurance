 --[dbo].[Insurance_GetProposalDetailsForPayment]  '0A326B77-AFD5-44DA-9871-1742624CFF16','2837A9EB-85E9-4141-B793-8A7AFB871024'    
  
CREATE       procedure [dbo].[Insurance_GetProposalDetailsForPayment]      
@InsurerId VARCHAR(50),    
@QuoteTransactionId VARCHAR(50)    
AS    
BEGIN     
	DECLARE @VehicleTypeId VARCHAR(50), @RequestBody NVARCHAR(MAX), @LeadId VARCHAR(50)
    BEGIN TRY    
		SELECT @RequestBody = RequestBody, @LeadId = LeadId    
		FROM Insurance_QuoteTransaction WITH(NOLOCK)    
		WHERE InsurerId=@InsurerId AND QuoteTransactionId=@QuoteTransactionId
		
		SELECT @VehicleTypeId = VehicleTypeId FROM Insurance_LeadDetails 
		WHERE LeadId = @LeadId

		SELECT @RequestBody RequestBody, @VehicleTypeId VehicleTypeId
	END TRY          
	BEGIN CATCH                     
		 DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
		SET @ErrorDetail=ERROR_MESSAGE()                                      
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                  
	END CATCH           
END