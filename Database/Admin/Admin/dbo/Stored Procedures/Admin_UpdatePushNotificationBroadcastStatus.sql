-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,28-Apr-2023>      
-- Description: <Description, Admin_UpdateNotificationStatus>
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_UpdatePushNotificationBroadcastStatus]       
 (      
	@NotificationBoradcastId VARCHAR(100),
	@FirebaseQueueId VARCHAR(MAX)
 ) 
 As
 Begin  
	BEGIN TRY
		
		Update [HeroAdmin].[dbo].[Admin_NotificationBoradcast] 
		Set IsInQueue = 1 , FirebaseQueueId = @FirebaseQueueId, CreatedOn= GETDATE()
		where NotificationBoradcastId = @NotificationBoradcastId
	
	END TRY 
	
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END