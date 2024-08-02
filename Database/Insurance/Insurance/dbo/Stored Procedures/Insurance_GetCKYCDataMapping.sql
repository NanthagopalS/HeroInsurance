-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--[Insurance_GetCKYCFields]'16413879-6316-4C1E-93A4-FF8318B14D37',0,0
-- =============================================
CREATE PROCEDURE [dbo].[Insurance_GetCKYCDataMapping]  
@QuoteTransactionId varchar(50) 
AS
BEGIN 
BEGIN TRY
	SELECT TransactionId, LeadId
	FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuoteTransactionId
END TRY   
BEGIN CATCH         
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
END CATCH
END
