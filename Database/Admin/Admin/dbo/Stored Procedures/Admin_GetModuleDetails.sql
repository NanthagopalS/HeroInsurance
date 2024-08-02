-- =============================================  
-- Author:  <Author,Ankit>  
-- Create date: <Create Date, 3-Feb-2022>  
-- Description: <Description,[GetModuleDetails]>  
--[Admin_Module]  
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_GetModuleDetails]  
(
	@ModuleGroupName VARCHAR(100) = NULL
)  
AS  
BEGIN  
 BEGIN TRY  
     SET NOCOUNT ON; 
	 
	 IF(@ModuleGroupName <> '' OR @ModuleGroupName IS NOT NULL)
	 BEGIN
			SELECT DISTINCT ModuleGroupName, ModuleId, ModuleName, PriorityIndex, AddOption, EditOption, ViewOption, DeleteOption, DownloadOption, IsActive FROM dbo.Admin_Module WITH(NOLOCK) WHERE ModuleGroupName = @ModuleGroupName AND IsActive = 1 ORDER BY PriorityIndex
	 END
	 ELSE
	 BEGIN
			SELECT DISTINCT ModuleGroupName, ModuleId, ModuleName, PriorityIndex, AddOption, EditOption, ViewOption, DeleteOption, DownloadOption, IsActive FROM dbo.Admin_Module WITH(NOLOCK) WHERE  IsActive = 1 ORDER BY ModuleGroupName, PriorityIndex
	 END
   
 END TRY                  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
