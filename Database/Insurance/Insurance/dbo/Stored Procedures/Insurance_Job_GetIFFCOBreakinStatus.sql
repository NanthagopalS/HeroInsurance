-- =============================================
-- Author:		<Author,,NANTHA S>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--[dbo].[Insurance_Job_GetIFFCOBreakinStatus]
-- =============================================

CREATE   PROCEDURE [dbo].[Insurance_Job_GetIFFCOBreakinStatus]
AS
BEGIN 
	BEGIN TRY
		SELECT TOP 10 BreakinId, LEADS.QuoteTransactionID AS QuoteTransactionId, PAYMENT.ProposalNumber AS ProposalNumber, LEADS.CreatedBy AS UserId FROM Insurance_LeadDetails LEADS WITH(NOLOCK) 
		INNER JOIN Insurance_PaymentTransaction PAYMENT WITH(NOLOCK) ON LEADS.QuoteTransactionID = PAYMENT.QuoteTransactionId
		WHERE LEADS.InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3' AND IsBreakin = 1
		AND IsBreakinApproved is NULL 
		AND BreakinId IS NOT NULL order by LEADS.CreatedOn desc
	END TRY   
	
BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END