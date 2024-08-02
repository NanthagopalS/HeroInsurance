
-- =============================================
-- Author:		<Author,Girish >
-- Create date: <Create Date,2/jan/2023>
-- Description:	<Description,Admin_GetBUDetails>
-- =============================================
CREATE PROCEDURE [dbo].[Admin_GetBUDetails] 
(
	@RoleTypeId VARCHAR(50) = NULL
)
AS
BEGIN
 BEGIN TRY        
	
	IF(@RoleTypeId <> '' or @RoleTypeId IS NOT NULL or @RoleTypeId != 'null')
	BEGIN
		Select BUId, BUName from [dbo].[Admin_BU] (NOLOCK) WHERE IsActive = 1 --where RoleTypeId = @RoleTypeId 
		ORDER BY CreatedOn DESC
	END
	ELSE
	BEGIN
		Select BUId, BUName from [dbo].[Admin_BU] (NOLOCK) WHERE IsActive = 1 ORDER BY CreatedOn DESC
	END

  END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END

