CREATE     PROCEDURE [dbo].[Insurance_GetCholaBreakinNotificationDetails]
@QuoteTransactionId VARCHAR(50) NULL  
AS
BEGIN
	BEGIN TRY

		SELECT BreakinId,MAKE.MakeName MakeName, MODEL.ModelName ModelName 
		FROM Insurance_LeadDetails Leads
		INNER JOIN Insurance_Variant VARIANT WITH(NOLOCK) ON Leads.VariantId = VARIANT.VariantId
		INNER JOIN Insurance_Model MODEL WITH(NOLOCK) ON MODEL.ModelId = VARIANT.ModelId
		INNER JOIN Insurance_Make MAKE  WITH(NOLOCK) ON MAKE.MakeId = MODEL.MakeId
		WHERE QuoteTransactionID = @QuoteTransactionId AND InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA' 
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END