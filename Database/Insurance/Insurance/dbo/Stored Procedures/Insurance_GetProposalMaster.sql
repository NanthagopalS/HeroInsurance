-- EXEC [dbo].[Insurance_GetProposalMaster] '78190CB2-B325-4764-9BD9-5B9806E99621','560001','36'  
-- EXEC [dbo].[Insurance_GetProposalMaster] '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9','560001',''
CREATE PROCEDURE [dbo].[Insurance_GetProposalMaster]  
@InsurerId VARCHAR(50) NULL,    
@Pincode VARCHAR(20) NULL,  
@State VARCHAR(50) NULL  
AS  
BEGIN  
 SET NOCOUNT ON  
 DECLARE @DefaultCityName VARCHAR(100), @DefaultCityValue VARCHAR(100) , @StateRefCode VARCHAR(50) 
  
 BEGIN TRY  
 --HDFC  
  IF @InsurerId = '0A326B77-AFD5-44DA-9871-1742624CFF16'  
  BEGIN  
  
   SELECT DISTINCT TXT_CITYDISTRICT Name, NUM_CITYDISTRICT_CD Value   
   FROM MOTOR.HDFC_CityDistrict WITH(NOLOCK)  
   WHERE NUM_STATE_CD = @State  
   ORDER BY TXT_CITYDISTRICT ASC  
  
   SELECT DISTINCT CITY.TXT_CITYDISTRICT DefaultCityName, CITY.NUM_CITYDISTRICT_CD DefaultCityValue  
   FROM MOTOR.HDFC_CityDistrict CITY WITH(NOLOCK)   
   INNER JOIN MOTOR.HDFC_PincodeMasterAndLocality PIN WITH(NOLOCK) ON CITY.NUM_CITYDISTRICT_CD = PIN.NUM_CITYDISTRICT_CD  
   WHERE PIN.NUM_PINCODE = @Pincode AND CITY.NUM_STATE_CD = @State  
     
  END  
 --Godigit  
  ELSE IF @InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621'  
  BEGIN  
     
   SELECT DISTINCT PN.City Name, PN.City Value  
   FROM MOTOR.GoDigit_PinCode PN WITH(NOLOCK)   
   WHERE State = (SELECT StateName FROM MOTOR.GoDigit_StateMaster WITH(NOLOCK) WHERE StateCode=@State)  
   ORDER BY PN.City ASC  
  
   SELECT DISTINCT PN.City DefaultCityName, PN.City DefaultCityValue  
   FROM MOTOR.GoDigit_PinCode PN WITH(NOLOCK)   
   WHERE Pincode = @Pincode AND State = (SELECT StateName FROM MOTOR.GoDigit_StateMaster WITH(NOLOCK) WHERE StateCode=@State)  
  END  
 --CHOLA  
  ELSE IF @InsurerId ='77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA'  
  BEGIN  
   SELECT DISTINCT CityDistrictDesc Name,CityDistrictDesc Value  
   FROM [HeroInsurance].[MOTOR].[Chola_PincodeMaster] WITH(NOLOCK)  
   WHERE StateDesc = @State  
   ORDER BY CityDistrictDesc ASC  
  
   SELECT DISTINCT CityDistrictDesc DefaultCityName,CityDistrictDesc DefaultCityValue  
   FROM [HeroInsurance].[MOTOR].[Chola_PincodeMaster] WITH(NOLOCK)  
   WHERE Pincode = @Pincode AND StateDesc = @State  
  END  
  --ICICI  
  ELSE IF @InsurerId ='FD3677E5-7938-46C8-9CD2-FAE188A1782C'  
  BEGIN  
   SELECT TRIM(TXT_CITYDISTRICT) Name ,IL_CITYDISTRICT_CD Value   
   FROM MOTOR.ICICI_City_Master WITH(NOLOCK)    
   WHERE IL_CITYDISTRICT_CD !='99999000' AND IL_STATE_CD=@State  
   ORDER BY TRIM(TXT_CITYDISTRICT) ASC  
  
   -- NOT HAVING PINCODE MASTER SO NEED TO VALIDATE WITH DIGIT MASTER  
   SELECT DISTINCT PN.City DefaultCityName, PN.City DefaultCityValue  
   FROM MOTOR.GoDigit_PinCode PN WITH(NOLOCK)   
   WHERE Pincode = @Pincode  
  END  
  --BAJAJ  
  ELSE IF @InsurerId ='16413879-6316-4C1E-93A4-FF8318B14D37'  
  BEGIN  
   SELECT DISTINCT RegistractionCity Name,RegistractionCity Value  
   FROM MOTOR.Bajaj_RTOCityMaster WITH(NOLOCK)  
   WHERE State = @State  
   ORDER BY RegistractionCity ASC  
  
   -- NOT HAVING PINCODE MASTER SO NEED TO VALIDATE WITH DIGIT MASTER  
   SELECT DISTINCT PN.City DefaultCityName, PN.City DefaultCityValue  
   FROM MOTOR.GoDigit_PinCode PN WITH(NOLOCK)   
   WHERE Pincode = @Pincode  
  END  
  -- Reliance
  ELSE IF @InsurerId ='372B076C-D9D9-48DC-9526-6EB3D525CAB6'  
  BEGIN  
   SELECT DISTINCT DistrictName Name,DistrictName Value  
   FROM [MOTOR].[Reliance_PincodeMaster] WITH(NOLOCK)  
   WHERE StateName = @State  
   ORDER BY DistrictName ASC  
  
   SELECT DISTINCT DistrictName DefaultCityName,DistrictName DefaultCityValue  
   FROM [MOTOR].[Reliance_PincodeMaster] WITH(NOLOCK)  
   WHERE Pincode = @Pincode AND StateName = @State  
   END
