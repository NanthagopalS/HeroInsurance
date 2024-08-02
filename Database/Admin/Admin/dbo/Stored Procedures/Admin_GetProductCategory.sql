
CREATE PROCEDURE [dbo].[Admin_GetProductCategory]  
  
AS  
BEGIN  
 BEGIN TRY  
      
		
		SELECT ProductCategoryId,ProductCategoryName, IsActive, Icon FROM [HeroInsurance].[dbo].[Insurance_ProductCategory] WITH(NOLOCK) order by PriorityIndex
   
 END TRY                  
 BEGIN CATCH  

  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
