-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,25-Nov-2022>
-- Description:	<Description,GetMakeModelFuel>
--[Insurance_GetProposalSummary] '16413879-6316-4C1E-93A4-FF8318B14D37','','','8D23B769-431F-4D5E-B19E-8028EDBA106D'
-- =============================================
CREATE   PROCEDURE [dbo].[Insurance_GetProposalSummary]   
@InsurerId VARCHAR(50)  = NULL,
@VehicleNumber VARCHAR(20)  = NULL,
@VariantId VARCHAR(50)  = NULL,
@QuoteTransactionId VARCHAR(100)  = NULL
AS  
BEGIN  
	DECLARE @QuoteId VARCHAR(50),  @StageId VARCHAR(50)

	SELECT @StageId = StageID, @QuoteId = RefQuoteTransactionId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuoteTransactionId
	
	IF @StageId = 'ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2' OR @StageId = 'CD0D06FB-3AFC-4AF2-9FE6-3DBF98F0A105'
		BEGIN
			SELECT ProposalRequestBody ProposalRequestBody
			FROM Insurance_ProposalDynamicDetails WITH(NOLOCK)
			WHERE QuoteTransactionId=@QuoteId
		END
	ELSE
		BEGIN
			SELECT ProposalRequestBody ProposalRequestBody
			FROM Insurance_ProposalDynamicDetails WITH(NOLOCK)
			WHERE QuoteTransactionId= @QuoteTransactionId
		END
		
END  

