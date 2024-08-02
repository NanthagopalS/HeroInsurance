--[Insurance_GetCKYCMasters] 'PersonalDocument', '85F8472D-8255-4E80-B34A-61DB8678135C','0'    
CREATE   PROCEDURE [dbo].[Insurance_GetCKYCMasters]      
@Type VARCHAR(20) = null,    
@InsurerId VARCHAR(50) = null,    
@IsCompany BIT = NULL    
AS      
BEGIN      
 IF(@Type='Gender')      
  SELECT GenderName AS Name,GenderValue AS Value FROM Insurance_Gender WITH(NOLOCK)      
 ELSE IF(@Type='PersonalDocument')      
 BEGIN    
    
  SELECT DocumentName AS Name,DocumentCode AS Value, Validation AS ValidationObject     
  INTO #Documents_TEMP FROM Insurance_Documents WITH(NOLOCK)     
  WHERE InsurerId = @InsurerId AND CODE = 'personal' AND IsActive = 1    
    
  IF @InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3' AND @IsCompany = 1    
  BEGIN    
   SELECT * FROM #Documents_TEMP WITH(NOLOCK) WHERE Value != 'PASSPORT' AND Value != 'VOTERID' AND Value != 'DRIVINGLICENSE' AND Value != 'AADHARCARDNUMBER' AND Value != 'NREGAJOBCARD' AND Value != 'NATIONALPOPULATIONREGISTERLETTER'    
  END    
  ELSE IF @InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3' AND @IsCompany = 0    
  BEGIN    
   SELECT * FROM #Documents_TEMP WITH(NOLOCK) WHERE Value != 'REGISTRATIONCERTIFICATE' AND Value != 'CERTIFICATEOFINCORPORATION/FORMATION'     
  END 
  ELSE IF @InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C' AND @IsCompany = 0    
  BEGIN    
   SELECT * FROM #Documents_TEMP WITH(NOLOCK) WHERE Value != 'CIN'
  END 
  ELSE IF @InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C' AND @IsCompany = 1   
  BEGIN    
   SELECT * FROM #Documents_TEMP WITH(NOLOCK) WHERE Value = 'CIN'
  END 
  ELSE    
  BEGIN    
   SELECT * FROM #Documents_TEMP WITH(NOLOCK)    
  END    
 END    
    
 ELSE IF(@Type='IdentityDocument')      
 BEGIN    
  SELECT DocumentName AS Name,DocumentCode AS Value, Validation AS ValidationObject INTO #POIDocuments_TEMP FROM Insurance_Documents WITH(NOLOCK) WHERE InsurerId = @InsurerId AND CODE = 'proofOfIdentity' AND IsActive = 1    
  IF @InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3' AND @IsCompany = 1    
  BEGIN    
   SELECT * FROM #POIDocuments_TEMP WITH(NOLOCK) WHERE Value != 'PASSPORT' AND Value != 'VOTER ID' AND Value != 'DRIVING LICENSE' AND Value != 'AADHAR CARD NUMBER' AND Value != 'NREGA JOB CARD' AND Value != 'NATIONAL POPULATION REGISTER LETTER'    
  END    
  ELSE IF @InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3' AND @IsCompany = 0    
  BEGIN    
   SELECT * FROM #POIDocuments_TEMP WITH(NOLOCK) WHERE Value != 'REGISTRATION CERTIFICATE' AND Value != 'CERTIFICATE OF INCORPORATION/FORMATION'     
  END    
  ELSE    
  BEGIN    
   SELECT * FROM #POIDocuments_TEMP WITH(NOLOCK)    
  END    
 END    
 ELSE IF(@Type='AddressDocument')      
 BEGIN    
  SELECT DocumentName AS Name,DocumentCode AS Value, Validation AS ValidationObject INTO #POADocuments_TEMP FROM Insurance_Documents WITH(NOLOCK) WHERE InsurerId = @InsurerId AND CODE = 'proofOfAddress' AND IsActive = 1    
    
  IF @InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3' AND @IsCompany = 1    
  BEGIN    
   SELECT * FROM #POADocuments_TEMP WITH(NOLOCK) WHERE Value != 'PASSPORT' AND Value != 'VOTER ID' AND Value != 'DRIVING LICENSE' AND Value != 'AADHAR CARD NUMBER' AND Value != 'NREGA JOB CARD' AND Value != 'NATIONAL POPULATION REGISTER LETTER'    
  END    
  ELSE IF @InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3' AND @IsCompany = 0    
  BEGIN    
   SELECT * FROM #POADocuments_TEMP WITH(NOLOCK) WHERE Value != 'REGISTRATION CERTIFICATE' AND Value != 'CERTIFICATE OF INCORPORATION/FORMATION'     
  END    
  ELSE    
  BEGIN    
   SELECT * FROM #POADocuments_TEMP WITH(NOLOCK)    
  END    
 END    
    
 ELSE IF(@Type='Customer')      
  SELECT 'INDIVIDUAL' Name, 'I' Value     
  UNION ALL    
  SELECT 'CORPORATE' Name, 'O' Value     
  ELSE IF(@Type='PoliticallyExposed')      
  SELECT 'Yes' Name, 'true' Value     
  UNION ALL    
  SELECT 'No' Name, 'false' Value     
 ELSE IF(@Type='Salutation')      
  SELECT TXT_Salutation AS Name,TXT_Salutation AS Value FROM MOTOR.ITGI_SALUTATIONMaster WITH(NOLOCK)    
   
 ELSE IF(@Type='City')  
 SELECT CITY_DESC AS Name,CITY_CODE AS Value FROM MOTOR.ITGI_CityMasters WITH(NOLOCK)    
  
  ELSE IF(@Type='State')  
 SELECT STATE_NAME AS Name,STATE_CODE AS Value FROM MOTOR.ITGI_StateMasters WITH(NOLOCK)   
END  
