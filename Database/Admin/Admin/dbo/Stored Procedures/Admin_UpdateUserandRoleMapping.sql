    
CREATE PROCEDURE [dbo].[Admin_UpdateUserandRoleMapping]       
(      
   @UserRoleMappingId VARCHAR (100),  
   @UserId varchar(100),    
   @UserName VARCHAR(50),      
   @EmpId  VARCHAR(50) ,      
   @MobileNo  VARCHAR(10),      
   @EmailId VARCHAR(50),      
   @Gender VARCHAR(10),      
   @DOB VARCHAR(20),     
   @ProfilePictureId VARCHAR(100),     
   @RoleTypeId varchar(100),    
   @BUId varchar(100) = NULL,      
   @RoleId  varchar(100),    
   @ReportingIdentityRoleId varchar(100),    
   @ReportingUserId Varchar(50),      
   @CategoryId varchar(100),      
   @CreatedBy VARCHAR(50),    
   @DocumentId VARCHAR(100) = NULL,    
   @IsActive bit,  
   @IsProfilePictureChange bit  
      
)      
AS      
BEGIN      
 BEGIN TRY      
  IF EXISTS(SELECT TOP 1 UserId FROM [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE UserId = @UserId)       
   BEGIN      
     print 'p1'
    IF NOT EXISTS(SELECT TOP 1 UserId FROM [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE MobileNo = @MobileNo AND EmailId = @EmailId)  
    BEGIN  
      UPDATE [HeroIdentity].[dbo].[Identity_User] SET   
     UserName = @UserName, EmpId = @EmpId, Gender = @Gender, DOB = @DOB, MobileNo = @MobileNo, EmailId = @EmailId, IsActive = @IsActive  , UpdatedOn= GETDATE() 
     WHERE UserId = @UserId  
    END  
    ELSE  
    BEGIN  
	print 'p2'
    UPDATE [HeroIdentity].[dbo].[Identity_User] SET   
     UserName = @UserName, EmpId = @EmpId, Gender = @Gender, DOB = @DOB, IsActive = @IsActive WHERE UserId = @UserId  
    END  
 END      
   
 IF NOT EXISTS(SELECT TOP 1 UserId from [HeroIdentity].[dbo].[Identity_UserDetail] WITH(NOLOCK) WHERE UserId = @UserId)  
 BEGIN 
 print 'hi1'
   INSERT INTO [HeroIdentity].[dbo].[Identity_UserDetail]   
   (UserId, ProfilePictureID, DocumentId) VALUES (@UserId, @ProfilePictureID, @DocumentId)   
 END  
 ELSE  
 BEGIN    
  print 'hi2'
   UPDATE [HeroIdentity].[dbo].[Identity_UserDetail]  SET   
   ProfilePictureID = @ProfilePictureID, DocumentId = @DocumentId   
   WHERE UserId = @UserId  
  
	UPDATE [dbo].[Admin_UserRoleMapping] SET  
	RoleId = @RoleId, ReportingUserId = @ReportingUserId, CategoryId = @CategoryId, BUId = @BUId, RoleTypeId = @RoleTypeId, ReportingIdentityRoleId = @ReportingIdentityRoleId, UpdatedBy = @CreatedBy, IsActive = @IsActive , UpdatedOn= GETDATE()
	WHERE UserRoleMappingId = @UserRoleMappingId AND UserId = @UserId  
   
   END   
 END TRY                      
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                   
 END CATCH      
      
END 
