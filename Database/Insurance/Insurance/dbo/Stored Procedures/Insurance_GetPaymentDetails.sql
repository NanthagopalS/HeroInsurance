-- =============================================
-- Author:		<Author,,FIROZ S>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--[Insurance_GetPaymentDetails]'78190CB2-B325-4764-9BD9-5B9806E99621','1FE87595-0DD7-43FA-8F23-665ED63CFDC3'
-- =============================================

CREATE PROCEDURE [dbo].[Insurance_GetPaymentDetails]  
(
	@InsurerId VARCHAR(50) = NULL,
	@QuotetransactionId VARCHAR(50)= NULL,
	@ApplicationId VARCHAR(100) =NULL
)
AS
BEGIN 
	BEGIN TRY
		IF(ISNULL(@ApplicationId,'')!='')
		BEGIN
			SELECT @QuotetransactionId=QuoteTransactionId,@InsurerId=InsurerId FROM Insurance_PaymentTransactioN WITH(NOLOCK) WHERE ApplicationId=@ApplicationId
		END

		SELECT PaymentTransactionId,
				PAY.QuoteTransactionId,
				ApplicationId,
				PAY.LeadId,
				CONCAT(LEAD.LeadName,' ',LEAD.MiddleName,' ',LEAD.LastName) AS LeadName,
				ProposalNumber,
				PaymentTransactionNumber,
				Amount,
				Status PaymentStatus,
				CKYCStatus,
				CKYCLink,
				CKYCFailReason CKYCFailedReason,
				PolicyDocumentLink,
				DocumentId,
				PAY.PolicyNumber,
				CustomerId,
				PAY.InsurerId AS InsurerId,
				POL.PreviousPolicyType AS PolicyType,
				INS.Logo AS Logo
		FROM Insurance_PaymentTransaction PAY WITH(NOLOCK) 
		LEFT JOIN Insurance_Insurer INS WITH(NOLOCK) ON PAY.InsurerId = INS.InsurerId
		LEFT JOIN Insurance_LeadDetails LEAD WITH(NOLOCK) ON LEAD.LeadId = PAY.LeadId
		LEFT JOIN Insurance_PreviousPolicyType POL WITH(NOLOCK) ON POL.PreviousPolicyTypeId = LEAD.PolicyTypeId
		WHERE PAY.InsurerId=@InsurerID AND PAY.QuoteTransactionId=@QuotetransactionId
	END TRY   
	
BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END