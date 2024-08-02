-- =============================================  
-- Author:  <Author,,AMBI GUPTA>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
--[Insurance_GetGoDigitCityState]'COUNTRY'   
-- =============================================  
CREATE   procedure [dbo].[Insurance_GetBajajProposalMasters]  
@AddressType VARCHAR(20)  
AS  
BEGIN  
  
 IF(@AddressType='City')  
  SELECT DISTINCT RegistractionCity Name, RegistractionCity Value FROM MOTOR.Bajaj_RTOCityMaster WITH(NOLOCK)  
 ELSE IF(@AddressType='State')  
  SELECT DISTINCT State Name, State Value FROM MOTOR.Bajaj_RTOCityMaster WITH(NOLOCK) ORDER BY STATE 
 ELSE IF(@AddressType='Country')  
  SELECT 'India' Name , 'IN' Value  
 ELSE IF(@AddressType='NomineeRelation')  
  SELECT Relation Name,Relation Value FROM MOTOR.Bajaj_RelationMaster WITH(NOLOCK)  
 ELSE IF(@AddressType='Financier')  
   SELECT Name as Value, Name as Name FROM MOTOR.GoDigit_FinancierMaster with(nolock)  
 ELSE IF(@AddressType='Gender')  
  --SELECT GenderValue AS Value,GenderName AS Name FROM Insurance_Gender WITH(NOLOCK)  
    SELECT 'Male' NAME, 'MALE' VALUE   
  UNION ALL  
  SELECT 'Female' NAME, 'FEMALE' VALUE   
 ELSE IF(@AddressType='ADDRESSTYPE')  
  SELECT 'PRIMARY_RESIDENCE' NAME, 'PRIMARY_RESIDENCE' VALUE 
  UNION ALL
  SELECT 'SECONDARY_RESIDENCE' NAME, 'SECONDARY_RESIDENCE' VALUE 
ELSE IF(@AddressType='YESNO')  
  SELECT 'Yes' NAME, 'Yes' VALUE 
  UNION ALL
  SELECT 'No' NAME, 'No' VALUE 
ELSE IF(@AddressType='SELFINSPECTION')  
  SELECT 'Yes' NAME, 'true' VALUE 
  UNION ALL
  SELECT 'No' NAME, 'false' VALUE 
END  
 
 