-- =========================================================================================           
-- Author:  <Author, Suraj>        
-- Create date: <Create Date,4-May-2023>        
-- Description: <Description, Admin_GetNotificationDetailById>  
-- code to get notification details(master record) event type and 
-- =========================================================================================           
 CREATE    PROCEDURE [dbo].[Admin_GetNotificationRecordById]         
 (        
 @NotificationId VARCHAR(100) = Null       
 )   
 As  
 Begin    
  
 BEGIN Try  
	  select AT.AlertType,NE.IsInApp,NE.IsWhatsApp,NE.IsEmail,NE.IsPush,NE.IsSMS,AN.NotificationId, AN.AlertTypeId, AN.NotificationTitle, AN.Description, AN.NotificationEventId, NE.EventTitle, NE.EventDescription,AN.RecipientUserids
	  from [HeroAdmin].[dbo].[Admin_Notification] as AN WITH(NOLOCK)
	  Left Join [HeroAdmin].[dbo].[Admin_NotificationEvent] as NE WITH(NOLOCK) on NE.EventCode = AN.NotificationEventId
	  Left Join [HeroAdmin].[dbo].[Admin_AlertType] as AT WITH(NOLOCK) on AT.AlertTypeId = AN.AlertTypeId
	  where 
			AN.NotificationId = 
			-- '3FD5D333-8D34-4207-AD95-179D790B4B2B'
			 @NotificationId 
			AND
			   AN.IsPublished!=1
 END Try  
   
 BEGIN CATCH          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
     
END