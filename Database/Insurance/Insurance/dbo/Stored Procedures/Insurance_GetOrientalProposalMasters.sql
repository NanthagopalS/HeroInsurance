
CREATE     procedure [dbo].[Insurance_GetOrientalProposalMasters]  
@AddressType VARCHAR(20)  
AS  
BEGIN  
  
 IF(@AddressType='City')  
  SELECT DISTINCT CityDescription Name, CityCode Value FROM MOTOR.Oriental_CityMaster WITH(NOLOCK)  
 ELSE IF(@AddressType='State')  
  SELECT DISTINCT Description Name, StateCode Value FROM MOTOR.Oriental_StateMaster WITH(NOLOCK)  
 ELSE IF(@AddressType='Country')  
  SELECT 'India' Name , 'IN' Value  
 ELSE IF(@AddressType='PERSONALDOCUMENT')  
  SELECT DocumentName Name , DocumentCode Value, Validation AS ValidationObject FROM Insurance_Documents where InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9'
  	ELSE IF(@AddressType='YESNO')  
		SELECT 'Yes' NAME, 'Yes' VALUE 
		UNION ALL
		SELECT 'No' NAME, 'No' VALUE 
 ELSE IF(@AddressType='Gender')   
    SELECT 'Male' NAME, 'MALE' VALUE   
  UNION ALL  
  SELECT 'Female' NAME, 'FEMALE' VALUE   
END