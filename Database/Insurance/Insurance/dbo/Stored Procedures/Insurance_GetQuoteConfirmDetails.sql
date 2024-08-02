  
CREATE   PROCEDURE [dbo].[Insurance_GetQuoteConfirmDetails]                           
@PolicyTypeId VARCHAR(100) = null,          
@VehicleNumber VARCHAR(100) = null,  
@PreviousSAODInsurerId VARCHAR(100) = null,  
@PreviousSATPInsurerId VARCHAR(100) = null,  
@NCBId VARCHAR(100) = null,  
@InsurerId VARCHAR(100) = null  
AS          
BEGIN          
 BEGIN TRY          
    
  DECLARE @ChasisNo VARCHAR(50),   
  @EngineNo VARCHAR(50),   
  @PreviousPolicyNumber VARCHAR(50),        
  @PolicyType VARCHAR(50),  
  @PreviousSAODInsurerName VARCHAR(100),  
  @PreviousSATPInsurerName VARCHAR(100),  
  @NCBValue VARCHAR(50),  
  @SAODInsurerCode VARCHAR(50),  
  @SATPInsurerCode VARCHAR(50),  
  @NCBName VARCHAR(50)  
    
  SELECT @ChasisNo=chassis,  
  @EngineNo=engine,   
  @PreviousPolicyNumber = vehicleInsurancePolicyNumber   
  FROM Insurance_VehicleRegistration with(nolock)   
  where vehicleNumber = @VehicleNumber --AND VariantId=@VariantId   
  
  IF(ISNULL( @PolicyTypeId, '' ) = '' )      
  BEGIN      
   SELECT @PolicyTypeId = PreviousPolicyTypeId FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D' --COMPREHENSIVE      
  END     
  
 SELECT @PolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType with(nolock) WHERE PreviousPolicyTypeId = @PolicyTypeId    
  
 SELECT @PreviousSAODInsurerName = InsurerName FROM Insurance_Insurer with(nolock) WHERE InsurerId = @PreviousSAODInsurerId  
  SELECT @PreviousSATPInsurerName = InsurerName FROM Insurance_Insurer with(nolock) WHERE InsurerId = @PreviousSATPInsurerId  
  
  IF(@InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37')      
  BEGIN      
 IF(@PreviousSAODInsurerId IS NOT NULL)  
 BEGIN  
  SET @SAODInsurerCode = (SELECT CompanyCode as InsurerCode FROM MOTOR.Bajaj_PrevInsuranceCompanyCode WITH(NOLOCK) WHERE PreviousInsurerId = @PreviousSAODInsurerId)    
 END  
 ELSE  
 BEGIN  
  SET @SAODInsurerCode = (SELECT CompanyCode as InsurerCode FROM MOTOR.Bajaj_PrevInsuranceCompanyCode WITH(NOLOCK) WHERE PreviousInsurerId = @PreviousSAODInsurerId)    
 END  
 SET @SATPInsurerCode = (SELECT InsurerCode FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @PreviousSATPInsurerId)  
  END  
  ELSE IF(@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621')  
  BEGIN  
   SET @SAODInsurerCode = (SELECT InsurerCode FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @PreviousSAODInsurerId)  
   SET @SATPInsurerCode = (SELECT InsurerCode FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @PreviousSAODInsurerId)  
  END  
  ELSE IF(@InsurerId = '0A326B77-AFD5-44DA-9871-1742624CFF16')  
  BEGIN  
   SET @SAODInsurerCode = (SELECT ShortName FROM MOTOR.HDFC_InsuranceMaster WITH(NOLOCK) WHERE InsurerId = @PreviousSAODInsurerId)  
   SET @SATPInsurerCode = (SELECT ShortName FROM MOTOR.HDFC_InsuranceMaster WITH(NOLOCK) WHERE InsurerId = @PreviousSATPInsurerId)  
  END  
  ELSE IF(@InsurerId = 'DC874A12-6667-41AB-A7A1-3BB832B59CEB')  
  BEGIN  
   SET @SAODInsurerCode = (SELECT [COMPANY CODE] FROM [MOTOR].[UIIC_PreviousInsurerMaster] WITH(NOLOCK) WHERE InsurerId = @PreviousSAODInsurerId)  
   SET @SATPInsurerCode = (SELECT [COMPANY CODE] FROM [MOTOR].[UIIC_PreviousInsurerMaster] WITH(NOLOCK) WHERE InsurerId = @PreviousSATPInsurerId)  
  END 
  
  
  IF(ISNULL( @NCBId, '' ) = '' )      
  BEGIN     
  SELECT @NCBId = null  
  END  
  SELECT @NCBValue = NCBValue, @NCBName = NCBName FROM Insurance_NCB with(nolock) WHERE NCBId = @NCBId  
  
  SELECT     
  @ChasisNo Chassis,  
  @EngineNo Engine,  
  @PolicyType CurrentPolicyType,  
  @PreviousSAODInsurerName PreviousSAODInsurerName,  
  @PreviousSATPInsurerName PreviousSATPInsurerName,  
  @NCBValue NCBValue,  
  @SATPInsurerCode SATPInsurerCode,  
  @SAODInsurerCode SAODInsurerCode,  
  @NCBName NCBName  
  
    
 END TRY                          
 BEGIN CATCH                    
        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
END 
