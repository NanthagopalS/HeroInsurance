-- =============================================      
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,12-May-2022>      
-- Description: <Description,[Admin_GetUserCategory]>       
-- =============================================      
CREATE     PROCEDURE [dbo].[Admin_GetUserCategory]      
      
AS      
BEGIN      
 BEGIN TRY      
   
  select UserCategoryId, UserCategoryName from [HeroAdmin].[dbo].[Admin_UserCategory] WITH(NOLOCK)
       
 END TRY                      
 BEGIN CATCH      
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
END     
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------    