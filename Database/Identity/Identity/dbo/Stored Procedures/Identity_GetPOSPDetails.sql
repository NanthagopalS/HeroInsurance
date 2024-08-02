           
-- EXEC [dbo].[Identity_GetPOSPDetails] '0340F56C-02C3-4B02-9A21-E2AAA771D7AA'
CREATE PROCEDURE [dbo].[Identity_GetPOSPDetails]      
(      
 @UserIdInput VARCHAR(100) = NULL      
)      
AS      
BEGIN       
 BEGIN TRY      
     
  SET NOCOUNT ON;         
      
  SELECT USERTB.UserId, 
  ISNULL(USERTB.UserName,'')UserName, 
  ISNULL(USERTB.EmailId,'')EmailId, 
  ISNULL(USERTB.MobileNo,'')MobileNo,
  ISNULL(USERTB.POSPId,'')POSPId,
  ISNULL(USERINFO.AadhaarNumber,'')AadhaarNumber, 
  ISNULL(USERINFO.PAN,'')PAN,
  ISNULL(USERINFO.DateofBirth,'')DateofBirth,
  ISNULL(CITY.CityName,'')CityName,
  ISNULL(ROL.ROLENAME,'') RoleName
  FROM Identity_User USERTB WITH(NOLOCK)
  LEFT JOIN Identity_UserDetail USERINFO WITH(NOLOCK) ON USERINFO.UserId = USERTB.UserId      
  LEFT JOIN Identity_UserAddressDetail ADDRESSTB WITH(NOLOCK) ON ADDRESSTB.UserId = USERTB.UserId   
  LEFT JOIN Identity_City CITY WITH(NOLOCK) ON CITY.CityId = ADDRESSTB.CityId
  LEFT JOIN Identity_RoleMaster ROL WITH(NOLOCK) ON USERTB.RoleId= ROL.RoleId
  WHERE USERTB.UserId = @UserIdInput and USERTB.IsActive = 1

 END TRY                      
 BEGIN CATCH                
             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
END 