--IFFCO
  ELSE IF @InsurerId ='E656D5D1-5239-4E48-9048-228C67AE3AC3'  
  BEGIN  
   SELECT DISTINCT CITY_DESC Name, CITY_CODE Value FROM [MOTOR].[ITGI_CityMasters] WITH(NOLOCK)  
   WHERE STATE_CODE = @State
   ORDER BY CITY_DESC ASC

   -- NOT HAVING PINCODE MASTER SO NEED TO VALIDATE WITH DIGIT MASTER  
   SELECT DISTINCT PN.City DefaultCityName, PN.City DefaultCityValue  
   FROM MOTOR.GoDigit_PinCode PN WITH(NOLOCK)   
   WHERE Pincode = @Pincode 
  END  
  --TATA AIG  
  IF @InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C'  
  BEGIN  
  
   SELECT DISTINCT CityName Name, CityName Value   
   FROM [MOTOR].[TATA_PincodeMaster] WITH(NOLOCK)  
   WHERE StateName = @State AND CityName NOT LIKE '%[0-9]%'  
   ORDER BY CityName ASC  
  
   SELECT DISTINCT CityName DefaultCityName, CityName DefaultCityValue  
   FROM [MOTOR].[TATA_PincodeMaster] WITH(NOLOCK)     
   WHERE Pincode = @Pincode AND StateName = @State  
   END
    --Oriental  
  IF @InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9'  
  BEGIN  

   SELECT DISTINCT CityDescription Name, CityCode Value   
   FROM [MOTOR].[Oriental_CityMaster] 
   WHERE StateCode = @State  
   ORDER BY CityDescription ASC  
  
   SELECT DISTINCT CITY.CityDescription DefaultCityName, CITY.CityCode DefaultCityValue  
   FROM [MOTOR].[Oriental_PinCodeMaster] PIN WITH(NOLOCK)  
   INNER JOIN [MOTOR].[Oriental_CityMaster] CITY 
   ON PIN.CityCode = CITY.CityCode
   WHERE PIN.PinCode = @Pincode AND CITY.StateCode = @State 
   ORDER BY CITY.CityDescription ASC    
     
  END  
 END TRY     
 BEGIN CATCH       
   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList              
 END CATCH   
 SET NOCOUNT OFF  
END