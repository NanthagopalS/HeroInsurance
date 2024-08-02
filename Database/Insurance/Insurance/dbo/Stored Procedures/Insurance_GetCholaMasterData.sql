-- =============================================      
-- Author:  <Author,Parth Gandhi>      
-- Create date: <Create Date,23-May-2023>      
-- Description: <Description,Getting master data for chola>      
--[dbo].[Insurance_GetCholaMasterData]'GENDER'       
-- =============================================      
CREATE   PROCEDURE [dbo].[Insurance_GetCholaMasterData]      
@AddressType VARCHAR(20)      
AS      
BEGIN      
      
 IF(@AddressType='GENDER')      
  SELECT 'Male' NAME, 'MALE' VALUE     
  UNION ALL    
  SELECT 'Female' NAME, 'FEMALE' VALUE     
   
 ELSE IF(@AddressType='MARITALSTATUS')      
  SELECT 'Married' NAME, 'Married' VALUE     
  UNION ALL    
  SELECT 'Unmarried' NAME, 'Unmarried' VALUE     
    
  ELSE IF(@AddressType='PINCODE')      
  SELECT DISTINCT PinCode Name, PinCode Value FROM MOTOR.Chola_PincodeMaster WITH(NOLOCK)  
  
 ELSE IF(@AddressType='NOMINEERELATION')      
  SELECT 'Self' NAME, 'Self' VALUE     
  UNION ALL    
  SELECT 'Spouse' NAME, 'Spouse' VALUE     
  UNION ALL    
  SELECT 'Father' NAME, 'Father' VALUE  
  UNION ALL    
  SELECT 'Mother' NAME, 'Mother' VALUE  
  UNION ALL    
  SELECT 'Son' NAME, 'Son' VALUE  
  UNION ALL    
  SELECT 'Daughter' NAME, 'Daughter' VALUE  
  UNION ALL    
  SELECT 'Others' NAME, 'Others' VALUE  
    
 ELSE IF(@AddressType='YESNO')      
  SELECT 'Yes' NAME, 'Yes' VALUE     
  UNION ALL    
  SELECT 'No' NAME, 'No' VALUE     
   
 ELSE IF(@AddressType='FINANCIER')      
  SELECT DISTINCT TXT_FINANCIER_NAME as Name, TXT_FINANCIER_NAME as Value   
  FROM [MOTOR].[Chola_FinancierMaster] WITH(NOLOCK)  
  
 ELSE IF(@AddressType='STATE')    
  SELECT DISTINCT StateDesc as Name, StateDesc as Value   
  FROM [HeroInsurance].[MOTOR].[Chola_PincodeMaster] WITH(NOLOCK)  
  ORDER BY StateDesc  
     
 ELSE IF(@AddressType='CITY')      
  SELECT DISTINCT CityDistrictDesc as Name, CityDistrictDesc as Value   
  FROM [HeroInsurance].[MOTOR].[Chola_PincodeMaster] WITH(NOLOCK)  
  ORDER BY CityDistrictDesc   
END