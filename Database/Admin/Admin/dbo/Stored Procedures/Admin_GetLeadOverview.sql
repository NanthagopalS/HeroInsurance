--[dbo].[Admin_GetLeadOverview] 'Leads','','',''  
CREATE    PROCEDURE [dbo].[Admin_GetLeadOverview]  
(                  
   @LeadType VARCHAR(100) = 'Leads',     -- Leads, Prospects    
   @UserId VARCHAR(100) = NULL,            
   @StartDate VARCHAR(100),                 
   @EndDate VARCHAR(100)    
 )             
AS                  
             
BEGIN                  
 BEGIN TRY              
  SELECT TOP(5) IU.UserId,   
		 IU.POSPId,   
		 IL.LeadId,   
		 IL.LeadName as CustomerName,   
		 SM.Stage as StageValue,
		 
		 CASE WHEN IL.PhoneNumber IS NULL OR IL.PhoneNumber = '' THEN ''
        ELSE IL.PhoneNumber
      END AS MobileNo,
      CASE
        WHEN IL.Email IS NULL OR IL.Email = '' THEN ''
        ELSE IL.Email
      END AS EmailId,
		  
		 'Motor' as PolicyType,   
		 IL.CreatedOn as GeneratedOn,  
		 IL.PolicyEndDate AS ExpiringOn,                  
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
		 ORDER BY IL.CreatedOn DESC 
     
 END TRY                                  
 BEGIN CATCH                
   DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                              
   SET @StrProcedure_Name=ERROR_PROCEDURE()                                              
   SET @ErrorDetail=ERROR_MESSAGE()                                              
   EXEC Admin_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name, @ErrorDetail = @ErrorDetail, @ParameterList = @ParameterList                                               
 END CATCH                  
               
END 