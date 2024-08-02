          
-- =============================================          
-- Author:  <Author, Suraj>                    
-- Description: <Get Admin Password to check with Hashing using UserID>          
-- [Identity_GetAdminPasswordByUserId] '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0'           
-- =============================================          
 CREATE   PROCEDURE [dbo].[Identity_GetAdminPasswordByUserId]           
(          
	 @UserId VARCHAR(100) = NULL        
)          
AS          
BEGIN          

	BEGIN TRY          
	IF EXISTS(SELECT TOP 1 UserId FROM Identity_User WITH(NOLOCK) WHERE UserId = @UserId AND IsActive = 1)            
		BEGIN          
			SELECT UserId,[Password] from Identity_User WHERE UserId = @UserId AND IsActive = 1
		END          
	ELSE          
		BEGIN          
			SELECT null as UserId, null as [Password]         
		END          
 END TRY        
         
 BEGIN CATCH                  
  IF @@TRANCOUNT > 0          
  ROLLBACK              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
END