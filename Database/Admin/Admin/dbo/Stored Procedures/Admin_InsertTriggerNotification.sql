-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,30-Apr-2023>      
-- Description: <Description, Admin_InsertTriggerNotification>
-- =========================================================================================         
 Create   PROCEDURE [dbo].[Admin_InsertTriggerNotification]       
 (      
	@AlertTypeId VARCHAR(100) = Null,
	@RecipientId VARCHAR(100) = Null,
	@RecipientUserids VARCHAR(100) = Null,
	@NotificationEventId VARCHAR(100) = Null	
 ) 
 As
 Begin  
	BEGIN TRY
	Declare @NotificationTitle VARCHAR(100), @Description VARCHAR(500)
			Set @NotificationTitle = (Select EventTitle from HeroAdmin.dbo.Admin_NotificationEvent)
			Set	@Description = (Select EventDescription from HeroAdmin.dbo.Admin_NotificationEvent)

		Insert into [HeroAdmin].[dbo].[Admin_Notification] 
		(AlertTypeId, RecipientId, RecipientUserids, NotificationTitle, Description, NotificationEventId)
		values (@AlertTypeId, @RecipientId, @RecipientUserids, @NotificationTitle, @Description, @NotificationEventId)
	END TRY
			
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END 