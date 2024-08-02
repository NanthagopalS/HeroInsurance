-- =============================================          
-- Author:  <Author,Parth Gandhi>          
-- Create date: <Create Date,>          
-- Description: <Description,,>          
--[dbo].[Insurance_GetRelianceMasterData]'AGREEMENT'           
-- =============================================          
CREATE    PROCEDURE [dbo].[Insurance_GetRelianceMasterData]          
@AddressType VARCHAR(20)          
AS          
BEGIN          
          
 IF(@AddressType='GENDER')            
  SELECT 'Male' NAME, 'MALE' VALUE           
  UNION ALL          
  SELECT 'Female' NAME, 'FEMALE' VALUE      
      
 ELSE IF(@AddressType='MARITALSTATUS')          
  SELECT 'Married' NAME, '1951' VALUE         
  UNION ALL        
  SELECT 'Unmarried' NAME, '1952' VALUE    
    UNION ALL        
  SELECT 'Divorcee' NAME, '1953' VALUE    
    UNION ALL        
  SELECT 'Widow' NAME, '1954' VALUE    
    UNION ALL        
  SELECT 'Widower' NAME, '1955' VALUE    
      
  ELSE IF(@AddressType='SALUTATION')          
  SELECT  Description Name, Description Value FROM [MOTOR].Reliance_Salutation WITH(NOLOCK)          
     
 ELSE IF(@AddressType='NOMINEERELATION')          
  SELECT Text Name,Id Value FROM MOTOR.Reliance_NomineeRelationship WITH(NOLOCK)    
    
  ELSE IF(@AddressType='YESNO')          
  SELECT 'Yes' NAME, 'Yes' VALUE         
  UNION ALL        
  SELECT 'No' NAME, 'No' VALUE         
    
 ELSE IF(@AddressType='FINANCIER')          
  SELECT DISTINCT TXT_FINANCIER_NAME as Name, TXT_FINANCIER_NAME as Value FROM [MOTOR].[Chola_FinancierMaster] WITH(NOLOCK)  
  ORDER BY TXT_FINANCIER_NAME ASC
      
 ELSE IF(@AddressType='STATE')          
  SELECT DISTINCT StateName as Name, StateName as Value FROM MOTOR.Reliance_PincodeMaster WITH(NOLOCK) ORDER BY StateName ASC       
      
 ELSE IF(@AddressType='CITY')          
  SELECT DISTINCT CityOrVillageName as Name, CityOrVillageName as Value FROM MOTOR.Reliance_PincodeMaster WITH(NOLOCK) ORDER BY CityOrVillageName ASC       
 ELSE IF(@AddressType='OCCUPATION')          
  SELECT DISTINCT [Name] as Name, Code as Value FROM [MOTOR].[Reliance_OccupationMaster] WITH(NOLOCK) 
  Where Code IS NOT NULL AND Code <> '' ORDER BY [Name] ASC      
END 