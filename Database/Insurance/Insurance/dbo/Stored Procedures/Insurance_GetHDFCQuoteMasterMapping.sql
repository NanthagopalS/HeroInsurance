-- =============================================    
-- Author:  <Firoz S>    
-- Create date: <01-12-2022>    
-- Description: <Insurance_GetHDFCQuoteMasterMapping>    
-- =============================================    
CREATE   PROCEDURE [dbo].[Insurance_GetHDFCQuoteMasterMapping]      
@PACoverId VARCHAR(2000),      
@AccessoryId VARCHAR(2000),      
@AddonId VARCHAR(2000),      
@DiscountId VARCHAR(2000),    
@InsurerId VARCHAR(50),      
@RTOId varchar(50),      
@VariantId varchar(50),      
@NCBId varchar(50),    
@PolicyTypeId varchar(50),    
@VehicleTypeId varchar(50),    
@PACoverExtensionId VARCHAR(2000),    
@DiscountExtensionId VARCHAR(2000),   
@AddOnsExtensionId VARCHAR(2000)= NULL,  
@VehicleNumber VARCHAR(50)    
AS      
BEGIN      
 BEGIN TRY      
   
   DECLARE @RTOLocationCode varchar(50),@NCBValue varchar(50),@VehicleModelCode varchar(50),    
   @chassis varchar(50), @engine varchar(50), @Customer_State_CD varchar(5), @vehicleSeatCapacity varchar(10),    
   @fuelType varchar(20), @RTOCode varchar(10), @CurrentPolicyType varchar(50), @PreviousPolicyType varchar(50),    
   @IsRecommended VARCHAR(10), @RecommendedDescription NVARCHAR(MAX), @ExtensionName varchar(500),   
   @ExtensionCountryCode varchar(10), @PolicyType VARCHAR(50), @PreviousPolicyNumber varchar(50),   
   @PreviousInsurerCode varchar(50)  
     
   SET NOCOUNT ON;      
    
  IF(ISNULL( @PolicyTypeId, '' ) = '' )    
  BEGIN    
   SELECT @PolicyTypeId = PreviousPolicyTypeId FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D' --COMPREHENSIVE BUNDLE(NB)    
  END  
   
  SELECT PACoverId, PACoverCode AS CoverName   
  FROM Insurance_PACover MP WITH(NOLOCK)  
  WHERE MP.PACoverId IN (SELECT VALUE FROM string_split(@PACoverId,','))  
   
  SELECT AccessoryId, AccessoryCode AS Accessory   
  FROM Insurance_Accessory MP  WITH(NOLOCK)     
  WHERE MP.AccessoryId IN (SELECT VALUE FROM string_split(@AccessoryId,','))  
  
  SELECT AddonId, AddOnCode AS AddOns   
  FROM Insurance_AddOns MP  WITH(NOLOCK)    
  WHERE AddonId IN (SELECT VALUE FROM string_split(@AddonId,','))    
    
  SELECT MP.DiscountId, DiscountCode AS DiscountName   
  FROM Insurance_Discounts MP WITH(NOLOCK)         
  WHERE MP.DiscountId IN (SELECT VALUE FROM string_split(@DiscountId,','))   
    
  SELECT EXT.PACoverExtensionId, EXT.PACoverExtension, EXT.PACoverId    
  FROM Insurance_PACoverExtension as EXT WITH(NOLOCK)     
  INNER JOIN Insurance_PACover as PA WITH(NOLOCK)ON PA.PACoverId = EXT.PACoverId     
  WHERE EXT.IsActive = 1 AND PA.PACoverId IN (SELECT VALUE FROM string_split(@PACoverId,','))     
  AND EXT.PACoverExtensionId IN (SELECT VALUE FROM string_split(@PACoverExtensionId,','))    
    
  SELECT EXT.DiscountExtensionId, EXT.DiscountExtension, EXT.DiscountId    
  FROM Insurance_DiscountExtension as EXT WITH(NOLOCK)    
  INNER JOIN Insurance_Discounts as PA WITH(NOLOCK) ON PA.DiscountId = EXT.DiscountId     
  WHERE EXT.IsActive = 1 AND PA.DiscountId IN (SELECT VALUE FROM string_split(@DiscountId,','))     
  AND EXT.DiscountExtensionId IN (SELECT VALUE FROM string_split(@DiscountExtensionId,','))    
  
  SELECT EXT.AddOnsExtensionId, EXT.AddOnsExtension, EXT.AddOnsId  
  FROM Insurance_AddOnsExtension as EXT WITH(NOLOCK)   
  INNER JOIN Insurance_AddOns as PA WITH(NOLOCK) ON PA.AddOnId = EXT.AddOnsId   
  WHERE EXT.IsActive = 1 AND PA.AddOnId IN (SELECT VALUE FROM string_split(@AddonId,','))   
  AND EXT.AddOnsExtensionId IN (SELECT VALUE FROM string_split(@AddOnsExtensionId,','))  
    
  SELECT @ExtensionName= RTRIM(STRING_AGG(EXT.AddOnsExtension, '-'))  
  FROM Insurance_AddOnsExtension as EXT WITH(NOLOCK)  
  INNER JOIN Insurance_AddOns as PA WITH(NOLOCK) ON PA.AddOnId = EXT.AddOnsId   
  WHERE EXT.IsActive = 1 AND PA.AddOnId IN (SELECT VALUE FROM string_split(@AddonId,','))   
  AND EXT.AddOnsExtensionId IN (SELECT VALUE FROM string_split(@AddOnsExtensionId,','))  
  
  SELECT @ExtensionCountryCode=EXTENSIONCOUNTRYCODE FROM MOTOR.HDFC_ExtensionCountryMaster WITH(NOLOCK)  
  WHERE EXTENSIONCOUNTRYNAME=@ExtensionName  
  
  IF(ISNULL( @ExtensionCountryCode, '' ) = '' )    
  BEGIN    
   SELECT @ExtensionCountryCode='0'  
  END  
  
  IF(@VehicleTypeId='6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056')    
  BEGIN    
   SELECT @RTOLocationCode = BRCM.RTO_CODE, @Customer_State_CD=BRCM.NUM_STATE_ID, @RTOCode=rto.RTOCode    
   FROM Insurance_RTO rto WITH(NOLOCK)     
   LEFT OUTER JOIN MOTOR.HDFC_RTOMasterMotor BRCM WITH(NOLOCK) ON BRCM.RTOId=rto.RTOId    
   WHERE (rto.RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4))    
  END    
  ELSE IF(@VehicleTypeId='2d566966-5525-4ed7-bd90-bb39e8418f39')    
  BEGIN    
   SELECT @RTOLocationCode = BRCM.RTO_CODE, @Customer_State_CD=BRCM.NUM_STATE_ID, @RTOCode=rto.RTOCode    
   FROM Insurance_RTO rto WITH(NOLOCK)     
   LEFT OUTER JOIN MOTOR.HDFC_RTOMasterCar BRCM WITH(NOLOCK) ON BRCM.RTOId=rto.RTOId    
   WHERE (rto.RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4))    
  END  
  ELSE IF(@VehicleTypeId='88a807b3-90e4-484b-b5d2-65059a8e1a91')  
  BEGIN  
	SELECT @RTOLocationCode = BRCM.TXT_RTO_LOCATION_CODE, @Customer_State_CD=BRCM.NUM_STATE_CODE, @RTOCode=rto.RTOCode  
	FROM Insurance_RTO rto WITH(NOLOCK)   
	LEFT OUTER JOIN [MOTOR].[HDFC_CV_RTOMaster] BRCM WITH(NOLOCK) ON BRCM.RTOId=rto.RTOId  
	WHERE (rto.RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4))  
  END  
    
  SELECT @VehicleModelCode=VEHICLEMODELCODE, @vehicleSeatCapacity = (SEATINGCAPACITY-1),@fuelType=TXT_FUEL     
  FROM [MOTOR].[HDFC_VehicleMaster] WITH(NOLOCK) WHERE Variantid = @VariantId    
    
  SELECT  @chassis=chassis,     
  @engine=engine,     
  @VehicleNumber=vehicleNumber,  
  @PreviousPolicyNumber=vehicleInsurancePolicyNumber  
  FROM Insurance_VehicleRegistration with(nolock) where VariantId=@VariantId AND regNo=@VehicleNumber    
    
  IF(ISNULL( @NCBId, '' ) = '')  
   BEGIN  
   SET @NCBValue = 0  
   END  
   ELSE  
   BEGIN  
   SET @NCBValue = (SELECT NCBValue FROM Insurance_NCB WITH(NOLOCK) WHERE NCBId = @NCBId)      
   END  
   
   
  IF (@PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B')    
  BEGIN    
   SELECT @CurrentPolicyType=''    
   SELECT @PreviousPolicyType='TP'    
  END    
  ELSE IF (@PolicyTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896')    
  BEGIN    
   SELECT @CurrentPolicyType = 'OD Only'    
   SELECT @PreviousPolicyType='Comprehensive Package'    
  END    
  ELSE    
  BEGIN    
   -- No previous policy and Comprehensive package    
   SELECT @CurrentPolicyType = 'OD Plus TP'    
   SELECT @PreviousPolicyType='Comprehensive Package'    
  END    
    
  SELECT @IsRecommended = IsRecommended, @RecommendedDescription = RecommendedDescription FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId    
    
  SELECT @PreviousInsurerCode=ShortName FROM MOTOR.HDFC_InsuranceMaster WITH(NOLOCK) WHERE InsurerId=@InsurerId  
  
  SELECT @PolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = @PolicyTypeId  
  
  SELECT  @RTOLocationCode as RTOLocationCode,     
  @NCBValue as NCBValue,     
  @VehicleModelCode VehicleModelCode,     
  @chassis chassis,    
  @engine engine,    
  @vehicleSeatCapacity vehicleSeatCapacity,    
  @Customer_State_CD  State_Id,    
  @fuelType Fuel,    
  @RTOCode RTOCode,    
  @CurrentPolicyType CurrentPolicyType,    
  @PreviousPolicyType PreviousPolicyType,     
  @IsRecommended IsRecommended,   
  @RecommendedDescription RecommendedDescription,  
  @ExtensionName GeogExtension,  
  @ExtensionCountryCode ExtensionCountryCode,  
  @PolicyType OriginalPreviousPolicyType,  
  @PreviousPolicyNumber PreviousInsurancePolicyNumber,  
  @PreviousInsurerCode PreviousInsurerCode  
   
  SELECT ConfigName, ConfigValue FROM Insurance_ICConfig WITH(NOLOCK) WHERE InsurerId=@InsurerId AND PolicyTypeId=@PolicyTypeId AND VehicleTypeId=@VehicleTypeId    
   
    
 END TRY                      
 BEGIN CATCH                          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
END 