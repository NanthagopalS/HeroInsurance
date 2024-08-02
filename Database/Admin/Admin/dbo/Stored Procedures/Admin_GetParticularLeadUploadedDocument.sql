  
    
CREATE  PROCEDURE [dbo].[Admin_GetParticularLeadUploadedDocument]         
(        
 @UserId varchar(200)  
)        
AS        
BEGIN        
 BEGIN TRY      
     
   Select IDD.DocumentFileName, IDTM.DocumentType, IDD.DocumentId, IDTM.ShortDescription, IDD.DocumentTypeId, IDD.CreatedOn, IDD.VerifyByUserId, IDD.VerifyOn, IDD.IsVerify, IDD.IsApprove, IDD.IsActive, IDD.UserId, IDD.FileSize, IDD.BackOfficeRemark  
 from [HeroIdentity].[dbo].[Identity_DocumentDetail] IDD WITH(NOLOCK) 
  Left Join [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] IDTM WITH(NOLOCK) on Convert(varchar(50),IDTM.Id) = IDD.DocumentTypeId  
 where UserId = @UserId  AND IDD.IsActive = 1
      
 END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList        
          
 END CATCH        
        
END   
