

-- =============================================
-- Author:		<Author, VISHAL KANJARIYA>
-- Create date: <Create Date,05-DEC-2022>
-- Description:	<Description,,INSERT USER OTP DETAIL>
--Identity_InsertUserOTPDetail '9987848971','','1064','1F18D0DF-35C9-4689-832-2428648E69DD','VERIFYOTP'
-- =============================================
CREATE   PROCEDURE [dbo].[Identity_InsertUserOTPDetail1] 
(
	@MobileNo VARCHAR(10) = NULL,
	@OTPId VARCHAR(100) = NULL,
	@OTPNumber VARCHAR(10) = NULL,
	@UserId VARCHAR(50)= NULL,
	@Condition VARCHAR(50)=NULL
)
AS
BEGIN
	BEGIN TRY
		
		DECLARE @UserProfileStage INT = NULL, @WrongOTPCount int = 0,  @CodePatternPOSPId VARCHAR(50) = NULL, @POSPId INT = 0  
		DECLARE @MobileNumber VARCHAR(20) = NULL, @FinalCodePOSPId VARCHAR(50) = NULL

		IF(@Condition='VERIFYOTP')
		BEGIN

			SELECT @UserProfileStage = UserProfileStage FROM Identity_User WITH(NOLOCK) WHERE UserId = @UserId

			SELECT @MobileNumber = MobileNo FROM Identity_User WITH(NOLOCK) WHERE UserId = @UserId

			IF EXISTS(SELECT TOP 1 OTPNumber FROM [Identity_OTP] WITH(NOLOCK) WHERE IsActive = 1 AND OTPNumber = @OTPNumber AND UserId = @UserId AND DATEDIFF(MINUTE,OTPSENDDATETIME,GETDATE()) <= 3)
			BEGIN
				
				--LogIn Entry
				Update Identity_UserLog SET IsActive = 0 WHERE UserId = @UserId

				INSERT INTO Identity_UserLog (UserId) VALUES (@UserId)
				
				UPDATE [Identity_OTP] SET IsVerify = 1 WHERE OTPNumber = @OTPNumber AND UserId = @UserId AND IsActive = 1

				SELECT CAST(1 AS BIT) IsValidOTP, @UserProfileStage AS UserProfileStage, @MobileNumber AS MobileNumber, '0' AS WrongOTPCount

				--POSP ID after authentication
				SET @CodePatternPOSPId = (SELECT CodePattern FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH(NOLOCK) WHERE [Code] = 'POSP' AND IsActive = 1)  
  
				SET @POSPId = (SELECT NextValue FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH(NOLOCK) WHERE [Code] = 'POSP' AND IsActive = 1)  
  
				SET @FinalCodePOSPId = CONCAT(@CodePatternPOSPId, CAST(@POSPId AS VARCHAR)) 

				SET @POSPId = @POSPId + 1  
				UPDATE [HeroIdentity].[dbo].[Identity_AutoGenerateId] SET NextValue = @POSPId WHERE [Code] = 'POSP' AND IsActive = 1  
				Update [HeroIdentity].[dbo].[Identity_User] SET POSPId = @FinalCodePOSPId where UserId = @UserId
     
			END
			ELSE
			BEGIN
				
				UPDATE [Identity_OTP] SET IsVerify = 0 WHERE UserId = @UserId AND DATEDIFF(MINUTE,OTPSENDDATETIME,GETDATE()) <= 3

				IF EXISTS(SELECT WrongOTPCount from Identity_OTP WITH(NOLOCK) WHERE UserId = @UserId AND DATEDIFF(MINUTE,OTPSENDDATETIME,GETDATE()) <= 3 AND IsActive = 1)
				BEGIN
					SET @WrongOTPCount = (SELECT WrongOTPCount from Identity_OTP WITH(NOLOCK) WHERE UserId = @UserId AND DATEDIFF(MINUTE,OTPSENDDATETIME,GETDATE()) <= 3 AND IsActive = 1)
				END
				ELSE
				BEGIN
					SET @WrongOTPCount = 0
				END

				SET @WrongOTPCount = @WrongOTPCount + 1


				UPDATE [Identity_OTP] SET WrongOTPCount = @WrongOTPCount WHERE UserId = @UserId AND DATEDIFF(MINUTE,OTPSENDDATETIME,GETDATE()) <= 3 AND IsActive = 1

				SELECT CAST(0 AS BIT) IsValidOTP, @UserProfileStage AS UserProfileStage, @MobileNumber AS MobileNumber, CAST(@WrongOTPCount AS VARCHAR(10)) AS WrongOTPCount
			END
		END
		ELSE
		BEGIN
			
			IF(ISNULL(@UserId,'') ='')
			BEGIN
				SELECT @UserId = UserId from Identity_User WITH(NOLOCK) WHERE MobileNo=@MobileNo
			END

			UPDATE [dbo].[Identity_OTP] SET IsActive = 0 WHERE UserId = @UserId AND @MobileNo = @MobileNo
			
			INSERT INTO [dbo].[Identity_OTP] (OTPId, OTPNumber, UserId, MobileNo, CreatedBy) 
			VALUES (@OTPId, @OTPNumber, @UserId, @MobileNo, @UserId)
		END
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END