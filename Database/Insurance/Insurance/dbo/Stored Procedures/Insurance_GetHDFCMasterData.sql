-- =============================================    
-- Author:  <Author,FIROZ S>    
-- Create date: <Create Date,,>    
-- Description: <Description,,>    
--[dbo].[Insurance_GetHDFCMasterData]'AGREEMENT'     
-- =============================================    
CREATE   PROCEDURE [dbo].[Insurance_GetHDFCMasterData]    
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
  ELSE IF(@AddressType='SALUTATION')    
  SELECT  TXT_SALUTATION Name, TXT_SALUTATION Value FROM MOTOR.HDFC_SalutationMaster WITH(NOLOCK)    
 ELSE IF(@AddressType='NOMINEERELATION')    
  SELECT TXT_RELATION_DESC Name,TXT_RELATION_DESC Value FROM MOTOR.HDFC_RelationMaster WITH(NOLOCK) WHERE ISActive = 1
  ELSE IF(@AddressType='YESNO')    
  SELECT 'Yes' NAME, 'Yes' VALUE   
  UNION ALL  
  SELECT 'No' NAME, 'No' VALUE   
 ELSE IF(@AddressType='FINANCIER')    
  SELECT DISTINCT TXT_FINANCIER_NAME as Name, NUM_FINANCIER_CD as Value FROM MOTOR.HDFC_FinancierMaster WITH(NOLOCK)    
 ELSE IF(@AddressType='STATE')    
  SELECT DISTINCT TXT_STATE as Name, NUM_STATE_CD as Value FROM MOTOR.HDFC_State WITH(NOLOCK) ORDER BY TXT_STATE ASC   
 ELSE IF(@AddressType='CITY')    
  SELECT DISTINCT TXT_CITYDISTRICT as Name, NUM_CITYDISTRICT_CD as Value FROM MOTOR.HDFC_CityDistrict WITH(NOLOCK) ORDER BY TXT_CITYDISTRICT ASC    
 ELSE IF(@AddressType='AGREEMENT')    
  SELECT 'Hire Purchase' NAME, 'Hire Purchase' VALUE   
  UNION ALL  
  SELECT 'Hypothecation' NAME, 'Hypothecation' VALUE   
  UNION ALL  
  SELECT 'Lease' NAME, 'Lease' VALUE   
END  
