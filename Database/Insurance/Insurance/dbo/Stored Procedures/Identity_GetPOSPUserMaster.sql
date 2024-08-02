-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,,25-Nov-2022>
-- Description:	<Description,[GetInsurer]>
--[Insurance_GetInsurer]
-- =============================================
CREATE       PROCEDURE [dbo].[Identity_GetPOSPUserMaster]
AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		SELECT Id BackgroundTypeId,BackgroundType FROM Identity_BackgroundTypeMaster WITH(NOLOCK) ORDER BY BackgroundType
		SELECT Id InsurerCompanyId,InsurerCompanyName FROM Identity_InsurerCompanyMaster WITH(NOLOCK) ORDER BY InsurerCompanyName
		SELECT Id POSPSourceTypeId,POSPSourceType FROM Identity_POSPSourceTypeMaster WITH(NOLOCK) ORDER BY POSPSourceType
		SELECT Id PremiumRangeTypeId,PremiumRangeType FROM Identity_PremiumRangeTypeMaster WITH(NOLOCK) ORDER BY PremiumRangeType
		SELECT CityId,CityName FROM Identity_City  WITH(NOLOCK) ORDER BY CityName
		SELECT StateId,StateName FROM Identity_State  WITH(NOLOCK) ORDER BY StateName
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END

