/*
Description : Get Chola PaymentID For Cron Job
Create BY   : Parth Gandhi
Created On  : 31,May-2023
*/
CREATE   PROCEDURE [dbo].[Insurance_Job_GetCholaPaymentStatus]      
@InsurerId VARCHAR(50) = NULL  
AS    
BEGIN     
 BEGIN TRY    
 SELECT TOP 10 ApplicationId
 FROM Insurance_PaymentTransaction payTrans WITH (NOLOCK) WHERE payTrans.InsurerId = @InsurerId   
 AND Status IN ('0151C6E3-8DC5-4BBD-860A-F1501A7647B2')  
 ORDER BY payTrans.CreatedOn DESC          
 END TRY       
     
BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH       
END