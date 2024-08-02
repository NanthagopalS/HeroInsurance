-- exec Identity_InsertUserandRoleMapping 'Avinash Kiran','EMPL412','99862011046','Avinash.Kiran@gmail.com','Male','1980-01-03','2D6B0CE9-15C7-4839-93D1-8387944BC42F','test',1
-- exec Identity_InsertUserandRoleMapping 'mist','EMPL412','99862011046','mist@gmail.com','Male','1980-01-03','2D6B0CE9-15C7-4839-93D1-8387944BC42F','',0

-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,03-01-2023>
-- Description:	<Description,,INSERT Identity_User ,Identity_UserRoleMapping   DETAIL>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_InsertUserandRoleMapping] 
(
	@UserName VARCHAR(50) ,
	@EmpID  VARCHAR(50)  ,
	@MobileNo  VARCHAR(10)  ,
	@EmailId VARCHAR(50)   ,
	@Gender VARCHAR(10)   ,
	@DOB   VARCHAR(20)  ,
	@RoleId  VARCHAR(50)   ,
	@CreatedBy VARCHAR(50) ,
	@StatusUser bit = 1,

	@RoleTypeID int,	
	@IdentityRoleId int,
    @ReportingIdentityRoleId int,
    --@UserID Varchar(50),
	@ReportingUserID Varchar(50),
	@CategoryID int,
	@StatusRoleUser bit = 1
	

)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
		DECLARE @Identity_User table([UserId] [uniqueidentifier]);
		DECLARE @insertedUserId varchar(100);
		INSERT INTO [dbo].[Identity_User]
           (
            [UserName]
		   ,[EmpID]           
           ,[MobileNo]
		   ,[EmailId]           
           ,[Gender]
           ,[DOB]
           ,[RoleId]
           ,[CreatedBy]
           ,[isActive]
           )
		    OUTPUT INSERTED.[UserId] INTO @Identity_User
     VALUES
            (@UserName,
			@EmpID  ,
			@MobileNo  ,
			@EmailId   ,
			@Gender  ,
		    @DOB      ,		  
			@RoleId   ,
			@CreatedBy,
			@StatusUser)
		 IF EXISTS( SELECT [UserId] FROM @Identity_User)
			 BEGIN
			       SELECT @insertedUserId = [UserId] FROM @Identity_User;
			       print 'Current userid ' + @insertedUserId
					INSERT INTO [dbo].[Identity_UserRoleMapping]
					   (
						[RoleTypeID]		 
					   ,[IdentityRoleId] 
                       ,[ReportingIdentityRoleId]					  
					   ,[UserID]
					   ,[ReportingUserID]
					   ,[CategoryID]
					   ,[isActive]
					   )
					VALUES
					   (
						@RoleTypeID,
						@IdentityRoleId,
						@ReportingIdentityRoleId,
						@insertedUserId , --Current inserted userid
						@ReportingUserID ,
						@CategoryID ,
						@StatusRoleUser
					   )
			 END
		
	 	IF @@TRANCOUNT > 0
            COMMIT
	END TRY                
	BEGIN CATCH          
	IF @@TRANCOUNT > 0
        ROLLBACK  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
