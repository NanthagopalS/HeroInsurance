
-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,23-Feb-2023>      
-- Description: <Description, Admin_PublishNotification>
-- 8 May, 2023 -- added NotificationCategory addition in the SP. -- suraj
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_PublishNotification]       
 (      
	@NotificationId VARCHAR(100) = Null     
 ) 
 As
BEGIN
BEGIN TRY 
	IF OBJECT_ID('tempNotification') IS NOT NULL
    DROP TABLE tempNotification
	CREATE TABLE tempNotification
	(
		NotificationId VARCHAR(100) NOT NULL,
		RecipientUserids VARCHAR(100) NULL
	)

	INSERT INTO tempNotification Select NotificationId, value from Admin_Notification 
	Outer Apply string_split(RecipientUserids, ',') as RecipientUserids

	Declare @AlertTypeResult VARCHAR(100)

	Set @AlertTypeResult = (Select AT.AlertType from [HeroAdmin].[dbo].[Admin_Notification] as AN
	Inner Join [HeroAdmin].[dbo].[Admin_AlertType] as At on At.AlertTypeId = An.AlertTypeId
	where NotificationId = @NotificationId)

	--- If it's In App then we will add 1 more entry to send firebase push notification
	if (@AlertTypeResult = 'In-App')
	BEGIN
	--Inset entry for In APP
	Insert into [HeroAdmin].[dbo].[Admin_NotificationBoradcast] (NotificationId, AlertTypeId, RecipientUserId, Title, Description,
				MobileDeviceId,NotificationCategory)
	select t.NotificationId,n.AlertTypeId,t.RecipientUserids,n.NotificationTitle,n.Description,ud.MobileDeviceId,n.NotificationCategory
	from tempNotification t
	LEFT JOIN [HeroAdmin].[dbo].[Admin_Notification] n on n.NotificationId = t.NotificationId
	LEFT JOIN [HeroAdmin].[dbo].[Admin_AlertType] at on at.AlertTypeId = n.AlertTypeId
	LEFT JOIN [HeroPOSP].[dbo].[POSP_UserDeviceDetail] ud on ud.UserId = t.RecipientUserids
	WHERE n.NotificationId = @NotificationId
	END

	Declare @Alerttypeidforpush VARCHAR(100)
	Set @Alerttypeidforpush = (Select AlertTypeId from [HeroAdmin].[dbo].[Admin_AlertType] where AlertType = 'Push'  and IsActive = 1)


	BEGIN 
	--Insert entry for push notification
	Insert into [HeroAdmin].[dbo].[Admin_NotificationBoradcast] (NotificationId, AlertTypeId , RecipientUserId, Title, Description,
				MobileDeviceId,NotificationCategory)
	select t.NotificationId,@Alerttypeidforpush,t.RecipientUserids,n.NotificationTitle,n.Description,ud.MobileDeviceId,n.NotificationCategory
	from tempNotification t
	LEFT JOIN [HeroAdmin].[dbo].[Admin_Notification] n on n.NotificationId = t.NotificationId
	LEFT JOIN [HeroAdmin].[dbo].[Admin_AlertType] at on at.AlertTypeId = n.AlertTypeId
	LEFT JOIN [HeroPOSP].[dbo].[POSP_UserDeviceDetail] ud on ud.UserId = t.RecipientUserids
	WHERE n.NotificationId = @NotificationId
	END
	DROP TABLE tempNotification

END TRY 
	
BEGIN CATCH        
	DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
	SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
	SET @ErrorDetail=ERROR_MESSAGE()                                  
	EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
	END CATCH      

END 