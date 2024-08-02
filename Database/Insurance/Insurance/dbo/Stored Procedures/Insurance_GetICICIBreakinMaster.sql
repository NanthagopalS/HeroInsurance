CREATE PROCEDURE [dbo].[Insurance_GetICICIBreakinMaster]
@CityCode VARCHAR(50)
AS
begin
	BEGIN TRY
		SET NOCOUNT ON;
		SELECT GST_STATE State, TXT_CITYDISTRICT City FROM MOTOR.ICICI_City_Master WITH(NOLOCK) where IL_CITYDISTRICT_CD = @CityCode
	END TRY          
	BEGIN CATCH         
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END