CREATE   PROCEDURE [dbo].[Insurance_UpdateICICIBreakinStatus]     
@IsBreakinApproved BIT = NULL,
@PaymentLink NVARCHAR(MAX) = NULL,
@Stage VARCHAR(50) = NULL,
@BreakinId VARCHAR(50) = NULL
                      
AS      
BEGIN      
	BEGIN TRY      
		IF(@IsBreakinApproved = 1)
			BEGIN
				
				DECLARE @QuoteId VARCHAR(100)  
  
				SELECT @QuoteId = QuoteTransactionId FROM Insurance_LeadDetails WITH(NOLOCK) WHERE BreakinId = @BreakinId  

				UPDATE Insurance_LeadDetails 
				SET PaymentLink = @PaymentLink,
				StageId = (SELECT StageID FROM Insurance_StageMaster WITH(NOLOCK) WHERE Stage = @Stage),
				IsBreakinApproved = @IsBreakinApproved, UpdatedOn = GETDATE()
				WHERE BreakinId = @BreakinId

				UPDATE Insurance_QuoteTransaction SET StageID = (SELECT StageID FROM Insurance_StageMaster WITH(NOLOCK) WHERE Stage = @Stage), UpdatedOn = GETDATE()
				WHERE QuoteTransactionId = @QuoteId
			END
		ELSE
			BEGIN
				UPDATE Insurance_LeadDetails 
				SET IsBreakinApproved = @IsBreakinApproved, UpdatedOn = GETDATE()
				WHERE BreakinId = @BreakinId
			END
	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END 