-- =============================================        
-- Author:  <Author,Madhu>        
-- Create date: <Create Date,,>        
-- Description: <Description,,>        
--[dbo].[Insurance_GetHDFCMasterData]'AGREEMENT'         
-- =============================================        
CREATE   PROCEDURE [dbo].[Insurance_GetUIICMasterData]        
@AddressType VARCHAR(20)        
AS        
BEGIN            
 IF(@AddressType='GENDER')        
  SELECT 'Male' NAME, 'Male' VALUE       
  UNION ALL      
  SELECT 'Female' NAME, 'Female' VALUE             
 ELSE IF(@AddressType='NOMINEERELATION')        
  SELECT RELATION Name,RELATION Value FROM MOTOR.UIIC_NomineeMaster WITH(NOLOCK)  
  ELSE IF(@AddressType='YESNO')        
  SELECT 'Yes' NAME, 'Yes' VALUE       
  UNION ALL      
  SELECT 'No' NAME, 'No' VALUE       
 ELSE IF(@AddressType='FINANCIER')        
  SELECT DISTINCT [Financier Name] as Name, [Financier Code] as Value FROM MOTOR.UIIC_FinancierMaster WITH(NOLOCK)  
  ELSE IF(@AddressType='FINANCIERBRANCH')        
  SELECT DISTINCT BranchName as Name, FinancierBranchCode as Value FROM MOTOR.UIIC_FinancierBranchMaster WITH(NOLOCK)    
 ELSE IF(@AddressType='STATE')        
  SELECT DISTINCT [STATE NAME] as Name, [STATE CODE] as Value FROM MOTOR.UIIC_StateMaster WITH(NOLOCK) ORDER BY [STATE NAME] ASC           
 ELSE IF(@AddressType='AGREEMENT')        
  SELECT 'Hire Purchase' NAME, 'Hire Purchase' VALUE       
  UNION ALL      
  SELECT 'Hypothecation' NAME, 'Hypothecation' VALUE       
  UNION ALL      
  SELECT 'Lease' NAME, 'Lease' VALUE    
END