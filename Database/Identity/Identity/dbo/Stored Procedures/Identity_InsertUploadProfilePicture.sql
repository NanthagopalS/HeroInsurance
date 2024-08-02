    
    
CREATE       PROCEDURE [dbo].[Identity_InsertUploadProfilePicture]         
(     
 @UserId VARCHAR(100) = NULL,            
 @ProfilePictureID VARCHAR(100) = NULL,          
 @ProfilePictureFileName VARCHAR(200) = NULL,      
 @ProfilePictureStoragePath VARCHAR(MAX) = NULL ,  
 @DocumentId VARCHAR(100) = NULL  
)          
AS          
  BEGIN       
         
        
  IF EXISTS(SELECT DISTINCT Id FROM [dbo].[Identity_UserDetail] WHERE UserId = @UserId)      
  BEGIN      
   UPDATE [dbo].[Identity_UserDetail] SET       
 ProfilePictureID = @ProfilePictureFileName,    
 ProfilePictureFileName = @ProfilePictureFileName,    
 ProfilePictureStoragePath = @ProfilePictureStoragePath ,  
 DocumentId= @DocumentId , UpdatedOn = GETDATE() 
   WHERE      
    UserId = @UserId     
  END    
      
  ELSE      
  BEGIN      
  BEGIN TRY      
   INSERT INTO [dbo].[Identity_UserDetail] (UserId, ProfilePictureID,ProfilePictureFileName,ProfilePictureStoragePath ,DocumentId)       
   VALUES       
   (@UserId, @ProfilePictureFileName, @ProfilePictureFileName,@ProfilePictureStoragePath,@DocumentId)      
    
       
  END TRY                      
 BEGIN CATCH                
             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList      
        
 END CATCH      
      
END     
END    
  


               
  
