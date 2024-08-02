CREATE PROCEDURE [dbo].[Insurance_GetOrientalQuoteMasterMapping]
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
@IsBrandNew bit = NULL,
@VehicleAge VARCHAR(50) = NULL
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
	@FuelId VARCHAR(50),
	@VehicleBodyType VARCHAR(50),
	@GeoExtensionCountry VARCHAR(50),
	@GeogExtension VARCHAR(50),
	@VoluntaryExcessCode VARCHAR(50),
	@IDVValue VARCHAR(50),
	@DiscountPercentage VARCHAR(50),
	@GetRTOId VARCHAR(50)

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

    SELECT @GeogExtension = ORIENTALEXT.Code
  FROM Insurance_AddOnsExtension EXT WITH(NOLOCK) 
  INNER JOIN Insurance_AddOns PA WITH(NOLOCK) ON PA.AddOnId = EXT.AddOnsId 
  INNER JOIN MOTOR.Oriental_GeoExtensionMaster ORIENTALEXT WITH(NOLOCK) ON EXT.AddOnsExtension = ORIENTALEXT.CountryDescription
  WHERE EXT.IsActive = 1 AND PA.AddOnId IN (SELECT VALUE FROM string_split(@AddonId,',')) 
  AND EXT.AddOnsExtensionId IN (SELECT VALUE FROM string_split(@AddOnsExtensionId,','))

    SELECT @VoluntaryExcessCode = ORIENTALDISEXT.Code  
  FROM Insurance_DiscountExtension EXT WITH(NOLOCK)  
  INNER JOIN Insurance_Discounts PA WITH(NOLOCK) ON PA.DiscountId = EXT.DiscountId
  INNER JOIN MOTOR.Oriental_voluntaryExcessMaster ORIENTALDISEXT WITH
  (NOLOCK) ON ORIENTALDISEXT.Description = EXT.DiscountExtension
  WHERE EXT.IsActive = 1 AND PA.DiscountId IN (SELECT VALUE FROM string_split(@DiscountId,','))   
  AND EXT.DiscountExtensionId IN (SELECT VALUE FROM string_split(@DiscountExtensionId,','))

  SELECT @VehicleMakeCode = VM.VEH_MAKE, 
  @VehicleModelCode = VM.VEH_MODEL,  
  @vehicleCubicCapacity = VM.VEH_CC,
  @VehicleSeatCapacity = VM.VEH_SEAT_CAP,
  @FuelId = VM.VEH_FUEL,
  @VehicleBodyType = VM.VEH_BODY
  FROM MOTOR.Oriental_VehicleMaster VM WITH(NOLOCK) 
  LEFT JOIN Insurance_Variant VA WITH(NOLOCK) ON VA.VariantId = @VariantId
  WHERE VM.VariantId = @VariantId

  IF @VehicleAge < 5 
  BEGIN
	  SELECT @DiscountPercentage = DISC_UPTO_5YRS FROM MOTOR.Oriental_VehicleMaster WHERE VEH_MODEL = @VehicleModelCode
  END
  ELSE IF @VehicleAge >=5 AND @VehicleAge < 10
  BEGIN
	  SELECT @DiscountPercentage = DISC_5_TO_10YRS FROM MOTOR.Oriental_VehicleMaster WHERE VEH_MODEL = @VehicleModelCode
  END

  SELECT @PreviousPolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK)
  WHERE PreviousPolicyTypeId = @PolicyTypeId 

  IF(ISNULL(@SAODInsurerId,'')<>'')	
	SELECT @SAODInsurer = InsurerName FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @SAODInsurerId  
  IF(ISNULL(@SATPInsurerId,'')<>'')	
	SELECT @SATPInsurer = InsurerName FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @SATPInsurerId 

  SELECT @IsRecommended = IsRecommended, 
  @RecommendedDescription = RecommendedDescription 
  FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId --Need to check Not have idea

  SELECT @RTOCode = RTOM.RTOCode
  FROM Insurance_RTO rto WITH(NOLOCK)    
  LEFT JOIN MOTOR.Oriental_RTOMaster RTOM WITH(NOLOCK) on RTOM.RTOId=rto.RTOId  
   WHERE (rto.RTOId = @RTOId OR rto.RTOCode =LEFT(@VehicleNumber,4))

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
	IF @VehicleAge = 0
	BEGIN 
		SELECT @IDVValue = UPTO_6_MONTHS FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 1
	BEGIN 
		SELECT @IDVValue = UPTO_1_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 2
	BEGIN 
		SELECT @IDVValue = UPTO_2_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 3
	BEGIN 
		SELECT @IDVValue = UPTO_3_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 4
	BEGIN 
		SELECT @IDVValue = UPTO_4_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 5
	BEGIN 
		SELECT @IDVValue = UPTO_5_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 6
	BEGIN 
		SELECT @IDVValue = UPTO_7_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 7
	BEGIN 
		SELECT @IDVValue = UPTO_7_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 8
	BEGIN 
		SELECT @IDVValue = UPTO_8_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 9
	BEGIN 
		SELECT @IDVValue = UPTO_9_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 10
	BEGIN 
		SELECT @IDVValue = UPTO_10_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge =11
	BEGIN 
		SELECT @IDVValue = UPTO_11_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 12
	BEGIN 
		SELECT @IDVValue = UPTO_12_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 13
	BEGIN 
		SELECT @IDVValue = UPTO_13_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 14
	BEGIN 
		SELECT @IDVValue = UPTO_14_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END
	ELSE IF @VehicleAge = 15
	BEGIN 
		SELECT @IDVValue = UPTO_15_YEAR FROM MOTOR.Oriental_IDVMaster 
		where INLIAS_VEHICLE_CODE = @VehicleModelCode
	END

  SELECT @VehicleMakeCode AS VehicleMakeCode,
  @VehicleModelCode AS VehicleModelCode,
  @VehicleClass AS VehicleClass,
  @PreviousPolicyType AS PreviousPolicyType,
  @RTOCityCode as CityCode,
  @CityName AS RTOCityName,
  @Zone AS Zone,
  @vehicleCubicCapacity AS vehicleCubicCapacity,
  @VehicleSeatCapacity AS VehicleSeatCapacity,
  @VehicleType as VehicleType,
  @NCBValue AS NCBValue,
  @PolicyType AS CurrentPolicyType,
  @RTOCode AS RTOCode,
  @GeogExtension AS GeogExtension,
  @VoluntaryExcessCode AS VoluntaryExcessCode,
  @FuelId AS FuelId,
  @VehicleBodyType AS VehicleBodyType,
  @IDVValue AS IDVValue,
  @DiscountPercentage AS DiscountPercentage,
  @SAODInsurer AS SAODInsurer,
  @SATPInsurer AS SATPInsurer

	IF(@IsBrandNew = 1)
	BEGIN
		SELECT @PolicyTypeId='20541BE3-D76E-4E73-9AB1-240CCB33DA5D'
	END

	  SELECT ConfigName, ConfigValue FROM Insurance_ICConfig WITH(NOLOCK) 
  WHERE InsurerId=@InsurerId AND PolicyTypeId=@PolicyTypeId AND VehicleTypeId=@VehicleTypeId 

 END TRY                            
 BEGIN CATCH                  
			   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                
 END CATCH
 END