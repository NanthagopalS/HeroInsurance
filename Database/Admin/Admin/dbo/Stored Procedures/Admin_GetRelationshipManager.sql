-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,23-Feb-2023>      
-- Description: <Description, Admin_GetRelationshipManager>
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_GetRelationshipManager]       
 (      
	@UserId VARCHAR(100) = Null     
 ) 
 As
 Begin  

 BEGIN TRY 
	
	BEGIN
	Select RL.RoleLevelName ,  RM.RoleName, RM.RoleTypeID, IU.UserName, IU.EmailId, IU.UserId, IU.MobileNo from [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK)
	Inner join [HeroAdmin].[dbo].[Admin_RoleMaster] RM WITH(NOLOCK) on  RM.RoleId = IU.RoleId
	Inner join [HeroAdmin].[dbo].[Admin_RoleLevel] RL WITH(NOLOCK) on RL.RoleLevelId = RM.RoleLevelID
	where IU.UserId = @UserId
	END
	
	END TRY 
	
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END 
