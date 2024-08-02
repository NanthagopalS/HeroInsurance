-------- =============================================  
-- Author:  <Author, Ankit>  
-- Create date: <Create Date, 24-Feb-2023>  
-- Description: <Description,,Admin_GetBUHierarchy>  
-- =============================================  
CREATE    PROCEDURE [dbo].[Admin_GetBUHierarchy]   
 
AS  
BEGIN  
 BEGIN TRY   
  SELECT bu.BULevelId as HierarchyLevelId, bu.BULevelName as HierarchyLevelName 
		From [Admin_BULevel] (NOLOCK) bu order by PriorityIndex

 END TRY                  
 BEGIN CATCH            
 IF @@TRANCOUNT > 0  
        ROLLBACK    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END 
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
