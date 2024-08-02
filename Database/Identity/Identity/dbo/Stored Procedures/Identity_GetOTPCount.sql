

-- =============================================
-- Author:		<Author, VISHAL KANJARIYA>
-- Create date: <Create Date, 20-DEC-2022>
-- Description:	<Description, GET USER OTP COUNT DETAIL>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_GetOTPCount] 
(
	@UserId VARCHAR(50)= NULL,
	@MobileNo VARCHAR(10) = NULL
)
AS
BEGIN
	
	DECLARE @WrongOTPCount INT = 0, @DBOTPCount INT = 0
	
	BEGIN TRY
		--OTP generated = 1 & wrong OTP > 3
		SET @WrongOTPCount = (SELECT WrongOTPCount 
		FROM Identity_OTP WITH(NOLOCK)
		WHERE DATEDIFF(MINUTE,OTPSendDateTime,GETDATE()) <= 3 AND UserId = @UserId and MobileNo = @MobileNo AND IsVerify = 0 AND IsActive = 1)

		--OTP generated > 3 
		SET @DBOTPCount = (SELECT COUNT(OTPNumber) AS DBOTPCount 
		FROM Identity_OTP WITH(NOLOCK)
		WHERE DATEDIFF(MINUTE,OTPSendDateTime,GETDATE()) <= 3 AND UserId = @UserId and MobileNo = @MobileNo AND IsVerify = 0)
		
		IF(@WrongOTPCount > @DBOTPCount)
		BEGIN
			SELECT @WrongOTPCount AS DBOTPCount
		END
		ELSE
		BEGIN
			SELECT @DBOTPCount AS DBOTPCount
		END		
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
