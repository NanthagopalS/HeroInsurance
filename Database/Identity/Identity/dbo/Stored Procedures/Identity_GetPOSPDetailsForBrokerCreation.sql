
-- EXEC  [dbo].[Identity_GetPOSPDetailsForBrokerCreation] 'POS/HERO/10009'

-- =============================================
-- Author:		<Author,,Nanthagopal >
-- Create date: <Create Date,,17/04/2023>
-- Description:	<Description,GetPOSPDetails>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_GetPOSPDetailsForBrokerCreation]
@POSPId VARCHAR(100) = NULL
AS
BEGIN
   BEGIN TRY  
		SELECT 
		USERS.EmailId,
		USERS.UserName,
		USERS.MobileNo,
		USERS.POSPId,
		USERDETAIL.Gender,
		USERDETAIL.PAN,
		USERDETAIL.AadhaarNumber,
		USERDETAIL.DateofBirth,
		USERADDRESS.AddressLine1,
		USERADDRESS.AddressLine2,
		USERADDRESS.Pincode,
		CITY.CityName,
		USERSTATE.StateName
		FROM Identity_User USERS WITH(NOLOCK)
		LEFT JOIN Identity_UserDetail  USERDETAIL WITH(NOLOCK) ON USERDETAIL.UserId = USERS.UserId
		LEFT JOIN Identity_UserAddressDetail USERADDRESS WITH(NOLOCK) ON USERS.UserId = USERADDRESS.UserId
		LEFT JOIN Identity_City CITY WITH(NOLOCK) ON CITY.CityId = USERADDRESS.CityId
		LEFT JOIN Identity_State USERSTATE WITH(NOLOCK) ON USERSTATE.StateId = USERADDRESS.StateId
		WHERE POSPId = @POSPId
   END TRY  
   BEGIN CATCH  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
