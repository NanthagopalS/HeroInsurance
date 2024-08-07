﻿-- =============================================  
-- Author:  <Author, Girish>  
-- Create date: <Create Date,29-DEC-2022>  
-- Description: <Description,,Admin_InsertUserCategory>  
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_InsertUserCategory]   
(  
 @UserCategoryName varchar(50),    
 @CreatedBy varchar(50)  
   
)  
AS  
BEGIN  
 BEGIN TRY  
  BEGIN TRANSACTION  
   INSERT INTO [dbo].[Admin_UserCategory]  
           ([UserCategoryName]  
           ,[CreatedBy])            
     VALUES  
           (@UserCategoryName  
           , @CreatedBy)  
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
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
