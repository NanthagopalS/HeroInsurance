-- exec [Admin_CheckForRole] '7B6ECCA2-1639-4C7C-BD41-88E728A19CA5'
Create   PROCEDURE [dbo].[Admin_CheckForRole] 
(
	@UserId VARCHAR(100) = NULL
)
AS
BEGIN
 BEGIN TRY 
 
		Declare @Category VARCHAR(100), @CheckForRole VARCHAR(100)
		Set @Category = 
		(select UC.UserCategoryId from HeroAdmin.dbo.Admin_UserRoleMapping URM WITH(NOLOCK)
		Join HeroAdmin.dbo.Admin_UserCategory UC WITH(NOLOCK) on URM.CategoryId = UC.UserCategoryId
		where URM.UserId = @UserId)
		
		BEGIN
			IF (@Category = '43B21452-64F8-4301-83D2-BC0F429282E0' or @Category = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C')
				Set @CheckForRole = '1'
			else
				Set @CheckForRole = '0'
		END

		Select @CheckForRole as CheckRole

  END TRY                        
 BEGIN CATCH                               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END