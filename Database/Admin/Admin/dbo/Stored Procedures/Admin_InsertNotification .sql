-------- =============================================      
-- Author:  <Author, Ankit>      
-- Create date: <Create Date, 6-Apr-2023>      
-- Description: <Description,,Admin_InsertNotification >      
-- =============================================      
Create     PROCEDURE [dbo].[Admin_InsertNotification ]       
(      
     
 @AlertTypeId varchar(100),    
 @RecipientId varchar(100),    
 @RecipientUserids varchar(max),    
 @NotificationOrigin varchar(100),    
 @NotificationTitle varchar(100),    
 @Description varchar(max),    
 @NotificationEventId varchar(100),    
 @IsPublished bit ,  
 @UserId VARCHAR(100)  
)      
AS      
BEGIN      
 BEGIN TRY       
   
  INSERT INTO [dbo].[Admin_Notification]    
  (AlertTypeId, RecipientId, RecipientUserids, NotificationCategory, NotificationOrigin, NotificationTitle, Description, NotificationEventId, IsPublished, CreatedBy)    
  values (@AlertTypeId, @RecipientId, @RecipientUserids, 'Important', @NotificationOrigin, @NotificationTitle, @Description, @NotificationEventId, @IsPublished, @UserId)    
 
    
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
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
