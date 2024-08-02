------------------------------------------------------  
/*Create By    : Parth   
  Created Date : 10,May-2023  
  Description  : To Get ALL POSP ID  
*/  
-------------------------------------------------------  
CREATE   PROCEDURE [dbo].[Insurance_Job_GetPospId]        
AS      
BEGIN       
 BEGIN TRY      
	SELECT TOP 10 iUser.POSPId AS PospId, 
		   detail.AadhaarNumber AS AadharNumber,
		   iUser.EmailId AS EmailId,
		   iUser.MobileNo AS MobileNumber,
		   iUser.UserName AS Name,
		   detail.PAN AS PanNumber,
		   stateName.StateName AS State
	FROM [HeroIdentity].[dbo].[Identity_User] iUser WITH (NOLOCK)
	LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] detail WITH (NOLOCK) ON detail.UserId = iUser.UserId 
	LEFT JOIN [HeroIdentity].[dbo].[Identity_UserAddressDetail] addressDetail WITH (NOLOCK) ON addressDetail.UserId = iUser.UserId 
	LEFT JOIN [HeroIdentity].[dbo].[Identity_State] stateName WITH (NOLOCK) ON stateName.StateId = addressDetail.StateId
	 WHERE iUser.IsActive = 1 AND iUser.POSPId IS NOT NULL AND AadhaarNumber IS NOT NULL AND EmailId IS NOT NULL
	 AND MobileNo IS NOT NULL AND UserName IS NOT NULL AND PAN IS NOT NULL AND StateName.StateName IS NOT NULL AND iUser.HDFCPospId IS NULL 
 END TRY              
BEGIN CATCH                        
                     
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                          
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                          
  SET @ErrorDetail=ERROR_MESSAGE()                                          
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                           
 END CATCH         
END