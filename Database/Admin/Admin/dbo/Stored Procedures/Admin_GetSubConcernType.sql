-- =============================================  
-- Author:  <Author,Ankit>  
-- Create date: <Create Date, 10-Apr-2022>  
-- Description: <Description,[GetSubConcernType]>  
--[Admin_Module]  
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_GetSubConcernType]  
(
	@ConcernTypeId VARCHAR(100)
)
AS  
BEGIN  
 BEGIN TRY  
	
	Select SubConcernTypeId, SubConcernTypeName from dbo.[Admin_SubConcernType] WITH(NOLOCK) where ConcernTypeId = @ConcernTypeId
	
   
 END TRY                  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
