CREATE     PROCEDURE [dbo].[Identity_GetUserProfilePictureDetail]    
@UserId VARCHAR(100) NULL   
AS          
BEGIN          
 BEGIN TRY          
          
      
  SELECT UserId, ProfilePictureID, ProfilePictureFileName, ProfilePictureStoragePath ,DocumentId    
  FROM Identity_UserDetail WITH(NOLOCK)   WHERE UserId =@UserId  
      
     
     
 END TRY                          
 BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
END     




