    

CREATE PROCEDURE [dbo].[Admin_ResetDatabaseAccountByUserId]     
(    
	@UserId varchar(10)
)    
AS    
BEGIN    
	 BEGIN TRY  
		
		
		SET @UserId = (SELECT TOP (1) UserId FROM Identity_User WITH(NOLOCK) WHERE UserId = @UserId)		
		
		  IF EXISTS(SELECT TOP (1) UserId FROM Identity_User WITH(NOLOCK) WHERE UserId = @UserId)    
		  BEGIN
		  
			DELETE FROM [HeroIdentity].[dbo].Identity_DocumentDetail WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_EmailVerification WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_OTP WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_PanVerification WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_ResetPasswordVerification WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_User WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_UserAddressDetail WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_UserBankDetail WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_UserBreadcrumStatusDetail WHERE UserId = @UserId
			DELETE FROM [HeroIdentity].[dbo].Identity_UserDetail WHERE UserId = @UserId

			DELETE FROM [HeroAdmin].[dbo].Admin_UserRoleMapping WHERE UserId = @UserId

			--POSP

			DELETE FROM [HeroPOSP].[dbo].POSP_Agreement WHERE UserId = @UserId

			DELETE FROM [HeroPOSP].[dbo].POSP_ExamPaperDetail WHERE ExamId IN (SELECT Id FROM [HeroPOSP].[dbo].POSP_Exam WHERE UserId = @UserId)
		
			DELETE FROM [HeroPOSP].[dbo].POSP_Exam WHERE UserId = @UserId

			DELETE FROM [HeroPOSP].[dbo].POSP_Rating WHERE UserId = @UserId

			DELETE FROM [HeroPOSP].[dbo].POSP_TrainingProgressDetail WHERE TrainingId IN (SELECT Id FROM [HeroPOSP].[dbo].POSP_Training WHERE UserId = @UserId)
		
			DELETE FROM [HeroPOSP].[dbo].POSP_Training WHERE UserId = @UserId
		  
		  END
		  
	 END TRY                    
	 BEGIN CATCH              
			   
		  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
		  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
		  SET @ErrorDetail=ERROR_MESSAGE()                                
		  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList    
		  
	 END CATCH  
    
END