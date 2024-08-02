                  
-- =============================================                  
-- Author:  <Author, Harsh Patel >                  
-- Create date: <Create Date,3-12-2022>                  
-- Description: <Description,,Identity_InsertReUplaodDocument>                  
-- =============================================                  
CREATE   PROCEDURE [dbo].[Identity_InsertReUplaodDocument]                   
(                  
 @UserId VARCHAR(100) = NULL,                    
 @DocumentTypeId VARCHAR(100) = NULL,            
 @DocumentId VARCHAR(100) = NULL,          
 @DocumentFileName VARCHAR(200) = NULL  ,
 @IsAdminUpdating bit = 0              
)                  
AS                  
BEGIN                  
           
   DECLARE @StatusId varchar(500) = NULL        
           
   BEGIN TRY   
     
 UPDATE Identity_DocumentDetail SET IsActive = 0, UpdatedOn = GETDATE() WHERE DocumentTypeId = @DocumentTypeId AND UserId = @UserId  
  
    INSERT INTO Identity_DocumentDetail (UserId, DocumentTypeId,DocumentFileName,VerifyByUserId,DocumentId)                    
      VALUES(@UserId, @DocumentTypeId,@DocumentFileName,'true',@DocumentId)         
        
   --UPDATE Breadcrum stage for PSOP User        
   IF EXISTS(SELECT Id FROM Identity_UserBreadcrumStatusDetail WITH(NOLOCK) WHERE UserId = @UserId AND StatusId IN (SELECT Id from Identity_UserBreadcrumStatusMaster WHERE PriorityIndex IN (2, 3, 4)))        
  BEGIN        
     
 SET @StatusId = (SELECT Id FROM Identity_UserBreadcrumStatusMaster WITH(NOLOCK) WHERE StatusName = 'KYC Pending' AND PriorityIndex = 3)  
     
 UPDATE Identity_UserBreadcrumStatusDetail SET StatusId = @StatusId, UpdatedOn = GETDATE() WHERE UserId = @UserId AND StatusId IN (SELECT Id from Identity_UserBreadcrumStatusMaster WHERE PriorityIndex IN (2, 3, 4))          
           
  END        
  ELSE        
  BEGIN        
   SET @StatusId = (SELECT Id FROM Identity_UserBreadcrumStatusMaster WITH(NOLOCK) WHERE StatusName = 'KYC Pending' AND PriorityIndex = 3)        
        
   INSERT INTO Identity_UserBreadcrumStatusDetail (UserId, StatusId)        
   VALUES(@UserId, @StatusId)        
           
  END        
               
 --UPDATE PROFILE STAGE FOR USERS  
 IF(@IsAdminUpdating = 0)
 BEGIN
	UPDATE Identity_User SET UserProfileStage =4, UpdatedOn = GETDATE() WHERE UserId=@UserId   
 END
 END TRY                                  
 BEGIN CATCH                            
                         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                              
  SET @ErrorDetail=ERROR_MESSAGE()                                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                               
 END CATCH                 
                  
END     
