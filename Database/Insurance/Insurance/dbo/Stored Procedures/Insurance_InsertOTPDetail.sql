CREATE    PROCEDURE [dbo].[Insurance_InsertOTPDetail] 
(
	@MobileNo VARCHAR(10) = NULL,
	@OTPId VARCHAR(100) = NULL,
	@OTPNumber VARCHAR(10) = NULL,
	@LeadID VARCHAR(50) = NULL,
	@UserId VARCHAR(50)= NULL,
	@Condition VARCHAR(50)=NULL
)
AS
BEGIN
BEGIN TRY

	DECLARE @MobileNumber VARCHAR(20) = NULL,
	@WrongOTPCount int = 0
	IF(@Condition='VERIFYOTP')
		BEGIN
			SELECT @MobileNumber = MobileNo,@WrongOTPCount = WrongOTPCount
			FROM [Insurance_OTP] WITH(NOLOCK) 
			WHERE UserId = @UserId and IsActive = 1 AND LeadId = @LeadID

			IF EXISTS(SELECT TOP 1 OTPNumber FROM [Insurance_OTP] WITH(NOLOCK) WHERE IsActive = 1 AND OTPNumber = @OTPNumber AND UserId = @UserId AND DATEDIFF(MINUTE,OTPSENDDATETIME,GETDATE()) <= 30 AND LeadId = @LeadID)
			BEGIN
				UPDATE [Insurance_OTP] 
				SET IsVerify = 1 , UpdatedOn = GETDATE() 
				WHERE OTPNumber = @OTPNumber AND UserId = @UserId AND IsActive = 1 AND LeadId = @LeadID

				SELECT CAST(1 AS BIT) IsValidOTP, @MobileNumber AS MobileNumber, '0' AS WrongOTPCount
			END
			ELSE
			BEGIN
				UPDATE [Insurance_OTP] 
				SET IsVerify = 0, WrongOTPCount = ISNULL(@WrongOTPCount,0)+1, UpdatedOn = GETDATE()
				WHERE UserId = @UserId AND DATEDIFF(MINUTE,OTPSENDDATETIME,GETDATE()) <= 30 AND LeadId = @LeadID AND IsActive = 1

				SELECT CAST(0 AS BIT) IsValidOTP, @MobileNumber AS MobileNumber, CAST(@WrongOTPCount AS VARCHAR(10)) AS WrongOTPCount
			END
		END
		ELSE
		BEGIN

		UPDATE [dbo].[Insurance_OTP] SET IsActive = 0, UpdatedOn = GETDATE() WHERE UserId = @UserId AND @MobileNo = @MobileNo
		INSERT INTO [dbo].[Insurance_OTP] (OTPId, OTPNumber,LeadId, UserId, MobileNo) 
			VALUES (@OTPId, @OTPNumber,@LeadID, @UserId, @MobileNo)
		END
			
END TRY 
BEGIN CATCH
 DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
END CATCH
END