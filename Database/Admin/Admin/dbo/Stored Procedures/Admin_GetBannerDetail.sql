

CREATE   PROCEDURE [dbo].[Admin_GetBannerDetail]  
  
AS  
BEGIN  
 BEGIN TRY  
      
		
		SELECT BD.BannerId, BD.DocumentId, BD.ProductCategoryId,PC.ProductCategoryName 
		FROM [HeroAdmin].[dbo].[Admin_BannerDetail] as BD WITH(NOLOCK)
		INNER JOIN [HeroInsurance].[dbo].[Insurance_ProductCategory] as PC WITH(NOLOCK) ON PC.ProductCategoryId = BD.ProductCategoryId 
		order by PriorityIndex
   
 END TRY                  
 BEGIN CATCH  

  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
