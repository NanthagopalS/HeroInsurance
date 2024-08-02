
CREATE   PROCEDURE [dbo].[Admin_UpdateUserandRoleMappingDetail]   
(  
 @UserID varchar(100),
 @UserName VARCHAR(50) ,  
 @EmpID  VARCHAR(50)  ,  
 @MobileNo  VARCHAR(10)  ,  
 @EmailId VARCHAR(50)   ,  
 @Gender VARCHAR(10)   ,  
 @DOB   VARCHAR(20)  ,  
 @RoleId  varchar(100),
 @UserRoleID varchar(100),
 @CreatedBy VARCHAR(50) ,
 @updatedBy VARCHAR(50) ,  
 @isActive bit ,
 @StatusUser bit = 1,  
  
 @RoleTypeID varchar(100),
 @IdentityRoleId varchar(100),
    @ReportingIdentityRoleId varchar(100),
 @ReportingUserID Varchar(50),  
 @CategoryID varchar(100),  
 @StatusRoleUser bit = 1  

 --@UserName VARCHAR(50) ,  
 --@EmpID  VARCHAR(50)  ,  
 --@MobileNo  VARCHAR(10)  ,  
 --@EmailId VARCHAR(50)   ,  
 --@Gender VARCHAR(10)   ,  
 --@DOB   VARCHAR(20)  ,  
 --@RoleId  varchar(100),
 --@CreatedBy VARCHAR(50) , 
 --@updatedBy VARCHAR(50) ,  
 --@isActive bit ,  
  
 --@UserRoleID varchar(100),
 --@RoleTypeID varchar(100),
 --@IdentityRoleId varchar(100),
 --@ReportingIdentityRoleId varchar(100),
 --@ReportingUserID varchar(100),
 --@CategoryID varchar(100),
 --@StatusUser bit = 1,
 --@StatusRoleUser bit = 1
 --@StatusRoleUser bit   
  
)  
AS  
BEGIN  
 BEGIN TRY  
 IF EXISTS(SELECT DISTINCT UserId FROM [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE UserId = @UserID)    
  BEGIN  
  UPDATE [HeroIdentity].[dbo].[Identity_User] SET  
    
            [UserName]  = @UserName  
     ,[EmpID] =  @EmpID          
           ,[MobileNo]= @MobileNo  
     ,[EmailId] = @EmailId             
           ,[Gender] = @Gender  
           ,[DOB]  =@DOB  
           ,[RoleId] =@RoleId  
		   ,[CreatedBy] = @CreatedBy
          ,[isActive]=@isActive 
           ,[updatedBy]= @updatedBy  
     ,[UpdatedOn]=getDate()  
            
      WHERE UserID=@UserID  
 END  
  IF EXISTS(SELECT DISTINCT UserId, RoleID as UserRoleId FROM [dbo].[Admin_UserRoleMapping] WITH(NOLOCK) WHERE UserId = @UserId and RoleID = @UserRoleID)    
    BEGIN           
     UPDATE  [dbo].[Admin_UserRoleMapping] SET       
      [RoleTypeID]= @RoleTypeID    
         ,[IdentityRoleId]=@IdentityRoleId  
                       , [ReportingIdentityRoleId]=@ReportingIdentityRoleId  
        ,[UserID] = @UserID  
        ,[ReportingUserID] =@ReportingUserID  
        ,[CategoryID] =@CategoryID  
		 ,[CreatedBy] = @CreatedBy
         ,[updatedBy]= @updatedBy,  
                  [UpdatedOn]=getDate()  
        WHERE UserId = @UserId and RoleID=@UserRoleID      
    END  
 END TRY                  
 BEGIN CATCH          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END  
