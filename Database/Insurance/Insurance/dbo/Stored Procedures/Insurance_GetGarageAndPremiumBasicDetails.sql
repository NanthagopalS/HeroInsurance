CREATE     PROCEDURE [dbo].[Insurance_GetGarageAndPremiumBasicDetails]
(
	@InsurerId VARCHAR(50)=null
)
AS
BEGIN
	BEGIN TRY

		SET NOCOUNT ON;

		SELECT GarageId,WorkshopName,FullAddress,City,State,Pincode,Latitude,Longitude,ProductType,EmailId,MobileNumber,ContactPerson
		FROM  Insurance_CashlessGarage WITH(NOLOCK) 
		WHERE IsActive=1

		SELECT Id AS PremiumBasicDetailsId,Title 
		FROM Insurance_PremiumBasicDetail WITH(NOLOCK) 

		SELECT Subtitle, Description, Icon
		FROM Insurance_PremiumBasicSubtitleDetail WITH(NOLOCK)  

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END