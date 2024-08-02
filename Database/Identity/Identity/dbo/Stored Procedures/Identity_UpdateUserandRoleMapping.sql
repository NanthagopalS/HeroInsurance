-- exec Identity_InsertUserandRoleMapping 'Avinash Kiran','EMPL412','99862011046','Avinash.Kiran@gmail.com','Male','1980-01-03','2D6B0CE9-15C7-4839-93D1-8387944BC42F','test',1
-- exec Identity_InsertUserandRoleMapping 'mist','EMPL412','99862011046','mist@gmail.com','Male','1980-01-03','2D6B0CE9-15C7-4839-93D1-8387944BC42F','',0

-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,03-01-2023>
-- Description:	<Description,,INSERT Identity_User ,Identity_UserRoleMapping   DETAIL>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_UpdateUserandRoleMapping] 
(
	@UserID Varchar(50),
	@UserName VARCHAR(50) ,
	@EmpID  VARCHAR(50)  ,
	@MobileNo  VARCHAR(10)  ,
	@EmailId VARCHAR(50)   ,
	@Gender VARCHAR(10)   ,
	@DOB   VARCHAR(20)  ,
	@RoleId  VARCHAR(50)   ,
	@updatedBy VARCHAR(50) ,
	@isActive bit ,

	@UserRoleID int,
	@RoleTypeID int,
	@IdentityRoleId int,
    @ReportingIdentityRoleId int,
	@ReportingUserID Varchar(50),
	@CategoryID int
	--,@StatusRoleUser bit 

)
AS
BEGIN
	BEGIN TRY
	IF EXISTS(SELECT DISTINCT UserId FROM [dbo].[Identity_User] WITH(NOLOCK) WHERE UserId = @UserID)  
	 BEGIN
	 UPDATE [dbo].[Identity_User] SET 
		
            [UserName]  = @UserName
		   ,[EmpID] =  @EmpID        
           ,[MobileNo]= @MobileNo
		   ,[EmailId] = @EmailId           
           ,[Gender] = @Gender
           ,[DOB]  =@DOB
           ,[RoleId] =@RoleId
           ,[updatedBy]= @updatedBy
		   ,[UpdatedOn]=getDate()
           ,[isActive]=@isActive
		    WHERE UserID=@UserID
	END
		IF EXISTS(SELECT DISTINCT UserId,UserRoleID FROM [dbo].[Identity_UserRoleMapping] WITH(NOLOCK) WHERE UserId = @UserId and UserRoleID=@UserRoleID)  
			 BEGIN			      
					UPDATE  [dbo].[Identity_UserRoleMapping] SET					
						[RoleTypeID]= @RoleTypeID	 
					    ,[IdentityRoleId]=@IdentityRoleId
                       , [ReportingIdentityRoleId]=@ReportingIdentityRoleId
					   ,[UserID] = @UserID
					   ,[ReportingUserID] =@ReportingUserID
					   ,[CategoryID] =@CategoryID,
					    [updatedBy]= @updatedBy,
		                [UpdatedOn]=getDate()
					   WHERE UserId = @UserId and UserRoleID=@UserRoleID			 
			 END
	END TRY                
	BEGIN CATCH        
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
