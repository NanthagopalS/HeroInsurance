  -- EXEC [Identity_SoftDeleteUserAccount] ''
CREATE   PROCEDURE [dbo].[Identity_SoftDeleteUserAccount]     
(    
	@MobileNo varchar(max)
)    
AS    
BEGIN    
	 BEGIN TRY  
		
		DECLARE @UserIdList TABLE (UserId VARCHAR(100))

		DECLARE @UserId VARCHAR(100) = NULL
			IF(ISNULL(@MobileNo,'') != '')
				BEGIN
		INSERT @UserIdList (UserId)
		SELECT UserID FROM HeroIdentity.dbo.Identity_User WHERE (ISNULL(@MobileNo,'')='' OR MobileNo IN (SELECT value
					FROM STRING_SPLIT(@MobileNo, ',')))
				-- Identity
				Update [HeroIdentity].[dbo].Identity_DocumentDetail Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_EmailVerification Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_OTP Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_PanVerification Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_ResetPasswordVerification Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_User Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_UserAddressDetail Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_UserBankDetail Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_UserBreadcrumStatusDetail Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				Update [HeroIdentity].[dbo].Identity_UserDetail Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)

				-- Admin
				-- Update [HeroAdmin].[dbo].Admin_UserRoleMapping Set IsActive = 0 WHERE UserId IN (SELECT UserId FROM @UserIdList)
				--POSP

				Update [HeroPOSP].[dbo].POSP_Agreement Set IsActive = 0, UpdatedOn = GETDATE() WHERE UserId IN (SELECT UserId FROM @UserIdList)

				Update [HeroPOSP].[dbo].POSP_ExamPaperDetail Set IsActive = 0, UpdatedOn = GETDATE() WHERE ExamId IN (SELECT Id FROM [HeroPOSP].[dbo].POSP_Exam WHERE UserId IN (SELECT UserId FROM @UserIdList))

				Update [HeroPOSP].[dbo].POSP_Exam Set IsActive = 0, UpdatedOn = GETDATE() WHERE UserId IN (SELECT UserId FROM @UserIdList)

				Update [HeroPOSP].[dbo].POSP_Rating Set IsActive = 0, UpdatedOn = GETDATE() WHERE UserId IN (SELECT UserId FROM @UserIdList)

				Update [HeroPOSP].[dbo].POSP_TrainingProgressDetail Set IsActive = 0, UpdatedOn = GETDATE() WHERE TrainingId IN (SELECT Id FROM [HeroPOSP].[dbo].POSP_Training WHERE UserId IN (SELECT UserId FROM @UserIdList))

				Update [HeroPOSP].[dbo].POSP_Training Set IsActive = 0, UpdatedOn = GETDATE() WHERE UserId IN (SELECT UserId FROM @UserIdList)

		  		
		  END
		  
	 END TRY                    
	 BEGIN CATCH              
			   
		  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
		  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
		  SET @ErrorDetail=ERROR_MESSAGE()                                
		  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList    
		  
	 END CATCH  
	END