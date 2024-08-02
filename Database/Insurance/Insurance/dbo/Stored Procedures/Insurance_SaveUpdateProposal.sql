
--[Insurance_SaveUpdateProposal] 'test','test','test','test'
CREATE procedure [dbo].[Insurance_SaveUpdateProposal]  
@QuoteTransactionID varchar(50),  
@RequestBody varchar(max),
@VehicleNumber varchar(20), 
@VariantId varchar(50)
AS  
BEGIN  
  
 DECLARE @LeadID varchar(50)  
 DECLARE @InsurerId varchar(100)
 begin try

	 SELECT @LeadID = LeadId, @InsurerId = InsurerId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionID = @QuoteTransactionID  
	 
   
	 MERGE INTO Insurance_ProposalDynamicDetails AS tg  
	 USING(  
	  SELECT @QuoteTransactionID QuoteTransactionID,        
		@RequestBody RequestBody,
		@VehicleNumber VehicleNumber,
		@VariantId VariantId,
		@LeadID LeadId,
		@InsurerId InsurerId
	 ) AS Src  
	 ON  tg.LeadId=Src.LeadId and tg.InsurerId=Src.InsurerId 
	 WHEN MATCHED THEN UPDATE SET        
	  tg.QuoteTransactionID = Src.QuoteTransactionID,
	  tg.ProposalRequestBody=Src.RequestBody,  
	  tg.VehicleNumber = Src.VehicleNumber,
	  tg.VariantId = Src.VariantId,
	  tg.UpdatedOn = getdate(),  
	  tg.UpdatedBy = '1'  
	 WHEN NOT MATCHED THEN         
	  INSERT(InsurerId,LeadID,QuoteTransactionID,ProposalRequestBody,CreatedBy,CreatedOn,VariantId,VehicleNumber)  
	  VALUES(@InsurerId,@LeadID, Src.QuoteTransactionID, Src.RequestBody,'1',getdate(),src.VariantId,src.VehicleNumber);  
  
	SELECT InsurerId, LeadId, QuoteTransactionID QuoteTransactionId, ProposalRequestBody
	FROM Insurance_ProposalDynamicDetails WITH(NOLOCK) WHERE QuoteTransactionID= @QuoteTransactionID


end try
begin catch
	SELECT '' LeadID  

	 DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
	 SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
	 SET @ErrorDetail=ERROR_MESSAGE()                                    
	 EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                

end catch
END  
