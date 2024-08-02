CREATE PROCEDURE [dbo].[Insurance_GetICICIMasterMapping]
(
@regNo varchar(50),
@userId varchar(50)
)
AS
begin
	BEGIN TRY
		SET NOCOUNT ON;
			SELECT Engine, regNo, chassis, vehicleManufacturingMonthYear, regDate, owner FROM Insurance_VehicleRegistration WITH(NOLOCK) where regNo = @regNo

			SELECT UserName FROM Identity_User WITH(NOLOCK) where userId = @userId
	END TRY          
	BEGIN CATCH         
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END