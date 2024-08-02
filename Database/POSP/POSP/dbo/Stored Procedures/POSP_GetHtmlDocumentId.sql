-- =============================================
-- Author: <Author,Harsh Patel>
-- Create date: <Create Date, 24-01-2022>
-- Description:	<Description,POSP_GetHtmlDocumentId>
-- =============================================
CREATE     PROCEDURE [dbo].[POSP_GetHtmlDocumentId]
(
@ConfigurationKey varchar(50) -- POSPAgreementFormat, ExamCertificate
)
AS
BEGIN
	BEGIN TRY
		
		SELECT ConfigurationValue FROM [HeroIdentity].[dbo].[Identity_Configuration] WITH(NOLOCK)  WHERE ConfigurationKey = @ConfigurationKey AND IsActive = 1
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
	END CATCH
END
