CREATE PROCEDURE [dbo].[Insurance_GetIFFCOQuoteMasterMapping]
@PACoverId VARCHAR(2000) = NULL,        
@AccessoryId VARCHAR(2000) = NULL,        
@AddonId VARCHAR(2000) = NULL,       
@DiscountId VARCHAR(2000) = NULL,      
@InsurerId VARCHAR(50) = NULL,        
@RTOId VARCHAR(50) = NULL,        
@VariantId VARCHAR(50) = NULL,        
@NCBId VARCHAR(50) = NULL,      
@PolicyTypeId VARCHAR(50) = NULL,      
@VehicleTypeId VARCHAR(50) = NULL,  
@VehicleNumber VARCHAR(50) = NULL,  
@DiscountExtensionId NVARCHAR(MAX) = NULL,  
@PACoverExtensionId VARCHAR(100) = NULL,
@AddOnsExtensionId NVARCHAR(MAX) = NULL,
@SAODInsurerId VARCHAR(100) = NULL,  
@SATPInsurerId VARCHAR(100) = NULL,
@IsBrandNew bit = NULL
AS
BEGIN        
BEGIN TRY 
	DECLARE @VehicleMakeCode VARCHAR(50),
	@VehicleModelCode VARCHAR(50),
	@PreviousPolicyType VARCHAR(50), 
	@VehicleClass varchar(50),
	@VehicleType VARCHAR(50),
	@CityName VARCHAR(50),
	@NCBValue VARCHAR(50),
	@SAODInsurer VARCHAR(100),
	@SATPInsurer VARCHAR(100),
	@IsRecommended bit,
	@RecommendedDescription NVARCHAR(MAX),
	@PolicyType VARCHAR(50),
	@RTOCityCode VARCHAR(50),
    @RTOCode VARCHAR(50),
	@Zone Varchar(5),
	@vehicleCubicCapacity VARCHAR(50),
	@VehicleSeatCapacity VARCHAR(50),
	@VehicleSubTypeCode VARCHAR(50),
	@GrossVehicleWeight VARCHAR(50),
	@ContractType VARCHAR(50)

  SET NOCOUNT ON;     
  
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

  SELECT EXT.DiscountExtensionId, EXT.DiscountExtension, EXT.DiscountId  
  FROM Insurance_DiscountExtension as EXT WITH(NOLOCK)  
  INNER JOIN Insurance_Discounts as PA WITH(NOLOCK) ON PA.DiscountId = EXT.DiscountId   
  WHERE EXT.IsActive = 1 AND PA.DiscountId IN (SELECT VALUE FROM string_split(@DiscountId,','))   
  AND EXT.DiscountExtensionId IN (SELECT VALUE FROM string_split(@DiscountExtensionId,','))

  SELECT EXT.PACoverExtensionId, EXT.PACoverExtension, EXT.PACoverId
  FROM Insurance_PACoverExtension as EXT WITH(NOLOCK) 
  INNER JOIN Insurance_PACover as PA WITH(NOLOCK) ON PA.PACoverId = EXT.PACoverId 
  WHERE EXT.IsActive = 1 AND PA.PACoverId IN (SELECT VALUE FROM string_split(@PACoverId,',')) 
  AND EXT.PACoverExtensionId IN (SELECT VALUE FROM string_split(@PACoverExtensionId,','))

  SELECT EXT.AddOnsExtensionId, EXT.AddOnsExtension, EXT.AddOnsId
  FROM Insurance_AddOnsExtension as EXT WITH(NOLOCK) 
  INNER JOIN Insurance_AddOns PA WITH(NOLOCK) ON PA.AddOnId = EXT.AddOnsId 
  WHERE EXT.IsActive = 1 AND PA.AddOnId IN (SELECT VALUE FROM string_split(@AddonId,',')) 
  AND EXT.AddOnsExtensionId IN (SELECT VALUE FROM string_split(@AddOnsExtensionId,','))

  IF @VehicleTypeId = '88a807b3-90e4-484b-b5d2-65059a8e1a91'
  BEGIN
	SET @ContractType = 'CVI'
  END
  ELSE IF @VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'
  BEGIN
	SET @ContractType = 'PCP'
  END
  ELSE
  BEGIN
	SET @ContractType = 'TWP'
  END

  SELECT @VehicleMakeCode = MAKE_CODE, 
  @VehicleModelCode = MODEL,  
  @VehicleClass = CONTRACT_TYPE,
  @vehicleCubicCapacity = CC,
  @VehicleSeatCapacity = VM.SEATING_CAPACITY,
  @VehicleSubTypeCode = VM.SubClass,
  @GrossVehicleWeight = VM.GVW
  FROM MOTOR.ITGI_VehicleMaster VM WITH(NOLOCK) 
  LEFT JOIN Insurance_Variant VA WITH(NOLOCK)  ON VA.VariantId = @VariantId
  WHERE VM.VariantId = @VariantId AND VM.CONTRACT_TYPE = @ContractType

  SELECT @PreviousPolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK)
  WHERE PreviousPolicyTypeId = @PolicyTypeId 

  IF(ISNULL(@SAODInsurerId,'')<>'')	
	SELECT @SAODInsurer = InsurerName FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @SAODInsurerId  
  IF(ISNULL(@SATPInsurerId,'')<>'')	
	SELECT @SATPInsurer = InsurerName FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @SATPInsurerId 

  SELECT @IsRecommended = IsRecommended, 
  @RecommendedDescription = RecommendedDescription 
  FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId --Need to check Not have idea

  SELECT @RTOCityCode =IRCM.RTO_CITY_CODE,@CityName=IRCM.RTO_CITY_NAME,@Zone=IRCM.IRDA_ZONE, @RTOCode = rto.RTOCode
  FROM Insurance_RTO rto WITH(NOLOCK)    
  LEFT JOIN MOTOR.ITGI_RTO_CityMaster IRCM WITH(NOLOCK) on IRCM.RTOId=rto.RTOId  
   WHERE (rto.RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4))

  SELECT @VehicleType = InsuranceName  
  FROM Insurance_InsuranceType WITH(NOLOCK) WHERE InsuranceTypeId = @VehicleTypeId

  IF(ISNULL( @NCBId, '' ) = '')
	BEGIN
		SET @NCBValue = 0
	END
  ELSE
	BEGIN
		SET @NCBValue = (SELECT NCBValue FROM Insurance_NCB WITH(NOLOCK) WHERE NCBId = @NCBId)    
	END

  IF(ISNULL( @PolicyTypeId, '' ) = '' )    
	BEGIN    
		SELECT @PolicyTypeId = PreviousPolicyTypeId FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D' --COMPREHENSIVE    
	END 

	SELECT @PolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = @PolicyTypeId

  SELECT @VehicleMakeCode AS VehicleMakeCode,
  @VehicleModelCode AS VehicleModelCode,
  @VehicleClass AS VehicleClass,
  @PreviousPolicyType AS PreviousPolicyType,
  @RTOCityCode as CityCode,
  @CityName AS RTOCityName,
  @Zone AS Zone,
  @vehicleCubicCapacity vehicleCubicCapacity,
  @VehicleSeatCapacity AS VehicleSeatCapacity,
  @VehicleType as VehicleType,
  @NCBValue AS NCBValue,
  @PolicyType AS CurrentPolicyType,
  @RTOCode AS RTOCode,
  @VehicleSubTypeCode AS VehicleSubTypeCode,
  @GrossVehicleWeight AS GrossVehicleWeight

	IF(@IsBrandNew = 1)
	BEGIN
		SELECT @PolicyTypeId='20541BE3-D76E-4E73-9AB1-240CCB33DA5D'
	END
 END TRY                        
 BEGIN CATCH                  
			   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                
 END CATCH
 END