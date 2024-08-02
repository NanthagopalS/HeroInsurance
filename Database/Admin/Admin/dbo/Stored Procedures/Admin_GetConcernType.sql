-- =============================================  
-- Author:  <Author,Ankit>  
-- Create date: <Create Date, 10-Apr-2022>  
-- Description: <Description,[GetConcernType]>  
--[Admin_Module]  
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_GetConcernType]  
 
AS  
BEGIN  
 BEGIN TRY  
	
	Select ConcernTypeId, ConcernTypeName from dbo.[Admin_ConcernType] (NOLOCK)
	
   
 END TRY                  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
