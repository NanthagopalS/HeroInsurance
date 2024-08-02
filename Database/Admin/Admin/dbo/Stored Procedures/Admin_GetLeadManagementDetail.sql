--viewLeadsType=Lead&searchText=&policyType=&preQuote=&pageSize=10&userId=6BC5BDF1-31A8-491D-9780-B1EECE9BDBBC    
    
CREATE      PROCEDURE [dbo].[Admin_GetLeadManagementDetail]                         
(                    
  @ViewLeadsType VARCHAR(100) = 'Lead',     -- Leads, Prospects               
  --@ViewLeadsType2 VARCHAR(100) = 'Leads',     -- Leads, Payment Gateway      
  @UserId VARCHAR(100) = NULL,      
  @SearchText VARCHAR(100) = NULL,                   
  @LeadType VARCHAR(100) = NULL,                  
  @POSPId  VARCHAR(100) = NULL,  --1   UserId Identity_User             
  @PolicyType VARCHAR(100) = NULL,  --2  InsuranceTypeId  Insurance_InsuranceType            
  @PreQuote VARCHAR(100) = NULL,    --3  StageId Insurance_StageMaster            
  @AllStatus VARCHAR(100) = NULL,                  
  @StartDate VARCHAR(100) = NULL,                    
  @EndDate VARCHAR(100) = NULL,                  
  @CurrentPageIndex INT = 1,                  
  @CurrentPageSize INT = 10
 )                   
AS                                   
BEGIN                        
 BEGIN TRY      
       
   SELECT   IU.UserId,     
   IU.POSPId,     
   IL.LeadId,     
   IL.LeadName as CustomerName,     
   SM.Stage as StageValue,IL.PhoneNumber AS MobileNo,IL.Email AS EmailId,  
   'Motor' as PolicyType,     
   Try_convert(datetime, IL.CreatedOn, 20)  as GeneratedOn,    
   Try_convert(datetime, IL.PolicyEndDate, 20) AS ExpiringOn,                    
   'Motor' as Product,    
   CASE WHEN PM.[Status] = 'Payment Completed' THEN 'Issued' ELSE 'Cancelled' END AS PolicyStatus,        
   PM.[Status] as PaymentStatus,    
   CASE WHEN IU.POSPId IS NOT NULL     
     THEN IU.POSPId + '-' + IU.UserName     
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' +     
   IU.UserName END AS CreatedBy,     
   PM.Amount,     
   SM.StageId            
   FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] IL WITH(NOLOCK)    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK) ON IU.UserId = IL.CreatedBy             
   LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH(NOLOCK) on SM.StageId = IL.StageId         
   LEFT JOIN [HeroInsurance].[dbo].[Insurance_PaymentTransaction] PM WITH(NOLOCK) ON PM.LeadId = IL.LeadId        
   WHERE IL.IsActive = 1    
   AND (IL.CreatedBy =@POSPId OR ISNULL(@POSPId,'')='')
   ORDER BY IL.CreatedOn DESC      
  
  
   SELECT 0 CurrentPageIndex,0 as PreviousPageIndex,0 as NextPageIndex,0 as CurrentPageSize,0 as TotalRecord   
              
 END TRY                                        
 BEGIN CATCH                          
    DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                                    
    SET @StrProcedure_Name=ERROR_PROCEDURE()                                                    
    SET @ErrorDetail=ERROR_MESSAGE()                                                    
    EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                                     
   END CATCH                        
                     
END 