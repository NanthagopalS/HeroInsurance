-- =============================================    
-- Author:  <Author,,Girish>    
-- Create date: <Create Date,,26-Dec-2022>    
-- Description: <Description,[Admin_GetIBULevelDetails]>    
--[Admin_BULevel]    
-- =============================================    
CREATE   PROCEDURE [dbo].[Admin_GetRoleLevelDetails]    
    
AS    
BEGIN    
 BEGIN TRY    
     SET NOCOUNT ON;    
    
  SELECT * FROM [dbo].[Admin_RoleLevel] WITH(NOLOCK) order by PriorityIndex
     
 END TRY                    
 BEGIN CATCH    
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
END    
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
