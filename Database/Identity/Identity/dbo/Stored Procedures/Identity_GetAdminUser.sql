  
-- =============================================  
-- Author:  <Author, Girish>  
-- Create date: <Create Date,17-Dec-2022>  
-- Description: <Description,GetINSERT Admin>  
--Identity_InsertLogUser 'GirishSF@Mantralabsglobal.com','XXXXXX'  
-- =============================================  
 CREATE PROCEDURE [dbo].[Identity_GetAdminUser]   
(  
 @EmailId VARCHAR(100) = NULL  
)  
AS  
BEGIN  
 BEGIN TRY  
    
  SELECT Top 1   
  USR.UserId as UserId,USR.EmailId as EmailId,USR.[Password] [Password],  
  ROLM.RoleId,ROLM.RoleName  
  FROM Identity_User USR WITH(NOLOCK)  
  JOIN Identity_RoleMaster ROLM WITH(NOLOCK) ON USR.RoleId=ROLM.RoleId  
  WHERE ROLM.RoleId = 'F86D7C1A-244C-4E7E-AD49-DB658DD5E0FB' AND EmailId = @EmailId  AND USR.IsActive = 1
  
 END TRY                  
 BEGIN CATCH    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END
