-- =========================================================================================           
-- Author:  <Author, Ankit>        
-- Create date: <Create Date,2-Feb-2023>        
-- Description: <Description, Admin_CheckUserExistOrNot>  
-- =========================================================================================           
 CREATE   PROCEDURE [dbo].[Admin_CheckUserExistOrNot]         
 (        
	  @EmpId VARCHAR(100) = NULL,
	  @MobileNo VARCHAR(100) = NULL,
	  @EmailId VARCHAR(100) = NULL,
	  @UserId VARCHAR(100) = NULL
 )        
AS 
	DECLARE @IsUserExists bit = 0, 
			@IsEmpIdExists bit = 0, 
			@IsMobileNoExists bit = 0,
			@IsEmailIdExists bit = 0

BEGIN        
 BEGIN TRY        
      
	  IF(@UserId IS NOT NULL OR @UserId <> '')
	  BEGIN
		--EDIT
		  IF EXISTS(SELECT TOP 1 UserId from [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE EmpID = @EmpId AND IsActive = 1 AND UserId != @UserId)
		  BEGIN
				SET @IsUserExists = 1
				SET @IsEmpIdExists = 1
		  END
		  ELSE IF EXISTS(SELECT TOP 1 UserId from [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE MobileNo = @MobileNo AND IsActive = 1 AND UserId != @UserId)
		  BEGIN
				SET @IsUserExists = 1
				SET @IsMobileNoExists = 1
		  END
		  ELSE IF EXISTS(SELECT TOP 1 UserId from [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE EmailId = @EmailId AND IsActive = 1 AND UserId != @UserId)
		  BEGIN
				SET @IsUserExists = 1
				SET @IsEmailIdExists = 1
		  END

	  END
	  ELSE
	  BEGIN
		--INSERT

		  IF EXISTS(SELECT TOP 1 UserId from [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE EmpID = @EmpId AND IsActive = 1)
		  BEGIN
				SET @IsUserExists = 1
				SET @IsEmpIdExists = 1
		  END
		  ELSE IF EXISTS(SELECT TOP 1 UserId from [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE MobileNo = @MobileNo AND IsActive = 1)
		  BEGIN
				SET @IsUserExists = 1
				SET @IsMobileNoExists = 1
		  END
		  ELSE IF EXISTS(SELECT TOP 1 UserId from [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) WHERE EmailId = @EmailId AND IsActive = 1)
		  BEGIN
				SET @IsUserExists = 1
				SET @IsEmailIdExists = 1
		  END

	  END


	  SELECT @IsUserExists as IsUserExists,
			 @IsEmpIdExists as IsEmpIdExists, 
			 @IsMobileNoExists as IsMobileNoExists,
			 @IsEmailIdExists as IsEmailIdExists
	  
 END TRY                        
 BEGIN CATCH          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
     
END 
