-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date, 2-May-2023>      
-- Description: <Description, Admin_UpdateNotificationViewStatus>
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_UpdateNotificationViewStatus]       
 (      
	@NotificationBoradcastId VARCHAR(100) = Null	
 ) 
 As
 Begin  
	BEGIN TRY
		Update [HeroAdmin].[dbo].[Admin_NotificationBoradcast]
		Set IsSeen = 1, CreatedOn= GETDATE()
		where NotificationBoradcastId = @NotificationBoradcastId
	END TRY
		
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END