﻿CREATE   PROCEDURE [dbo].[Admin_UpdateUserEmailIdDetail]   
(  
 @UserId VARCHAR(100) = NULL,  
 @EmailId VARCHAR(100) = NULL   
)  
AS  
BEGIN  
 BEGIN TRY  
  BEGIN  
   UPDATE [HeroIdentity].[dbo].[Identity_User] SET   
    EmailId = @EmailId , UpdatedOn = GETDATE()     
   WHERE  
    UserId = @UserId  
  END  
    
     
  END TRY                  
 BEGIN CATCH            
         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList  
    
 END CATCH  
  
END 
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
