  
CREATE  PROCEDURE [dbo].[Admin_DeletePoliciesDetail]         
(        
 @POSPId VARCHAR(100)  
)        
AS        
BEGIN        
 BEGIN TRY   
  
 IF EXISTS(SELECT DISTINCT LeadId FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] WHERE LeadId = @POSPId)      
 BEGIN   
   
 Update[HeroInsurance].[dbo].[Insurance_LeadDetails]  SET IsActive = 0
 WHERE LeadId = @POSPId;  
  
 END   
 END TRY                      
 BEGIN CATCH                
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
        
END 
