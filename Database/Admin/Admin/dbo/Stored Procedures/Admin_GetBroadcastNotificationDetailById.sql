-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date, 1-May-2023>      
-- Description: <Description, Admin_GetNotificationDetailById>
-- Gets notification broadcast details with the FMCtoken details where notifications are needed to be sent.
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_GetBroadcastNotificationDetailById]       
 (      
	@NotificationId VARCHAR(100) = Null	
 ) 
 As
 Begin  

	BEGIN TRY
	Select NB.NotificationBoradcastId, NB.Title, NB.RecipientUserId, NB.Description, UDD.BrowserId, UDD.MobileDeviceId,
	UDD.IsActive,NB.IsActive,NB.IsProccessed,NB.IsDelivered
	--,UUD.UserDeviceIdUserDeviceId
	from [HeroAdmin].[dbo].[Admin_NotificationBoradcast] as NB WITH(NOLOCK)
	Left Join [HeroPOSP].[dbo].[POSP_UserDeviceDetail] as UDD WITH(NOLOCK) on UDD.UserId = NB.RecipientUserId
	where NB.NotificationId = @NotificationId and UDD.IsActive = 1 and NB.IsActive= 1 and NB.IsProccessed=0 and NB.IsDelivered=0
	END TRY

	
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END