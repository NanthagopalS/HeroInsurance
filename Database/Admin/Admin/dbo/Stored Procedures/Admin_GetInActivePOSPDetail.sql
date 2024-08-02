-- =========================================================================================           
-- Author:  <Author, Parth>        
-- Create date: <Create Date,31-Jan-2023>        
-- Description: <Description, Admin_GetInActivePOSPDetail>     
-- exec Admin_GetInActivePOSPDetail null,'2BA70571-63B3-4193-9F31-BDBCC59ED08B',null,null,1   
-- =========================================================================================           
 CREATE PROCEDURE [dbo].[Admin_GetInActivePOSPDetail]         
 (        
   @CriteriaType VARCHAR(100), --Login Activity / Policy Issuance  
   @FromDate VARCHAR(100),    
   @ToDate VARCHAR(100),    
   @PageIndex INT = 1       
 )        
AS        
 DECLARE @RowsOfPage  INT  
 SET @RowsOfPage = 10  
  
BEGIN        
 BEGIN TRY        
      
 IF(@CriteriaType = 'Login Activity')  
 BEGIN  
  IF(@PageIndex = -1)
  BEGIN
	SELECT IU.POSPId, IU.UserName, IU.MobileNo as MobileNumber, UR.ReportingIdentityRoleId as ReportingManager, '-' as PolicySold, '-' as Premium  
	FROM [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK)
	INNER JOIN [HeroIdentity].[dbo].[Identity_OTP] IOTP WITH(NOLOCK) ON IU.UserId = IOTP.UserId  
	INNER JOIN Admin_UserRoleMapping UR WITH(NOLOCK) ON IU.UserId = UR.UserId  
	WHERE IU.IsActive = 1 AND IOTP.IsActive = 1 AND UR.IsActive = 1 AND DATEDIFF(MINUTE,IOTP.OTPSendDateTime,GETDATE()) > 5  
	ORDER BY IU.UserName 
  END
  ELSE
  BEGIN
	SELECT IU.POSPId, IU.UserName, IU.MobileNo as MobileNumber, UR.ReportingIdentityRoleId as ReportingManager, '-' as PolicySold, '-' as Premium  
	FROM [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK) 
	INNER JOIN [HeroIdentity].[dbo].[Identity_OTP] IOTP WITH(NOLOCK) ON IU.UserId = IOTP.UserId  
	INNER JOIN Admin_UserRoleMapping UR WITH(NOLOCK) ON IU.UserId = UR.UserId  
	WHERE IU.IsActive = 1 AND IOTP.IsActive = 1 AND UR.IsActive = 1 AND DATEDIFF(MINUTE,IOTP.OTPSendDateTime,GETDATE()) > 5  
	ORDER BY IU.UserName OFFSET (@PageIndex-1)*@RowsOfPage ROWS FETCH NEXT @RowsOfPage ROWS ONLY  
  END
  
 END  
 ELSE IF(@CriteriaType = 'Policy Issuance')  
 BEGIN  
   --Pending.... Original Query Pending.....  
  IF(@PageIndex = -1)
  BEGIN
		SELECT IU.POSPId, IU.UserName, IU.MobileNo as MobileNumber, UR.ReportingIdentityRoleId as ReportingManager, '-' as PolicySold, '-' as Premium  
		FROM [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK) 
		INNER JOIN [HeroIdentity].[dbo].[Identity_OTP] IOTP WITH(NOLOCK) ON IU.UserId = IOTP.UserId  
		INNER JOIN Admin_UserRoleMapping UR WITH(NOLOCK) ON IU.UserId = UR.UserId  
		WHERE IU.IsActive = 1 AND IOTP.IsActive = 1 AND UR.IsActive = 1 AND DATEDIFF(MINUTE,IOTP.OTPSendDateTime,GETDATE()) > 5  
		ORDER BY IU.UserName 
  END
  ELSE
  BEGIN
	SELECT IU.POSPId, IU.UserName, IU.MobileNo as MobileNumber, UR.ReportingIdentityRoleId as ReportingManager, '-' as PolicySold, '-' as Premium  
  FROM [HeroIdentity].[dbo].[Identity_User] IU  WITH(NOLOCK)
   INNER JOIN [HeroIdentity].[dbo].[Identity_OTP] IOTP WITH(NOLOCK) ON IU.UserId = IOTP.UserId  
   INNER JOIN Admin_UserRoleMapping UR WITH(NOLOCK) ON IU.UserId = UR.UserId  
  WHERE IU.IsActive = 1 AND IOTP.IsActive = 1 AND UR.IsActive = 1 AND DATEDIFF(MINUTE,IOTP.OTPSendDateTime,GETDATE()) > 5  
  ORDER BY IU.UserName OFFSET (@PageIndex-1)*@RowsOfPage ROWS FETCH NEXT @RowsOfPage ROWS ONLY  
  END
  
 END  
   
 END TRY                        
 BEGIN CATCH          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
     
END 
