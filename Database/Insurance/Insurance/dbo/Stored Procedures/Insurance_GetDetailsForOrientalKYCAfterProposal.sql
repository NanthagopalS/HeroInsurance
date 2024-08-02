-- ==========================================================================================   
-- Author:  <Author,Nantha S>    
-- Create date: <Create Date,13-10-2023,>    
-- Description: <Description,Need to get the details from db which stored during the proposal to perform kyc,>    
-- ==========================================================================================  
--EXEC [dbo].[Insurance_GetDetailsForOrientalKYCAfterProposal] '0135687C-2580-4E8C-B3CC-BEB5BD577FCD', '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9'

CREATE   PROCEDURE [dbo].[Insurance_GetDetailsForOrientalKYCAfterProposal]    
@QuoteTransactionId VARCHAR(50) = NULL,    
@InsurerId VARCHAR(50) = NULL    
AS    
BEGIN     
 BEGIN TRY    

  SELECT ProposalId AS PolicyNumber, LeadId AS LeadID FROM Insurance_QuoteTransaction WITH(NOLOCK)
  WHERE RefQuoteTransactionId = @QuoteTransactionId AND InsurerId = @InsurerId  AND StageID = 'ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2'

 END TRY          
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH       
END