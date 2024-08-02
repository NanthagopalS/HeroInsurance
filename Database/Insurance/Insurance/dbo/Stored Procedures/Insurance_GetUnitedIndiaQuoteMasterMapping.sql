CREATE   PROCEDURE [dbo].[Insurance_GetUnitedIndiaQuoteMasterMapping]
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
@VehicleAge VARCHAR(50)
AS
BEGIN        
BEGIN TRY 
	DECLARE @VehicleMake VARCHAR(50),
	@VehicleModel VARCHAR(50),
	@PreviousPolicyType VARCHAR(50), 
	@VehicleType VARCHAR(50),
	@VehicleVariant VARCHAR(100),
	@NCBValue VARCHAR(50),
	@SAODInsurer VARCHAR(100),
	@SATPInsurer VARCHAR(100),
	@IsRecommended bit,
	@RecommendedDescription NVARCHAR(MAX),
	@PolicyType VARCHAR(50),
    @RTOCode VARCHAR(50),
	@Zone Varchar(5),
	@vehicleCubicCapacity VARCHAR(50),
	@VehicleSeatCapacity VARCHAR(50),
	@Fuel VARCHAR(50),
	@VehicleBodyType VARCHAR(50),
	@GeoExtensionCountry VARCHAR(50),
	@GeogExtension VARCHAR(50),
	@VoluntaryExcessCode VARCHAR(50),
	@IDVValue VARCHAR(50),
	@DiscountPercentage VARCHAR(50),
	@GetRTOId VARCHAR(50),
	@VehicleModelCode VARCHAR(50),
	@RTOLocationName VARCHAR(100),
	@RTOStateName VARCHAR(100),
	@VehicleCatagory VARCHAR(100),
	@RTODetail VARCHAR(50),
	@RegistrationRTOCode VARCHAR(50),
	@PreviousInsurerCode VARCHAR(50),
	@ExShowRoomPrice VARCHAR(50)

  SET NOCOUNT ON;     

  IF @VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'
  BEGIN
	SET @VehicleCatagory = 'PrivateCar'
  END
  ELSE IF @VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'
  BEGIN
	SET @VehicleCatagory = 'Two Wheeler'
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

  SELECT @RTOCode = RTOM.TXT_RTO_LOCATION_CODE, 
  @RTOLocationName = RTOM.TXT_RTO_LOCATION_DESC,
  @Zone = RTOM.TXT_REGISTRATION_ZONE,
  @RTOStateName = TXT_STATE,
  @RTODetail = rto.RTOCode,
  @RegistrationRTOCode = rto.RTOCode
  FROM Insurance_RTO rto WITH(NOLOCK)    
  LEFT JOIN MOTOR.UIIC_RTOMaster RTOM WITH(NOLOCK) on RTOM.RTOId=rto.RTOId 
   WHERE (rto.RTOId = @RTOId OR rto.RTOCode =LEFT(@VehicleNumber,4)) AND RTOM.VEHICLECLASSDESC = @VehicleCatagory

  SELECT @VehicleMake = VM.Make, 
  @VehicleModel = VM.model,  
  @vehicleCubicCapacity = VM.cubic_cap,
  @VehicleSeatCapacity = VM.seating_cap,
  @Fuel = VM.fuel_type,
  @VehicleVariant = VM.variant,
  @VehicleBodyType = VM.body_type,
  @VehicleModelCode = VM.IBBUniquecode,
  @ExShowRoomPrice = VM.ex_showroom_price
  FROM MOTOR.UIIC_VehicleMaster VM WITH(NOLOCK) 
  LEFT JOIN Insurance_Variant VA WITH(NOLOCK) ON VA.VariantId = @VariantId
  WHERE VM.VariantId = @VariantId AND VM.state_code =LEFT(@RTODetail,2)

  SELECT @PreviousPolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK)
  WHERE PreviousPolicyTypeId = @PolicyTypeId 


	SELECT @SAODInsurer = [COMPANY NAME], @PreviousInsurerCode = [COMPANY CODE] 
	FROM [MOTOR].[UIIC_PreviousInsurerMaster] WITH(NOLOCK) 
	WHERE InsurerId = @SAODInsurerId  

	SELECT @SATPInsurer = [COMPANY NAME] 
	FROM [MOTOR].[UIIC_PreviousInsurerMaster] WITH(NOLOCK) 
	WHERE InsurerId = @SATPInsurerId  

  SELECT @IsRecommended = IsRecommended, 
  @RecommendedDescription = RecommendedDescription 
  FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId --Need to check Not have idea

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

	IF @VehicleAge <= 2
	BEGIN 
		SELECT @IDVValue = '.8'
	END
	ELSE IF @VehicleAge = 3
	BEGIN 
		SELECT @IDVValue = '.7'
	END
	ELSE IF @VehicleAge = 4
	BEGIN 
		SELECT @IDVValue = '.6'
	END
	ELSE IF @VehicleAge = 5
	BEGIN 
		SELECT @IDVValue = '.5'
	END
	ELSE IF @VehicleAge = 6
	BEGIN 
		SELECT @IDVValue = '.4'
	END
	ELSE IF @VehicleAge = 7
	BEGIN 
		SELECT @IDVValue = '.35'
	END
	ELSE IF @VehicleAge = 8
	BEGIN 
		SELECT @IDVValue = '.25'
	END
	ELSE IF @VehicleAge > 9
	BEGIN 
		SELECT @IDVValue = '.2'
	END

  SELECT @VehicleMake AS VehicleMake,
  @VehicleModelCode AS VehicleModelCode,
  @VehicleModel AS VehicleModel,
  @PreviousPolicyType AS PreviousPolicyType,
  @Zone AS Zone,
  @vehicleCubicCapacity AS vehicleCubicCapacity,
  @VehicleSeatCapacity AS VehicleSeatCapacity,
  @VehicleVariant AS VehicleVariant,
  @VehicleType as VehicleType,
  @PolicyType AS CurrentPolicyType,
  @RTOCode AS RTOCode,
  @Fuel AS Fuel,
  @RTOStateName AS RTOStateName,
  @RTOLocationName AS RTOLocationName,
  @RegistrationRTOCode AS RegistrationRTOCode,
  @VehicleBodyType AS VehicleBodyType,
  @SAODInsurer AS SAODInsurer,
  @SATPInsurer AS SATPInsurer,
  @NCBValue AS NCBValue,
  @PreviousInsurerCode AS PreviousInsurerCode,
  @ExShowRoomPrice AS ExShowRoomPrice,
  @IDVValue AS IDVPercentage

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