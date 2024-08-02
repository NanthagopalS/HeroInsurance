  
CREATE       PROCEDURE [dbo].[POSP_GetProductCategory]          
AS        
BEGIN        
 BEGIN TRY        
 SELECT InsuranceTypeId, InsuranceName, InsuranceType, ProductCategoryId, ImageURL, IsActive, IsEnable  
 FROM [HeroInsurance].[dbo].[Insurance_InsuranceType] WITH(NOLOCK)  
 ORDER BY  PriorityIndex ASC --InsuranceType, InsuranceName 
  
  END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END   