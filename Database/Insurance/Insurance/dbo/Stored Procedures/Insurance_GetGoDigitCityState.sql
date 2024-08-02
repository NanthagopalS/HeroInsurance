-- =============================================  
-- Author:  <Author,,AMBI GUPTA>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
--[Insurance_GetGoDigitCityState]'COUNTRY'   
-- =============================================  
CREATE   procedure [dbo].[Insurance_GetGoDigitCityState]  
@AddressType VARCHAR(20)  
AS  
BEGIN  
  
 IF(@AddressType='CITY')  
  SELECT DISTINCT City Name, City Value FROM MOTOR.GoDigit_PinCode WITH(NOLOCK)  
 ELSE IF(@AddressType='STATE')  
  SELECT StateCode Value,StateName Name FROM [MOTOR].[GoDigit_StateMaster] WITH(NOLOCK)  
 ELSE IF(@AddressType='COUNTRY')  
  SELECT 'India' Name , 'IN' Value  
 ELSE IF(@AddressType='NOMINEE')  
  SELECT Nominee_Relation Name,Nominee_Relation Value FROM MOTOR.GoDigit_Nominee WITH(NOLOCK)  
 ELSE IF(@AddressType='FINANCIER')  
  SELECT Name as Value, Name as Name FROM MOTOR.GoDigit_FinancierMaster with(nolock)  
 ELSE IF(@AddressType='GENDER')  
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
