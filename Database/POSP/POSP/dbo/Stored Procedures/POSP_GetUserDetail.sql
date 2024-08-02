

CREATE   PROCEDURE[dbo].[POSP_GetUserDetail]  
(  
 @UserId VARCHAR(100) = NULL  
)  
AS  
BEGIN   
 BEGIN TRY  
  
  SET NOCOUNT ON;  
  
  --DECLARE @ExamCertificate bit = 0, @POSPAgreement bit = 0  
  
  SELECT IU.UserId, IU.UserName, IU.EmailId, IU.MobileNo, IU.POSPId   
     
  FROM [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) 
  
  WHERE IU.UserId = @UserId   
    
 
  
 END TRY               
 BEGIN CATCH            
         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  
