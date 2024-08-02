-- =========================================================================================           
-- Author:  <Author, Ankit>        
-- Create date: <Create Date,23-Feb-2023>        
-- Description: <Description, Admin_GetNotificationBroadcastById>  
-- code to get notification details(master record) from which the title and description is taken
-- =========================================================================================           
 CREATE    PROCEDURE [dbo].[Admin_GetPushNotificationById]         
 (        
 @NotificationId VARCHAR(100) = Null       
 )   
 As  
 Begin    
  
 BEGIN Try  
  -- --fetch all push notification only fromm publish table (notificationid), only those record which are not in queue and not processed  
  --select * from [HeroAdmin].[dbo].[Admin_NotificationBoradcast] where IsInQueue = 0 and IsProccessed = 0 and IsDelivered = 0 --and NotificationId = @NotificationId  
  
  ----foreach record, fetch the device id, title,message  
  --select Ud.MobileDeviceId, Nb.Title, Nb.MessageTemplate, Nb.Description, Nb.IsDelivered, Nb.IsInQueue, Nb.IsProccessed from [HeroAdmin].[dbo].[Admin_NotificationBoradcast] as Nb  
  --Left Join [HeroPOSP].[dbo].[POSP_UserDeviceDetail] as Ud on Ud.UserId = Nb.RecipientUserId 
  --Left Join [HeroAdmin].[dbo].[Admin_AlertType] as AT on AT.AlertTypeId = Nb.AlertTypeId
  --where   
  --((@NotificationId is null or @NotificationId  = '') or (Nb.NotificationId = @NotificationId))  
  --and (AT.AlertType = 'Push')

	  select AN.NotificationId, AN.AlertTypeId, AN.RecipientUserids, AN.NotificationTitle, AN.Description, AN.NotificationEventId, NE.EventTitle, NE.EventDescription, NE.IsPush
	  from [HeroAdmin].[dbo].[Admin_Notification] as AN WITH(NOLOCK)
	  Left Join [HeroAdmin].[dbo].[Admin_NotificationEvent] as NE WITH(NOLOCK) on NE.NotificationEventId = AN.NotificationEventId
	  Left Join [HeroAdmin].[dbo].[Admin_AlertType] as AT WITH(NOLOCK) on AT.AlertTypeId = AN.AlertTypeId
	  where 
			AN.NotificationId = @NotificationId AND
			  AT.AlertType = 'Push' and AN.IsActive = 1 and AN.IsPublished!=1
  
 END Try  
   
 BEGIN CATCH          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
     
END