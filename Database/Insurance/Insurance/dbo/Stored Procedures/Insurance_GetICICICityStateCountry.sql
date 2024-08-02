-- =============================================  
-- Author:  <Author,,FIROZ S>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
--[dbo].[Insurance_GetICICICityStateCountry]'YESNO'   
-- =============================================  
CREATE PROCEDURE [dbo].[Insurance_GetICICICityStateCountry]  
@AddressType VARCHAR(20)  
AS  
BEGIN  
  
 IF(@AddressType='CITY') 
  SELECT TXT_CITYDISTRICT Name ,IL_CITYDISTRICT_CD Value FROM MOTOR.ICICI_City_Master WITH(NOLOCK)  where IL_CITYDISTRICT_CD !='99999000' ORDER BY TXT_CITYDISTRICT ASC
 ELSE IF(@AddressType='STATE')  
  SELECT ILState Value,ILStateName Name FROM MOTOR.ICICI_State WITH(NOLOCK) ORDER BY ILStateName ASC
 ELSE IF(@AddressType='COUNTRY')
  SELECT 'INDIA' Name , '100' Value  
 ELSE IF(@AddressType='RELATION')
   SELECT Name as NAME,Name as Value  FROM [HeroInsurance].[MOTOR].[ICICI_PrevInsurer_Relation_Occupation_Financier_Master] WITH(NOLOCK) WHERE MasterType = 'RELATION'
 ELSE IF(@AddressType='FINANCIER')
   SELECT Name as NAME,Name as Value  FROM [HeroInsurance].[MOTOR].[ICICI_PrevInsurer_Relation_Occupation_Financier_Master] WITH(NOLOCK) WHERE MasterType = 'FINANCER'
ELSE IF(@AddressType='YESNO')  
  SELECT 'Yes' NAME, 'Yes' VALUE 
  UNION ALL
  SELECT 'No' NAME, 'No' VALUE
 ELSE IF(@AddressType='GENDER')  
  SELECT 'Male' NAME, 'MALE' VALUE 
  UNION ALL
  SELECT 'Female' NAME, 'FEMALE' VALUE
END  
