  
-- =============================================  
-- Author:  <Author,,Harsh Patel >  
-- Create date: <Create Date,,15-12-2022>  
-- Description: <Description,,>  
-- =============================================  
CREATE   PROCEDURE [dbo].[Identity_GetUserListForDepartmentTagging]     
(
	@TaggingType VARCHAR(100) --PreSale, PostSale, Marketing, Claim
)
AS  
BEGIN  
	BEGIN TRY   
  
		SELECT IU.UserId, IU.UserName
		FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] UM
			INNER JOIN Identity_User IU WITH(NOLOCK) ON IU.UserId = UM.UserId
			INNER JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] RM WITH(NOLOCK) ON RM.RoleId = UM.RoleId
		WHERE UM.IsActive = 1 AND IU.IsActive = 1 AND RM.IsActive = 1 AND RM.RoleName = @TaggingType
		ORDER BY IU.UserName
    
 END TRY                          
 BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                       
   
 END CATCH    
END  
