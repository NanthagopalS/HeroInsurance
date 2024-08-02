 -- exec [Identity_GetPOSPUserMaster]   
-- =============================================    
-- Author:  <Author,,AMBI GUPTA>    
-- Create date: <Create Date,,25-Nov-2022>    
-- Description: <Description,[GetInsurer]>    
--[Insurance_GetInsurer]    
-- =============================================    
CREATE        PROCEDURE [dbo].[Identity_GetPOSPUserMaster]    
AS    
BEGIN    
 BEGIN TRY    
  -- SET NOCOUNT ON added to prevent extra result sets from    
  -- interfering with SELECT statements.    
  SET NOCOUNT ON;    

  --SELECT * FROM Identity_EducationQualificationTypeMaster ORDER  BY QualificationID  ASC  
  SELECT Id as BackgroundTypeId, BackgroundType FROM Identity_BackgroundTypeMaster WITH(NOLOCK) ORDER BY BackgroundType    
  SELECT convert(varchar(100), InsurerId) as InsurerCompanyId, InsurerName as InsurerCompanyName FROM [HeroInsurance].[dbo].[Insurance_Insurer] WITH(NOLOCK) where IsActive = 1  ORDER BY InsurerCompanyName  
  SELECT Id as POSPSourceTypeId, POSPSourceType FROM Identity_POSPSourceTypeMaster WITH(NOLOCK) ORDER BY POSPSourceType    
  SELECT Id as PremiumRangeTypeId, PremiumRangeType FROM Identity_PremiumRangeTypeMaster WITH(NOLOCK) ORDER BY OrderBy    
  SELECT CAST(CityId as varchar(50)) as CityId, CityName FROM Identity_City  WITH(NOLOCK) ORDER BY CityName    
  SELECT CAST(StateId AS VARCHAR(50)) as StateId, StateName FROM Identity_State  WITH(NOLOCK) ORDER BY StateName    
  SELECT Id as BankNameId, BankName FROM Identity_BankNameMaster WITH(NOLOCK) ORDER BY BankName    
  SELECT Id as EducationQualificationTypeId, EducationQualificationType FROM Identity_EducationQualificationTypeMaster WITH(NOLOCK) ORDER BY QualificationID  ASC  
  SELECT Id, ProductName, PriorityIndex FROM Identity_InsuranceProductsOfInterest WITH(NOLOCK) order by PriorityIndex  
     
 END TRY                    
 BEGIN CATCH              
           
  
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
END    
    
