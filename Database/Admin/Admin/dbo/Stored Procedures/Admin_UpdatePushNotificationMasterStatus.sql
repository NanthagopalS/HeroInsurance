-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,28-Apr-2023>      
-- Description: <Description, Admin_UpdateNotificationStatus>
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_UpdatePushNotificationMasterStatus]       
 (      
	@NotificationId VARCHAR(100)
 ) 
 As
 Begin  
	BEGIN TRY
		
		Update [HeroAdmin].[dbo].[Admin_Notification] 
		Set IsPublished = 1 , CreatedOn= GETDATE()
		where NotificationId = @NotificationId
	
	END TRY 
	
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END