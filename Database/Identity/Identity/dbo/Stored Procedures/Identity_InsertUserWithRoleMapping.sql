    
CREATE   PROCEDURE [dbo].[Identity_InsertUserWithRoleMapping]     
(  
  @UserName VARCHAR(100),    
  @EmpId  VARCHAR(100) ,    
  @MobileNo  VARCHAR(10),    
  @EmailId VARCHAR(100),    
  @Gender VARCHAR(10),    
  @DOB   VARCHAR(20),   
  @ProfilePictureId VARCHAR(100), ----------  
  @RoleTypeId varchar(100),  
  @BUId varchar(100) = NULL,    
  @RoleId  varchar(100),  
  @ReportingIdentityRoleId varchar(100),  
  @ReportingUserId VARCHAR(100),    
  @CategoryId varchar(100),    
  @CreatedBy VARCHAR(100),  
  @DocumentId VARCHAR(100) = NULL,
  @Password VARCHAR(MAX) = NULL
)  
AS    
BEGIN    
 BEGIN TRY    
   
 DECLARE @UserId VARCHAR(100) = NULL, @RoleId1 VARCHAR(100) = NULL  
  
 SET @RoleId1 = (SELECT RoleId FROM [Identity_RoleMaster] WITH(NOLOCK) WHERE RoleName = 'Back Office')  
  
  
 IF NOT EXISTS(SELECT TOP 1 UserId from [Identity_User] WITH(NOLOCK) WHERE EmailId = @EmailId AND MobileNo = @MobileNo AND IsActive = 1)  
 BEGIN    
   INSERT INTO [Identity_User] (UserName, EmpId, MobileNo, EmailId, Gender, DOB, RoleId,Password)       
   VALUES (@UserName, @EmpId, @MobileNo, @EmailId, @Gender, @DOB, @RoleId1,@Password)  
 END  
 ELSE  
 BEGIN  
  UPDATE [Identity_User] SET   
  UserName = @UserName, EmpId = @EmpId, Gender = @Gender, DOB = @DOB, RoleId = @RoleId1, UpdatedOn = GETDATE() WHERE MobileNo = @MobileNo AND EmailId = @EmailId AND IsActive = 1  
 END  
  
  
 SET @UserId = (SELECT TOP 1 UserId FROM [Identity_User] WITH(NOLOCK) WHERE EmailId = @EmailId AND MobileNo = @MobileNo AND IsActive = 1)  
  
  
  
 IF NOT EXISTS(SELECT TOP 1 UserId from [Identity_UserDetail] WITH(NOLOCK) WHERE UserId = @UserId AND IsActive = 1)  
 BEGIN  
   INSERT INTO [Identity_UserDetail]   
   (UserId, ProfilePictureID, DocumentId) VALUES (@UserId, @ProfilePictureID, @DocumentId)   
 END  
 ELSE  
 BEGIN  
    
   UPDATE [Identity_UserDetail]  SET   
   ProfilePictureID = @ProfilePictureID, DocumentId = @DocumentId, UpdatedOn = GETDATE()   
   WHERE UserId = @UserId  
 END  
   
  
  
 IF NOT EXISTS(SELECT TOP 1 UserId from [HeroAdmin].[dbo].[Admin_UserRoleMapping] WITH(NOLOCK) WHERE UserId = @UserId AND IsActive = 1)  
 BEGIN  
   INSERT INTO [HeroAdmin].[dbo].[Admin_UserRoleMapping]   
   (UserId, RoleId, ReportingUserId, CategoryId, BUId, RoleTypeId, ReportingIdentityRoleId, IsActive, CreatedBy) VALUES   
   (@UserId, @RoleId, @ReportingUserId, @CategoryId, @BUId, @RoleTypeId, @ReportingIdentityRoleId, 1, @CreatedBy)  
 END  
 ELSE  
 BEGIN  
    
  UPDATE [HeroAdmin].[dbo].[Admin_UserRoleMapping] SET  
  RoleId = @RoleId, ReportingUserId = @ReportingUserId, CategoryId = @CategoryId, BUId = @BUId, RoleTypeId = @RoleTypeId, ReportingIdentityRoleId = @ReportingIdentityRoleId, UpdatedOn = GETDATE()  
  WHERE UserId = @UserId AND IsActive = 1  
     
 END  
    
 END TRY                    
 BEGIN CATCH              
   DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
   SET @StrProcedure_Name=ERROR_PROCEDURE()                                
   SET @ErrorDetail=ERROR_MESSAGE()                                
   EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList     
     
 END CATCH    
    
END