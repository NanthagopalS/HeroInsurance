-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,,25-nov-2022>
-- Description:	<Description,[Insurance_GetRTO]>
--[Insurance_GetRTO]
-- =============================================
CREATE     PROCEDURE [dbo].[Insurance_GetRTO]
AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;
	
	Declare @minyear int
	Declare @maxyear int
	set @maxyear = (select max(year) from Insurance_Year)
	set @minyear  =(select max(year)-15 from insurance_year)

		SELECT CAST(YearId AS VARCHAR(50))YearId,Year
		FROM Insurance_Year WITH(NOLOCK) where year between @minyear and @maxyear
		ORDER BY [YEAR] DESC

		SELECT CAST(StateId AS VARCHAR(50))StateId,StateName
		FROM Insurance_State STATE WITH(NOLOCK)
		ORDER BY StateName

		SELECT CAST(CityId AS VARCHAR(50))CityId,CityName,StateId
		FROM Insurance_City WITH(NOLOCK)
		ORDER BY IsPopular desc,CityName

		SELECT DISTINCT CAST(RTO.RTOId AS VARCHAR(50))RTOId,RTOCode,ISNULL(CityId,'')CityId
		FROM Insurance_RTO RTO WITH(NOLOCK)
		ORDER BY RTOCode
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END


