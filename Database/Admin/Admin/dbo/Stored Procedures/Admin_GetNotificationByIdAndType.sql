CREATE       PROCEDURE [dbo].[Admin_GetNotificationByIdAndType]      
(      
 @UserId VARCHAR(100),      
 @NotificationCategory VARCHAR(100) = NULL,  --(General/Important)      
 @TimeLimit int    
 )      
AS    
BEGIN              
 BEGIN TRY      
      
 BEGIN      
     
  IF Exists (select 1 from Admin_NotificationBoradcast WITH(NOLOCK) WHERE RecipientUserId = @UserId  AND  NotificationCategory=@NotificationCategory and DATEDIFF(minute,CreatedOn,getdate()) <=  @TimeLimit)    
  BEGIN    
      
 select NotificationBoradcastId, NotificationId, Title, Description, CreatedOn, RecipientUserId as UserId, NotificationCategory, ISNULL(IsSeen, 0) as IsSeen, 'New' as NotificationAge    
 from Admin_NotificationBoradcast WITH(NOLOCK)    
 WHERE RecipientUserId = @UserId  AND  NotificationCategory=@NotificationCategory and DATEDIFF(minute,CreatedOn,getdate()) <=  @TimeLimit    
 order by CreatedOn desc    
    
 select NotificationBoradcastId, NotificationId, Title, Description, CreatedOn, RecipientUserId as UserId, NotificationCategory, ISNULL(IsSeen, 0) as IsSeen, 'Old' as NotificationAge    
 from Admin_NotificationBoradcast WITH(NOLOCK)     
 WHERE RecipientUserId = @UserId  AND  NotificationCategory=@NotificationCategory and DATEDIFF(minute,CreatedOn,getdate()) >  @TimeLimit    
 order by CreatedOn desc    
  END    
  ELSE    
  BEGIN    
      
  select NotificationBoradcastId, NotificationId, Title, Description, CreatedOn, RecipientUserId as UserId, NotificationCategory, ISNULL(IsSeen, 0) as IsSeen, 'New' as NotificationAge    
 from Admin_NotificationBoradcast WITH(NOLOCK)     
 WHERE RecipientUserId = @UserId  AND  NotificationCategory=@NotificationCategory     
 and CreatedOn = (select MAX(CreatedOn) from Admin_NotificationBoradcast WHERE RecipientUserId = @UserId  AND  NotificationCategory=@NotificationCategory)    
 order by CreatedOn desc    
    
 select NotificationBoradcastId, NotificationId, Title, Description, CreatedOn, RecipientUserId as UserId, NotificationCategory, ISNULL(IsSeen, 0) as IsSeen, 'Old' as NotificationAge    
 from Admin_NotificationBoradcast WITH(NOLOCK)     
 WHERE RecipientUserId = @UserId  AND  NotificationCategory=@NotificationCategory     
 and CreatedOn < (select MAX(CreatedOn) from Admin_NotificationBoradcast WHERE RecipientUserId = @UserId  AND  NotificationCategory=@NotificationCategory)    
 order by CreatedOn desc    
  END    
 END    
      
 END TRY                              
 BEGIN CATCH              
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                          
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                          
  SET @ErrorDetail=ERROR_MESSAGE()                                          
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                           
 END CATCH              
END       
