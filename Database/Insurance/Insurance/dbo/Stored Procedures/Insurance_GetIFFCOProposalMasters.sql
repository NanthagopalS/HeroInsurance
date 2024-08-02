-- =============================================  
-- Author:  <Author,,AMBI GUPTA>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
--[Insurance_GetGoDigitCityState]'COUNTRY'   
-- =============================================  
CREATE     procedure [dbo].[Insurance_GetIFFCOProposalMasters]  
@AddressType VARCHAR(20)  
AS  
BEGIN  
  
	IF(@AddressType='SALUTATION')  
		SELECT Mode Value,Mode Name FROM [MOTOR].[ITGI_SalutationMaster] WITH(NOLOCK) 

	ELSE IF(@AddressType='GENDER') 
		    SELECT 'Male' NAME, 'MALE' VALUE   
			UNION ALL  
			SELECT 'Female' NAME, 'FEMALE' VALUE
		--SELECT Mode Value,Mode Name FROM [MOTOR].[ITGI_GenderMaster] WITH(NOLOCK) 

	ELSE IF(@AddressType='maritalStatus')  
		SELECT MaritalStatusCode Value,Marital_Status Name FROM [MOTOR].[ITGI_MaritalStatusMaster] WITH(NOLOCK) 

	 ELSE IF(@AddressType='CITY')  
		SELECT DISTINCT CITY_DESC Name, CITY_CODE Value FROM [MOTOR].[ITGI_CityMasters] WITH(NOLOCK)  

	 ELSE IF(@AddressType='STATE')  
		SELECT STATE_CODE Value, STATE_NAME Name FROM [MOTOR].[ITGI_StateMasters] WITH(NOLOCK)  
	   
	 ELSE IF(@AddressType='NOMINEE')  
		SELECT NOMINEE_RELATIONSHIP Name,NOMINEE_RELATIONSHIP Value FROM [MOTOR].[ITGI_NomineeRelationMaster] WITH(NOLOCK)  

	 ELSE IF(@AddressType='FINANCIER')  
		SELECT Financier as Value, Financier as Name FROM [MOTOR].[ITGI_FinancierMaster] with(nolock)  

	 ELSE IF(@AddressType='occupation')  
		SELECT OccupationCode as Value, OCCUPATION as Name FROM MOTOR.ITGI_OccupationMaster with(nolock)  

	ELSE IF(@AddressType='YESNO')  
		SELECT 'Yes' NAME, 'Yes' VALUE 
		UNION ALL
		SELECT 'No' NAME, 'No' VALUE 
END