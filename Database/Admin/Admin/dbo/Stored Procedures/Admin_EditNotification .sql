-------- =============================================  
-- Author:  <Author, Ankit>  
-- Create date: <Create Date, 6-Apr-2023>  
-- Description: <Description,,Admin_EditNotification >  
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_EditNotification ]   
(  
	@NotificationId varchar(100),
	@AlertTypeId varchar(100),
	@RecipientId varchar(100),
	@UserIds varchar(max),
	@Title varchar(100),
	@Description varchar(max)
)  
AS  
BEGIN  
 BEGIN TRY   
	
	BEGIN

	Update [HeroAdmin].[dbo].[Admin_Notification]
	Set AlertTypeId = @AlertTypeId, RecipientId = @RecipientId, RecipientUserids = @UserIds, NotificationTitle = @Title, Description = @Description
	where NotificationId = @NotificationId

	END

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
