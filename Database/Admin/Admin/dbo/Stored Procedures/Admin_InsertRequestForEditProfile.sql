-- =========================================================================================               
-- Author:  <Author, Parth>            
-- Create date: <Create Date,17-Macrh-2023>            
-- Description: <Description, Admin_InsertRequestForEditProfile>         
-- exec Admin_InsertRequestForEditProfile null,'2BA70571-63B3-4193-9F31-BDBCC59ED08B',null,null,1       
-- =========================================================================================               
 CREATE PROCEDURE [dbo].[Admin_InsertRequestForEditProfile]             
 (            
   @UserId VARCHAR(100),            
   @RequestType VARCHAR(100),          
   @NewRequestTypeContent VARCHAR(100), 
   @CreatedBy VARCHAR(100) = NULL
 )  
AS      
BEGIN  
 BEGIN TRY  
  BEGIN TRANSACTION  
   INSERT INTO [dbo].[Admin_RequestForEditProfile] ([UserId],[RequestType],[NewRequestTypeContent],[CreatedBy])            
     VALUES (@UserId,@RequestType,@NewRequestTypeContent, @CreatedBy)  
   IF @@TRANCOUNT > 0  
            COMMIT  
 END TRY                  
 BEGIN CATCH            
 IF @@TRANCOUNT > 0  
        ROLLBACK    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END 
