CREATE   PROCEDURE [dbo].[Insurance_Job_GetCholaBreakinStatus]  
@InsurerId VARCHAR(50) = NULL  
AS  
BEGIN   
 BEGIN TRY  
  SELECT TOP 10 BreakinId,LeadId FROM Insurance_LeadDetails WITH(NOLOCK)   
  WHERE InsurerId = @InsurerId   
  AND IsBreakin = 1  
  AND IsBreakinApproved IS NULL   
  AND BreakinId IS NOT NULL order by CreatedOn desc  
 END TRY     
   
BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH     
END