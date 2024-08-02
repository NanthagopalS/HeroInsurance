
CREATE PROCEDURE [dbo].[Identity_GetPOSPConfigurationDetail]
AS
BEGIN
	BEGIN TRY

		  SELECT Id, ConfigurationKey, ConfigurationValue, ConfigurationLable, ConfigurationIcon, IsActive 
		  FROM Identity_Configuration WITH(NOLOCK)
		  WHERE ConfigurationKey in ('SupportTime', 'SupportEmail', 'SupportNumber', 'Headquarter Address')
		  order by ConfigurationKey
	
	END TRY                
	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
