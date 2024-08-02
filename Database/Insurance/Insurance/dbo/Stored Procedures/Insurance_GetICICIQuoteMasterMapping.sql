
CREATE PROCEDURE [dbo].[Insurance_GetICICIQuoteMasterMapping]        
@PACoverId VARCHAR(2000) = null,        
@AccessoryId VARCHAR(2000) = null,        
@AddonId VARCHAR(2000) = null,       
@DiscountId VARCHAR(2000) = null,      
@InsurerId VARCHAR(50) = null,        
@RTOId VARCHAR(50) = null,        
@VariantId VARCHAR(50) = null,        
@NCBId VARCHAR(50) = null,      
@PolicyTypeId VARCHAR(50) = null,      
@VehicleTypeId VARCHAR(50) = null,  
@VehicleNumber VARCHAR(50) = null,  
@DiscountExtensionId NVARCHAR(MAX) = null,  
@PACoverExtensionId VARCHAR(100) = null,
@AddOnsExtensionId NVARCHAR(MAX) = null,
@SAODInsurerId VARCHAR(100) = null,  
@SATPInsurerId VARCHAR(100) = null,
@IsBrandNew bit = NULL
  
AS        
BEGIN        
 BEGIN TRY        
		
  DECLARE @VehicleMakeCode VARCHAR(50), @VehicleModelCode VARCHAR(50), @RTOLocationCode VARCHAR(50),      
  @ExShowRoomPrice VARCHAR(50), @ManufactureDate vARCHAR(50), @regDate varchar(50), @GSTToState VARCHAR(50),   
  @RegistrationDate VARCHAR(50), @PreviousPolicyType VARCHAR(50), @VehicleClass varchar(50),  
  @VehicleType VARCHAR(50), @CItyName VARCHAR(50), @StateName VARCHAR(50), @CityId VARCHAR(50),  
  @StateId VARCHAR(50),@RTODescription VARCHAR(100),@NCBValue VARCHAR(50), @SAODInsurer VARCHAR(100),  
  @SATPInsurer VARCHAR(100), @IsRecommended bit, @RecommendedDescription NVARCHAR(MAX), @ChasisNo VARCHAR(100),
  @EngineNo VARCHAR(100), @PolicyType VARCHAR(50), @CountryCode VARCHAR(50), @RegistrationStateCode VARCHAR(50),
  @CityCode VARCHAR(50), @LicensePlateNumber varchar(50), @PreviousPolicyNumber NVARCHAR(MAX)
	  
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

  SELECT @VehicleMakeCode = VehicleManufactureCode, 
  @VehicleModelCode = VehicleModelCode,
  @ExShowRoomPrice = MaxPrice,  
  @VehicleClass = VehicleClassCode 
  FROM MOTOR.ICICI_VehicleMaster WITH(NOLOCK) WHERE VariantId = @VariantId   
  
  SELECT @PreviousPolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK)
  WHERE PreviousPolicyTypeId = @PolicyTypeId  

  if(ISNULL(@SAODInsurerId,'')<>'')	
	SELECT @SAODInsurer = InsurerName FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @SAODInsurerId  
  if(ISNULL(@SATPInsurerId,'')<>'')	
	SELECT @SATPInsurer = InsurerName FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @SATPInsurerId  
  
  SELECT @IsRecommended = IsRecommended, 
  @RecommendedDescription = RecommendedDescription 
  FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId  
	
  SELECT @RTOLocationCode = RTOLocationCode,
  @RegistrationStateCode = ILStateCode,
  @CountryCode = CountryCode ,
  @CityCode = CityDistrictCode, 
  @GSTToState = ILState,
  @LicensePlateNumber = RTOCode
  FROM MOTOR.ICICI_RTOMaster WITH(NOLOCK)
  WHERE (RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4)) AND VehicleClassCode = @VehicleClass

  
  SELECT @ManufactureDate = vehicleManufacturingMonthYear, 
  @regDate = regDate,
  @ChasisNo=chassis,
  @EngineNo=engine, 
  @PreviousPolicyNumber = vehicleInsurancePolicyNumber 
  FROM Insurance_VehicleRegistration with(nolock) 
  where vehicleNumber = @VehicleNumber --AND VariantId=@VariantId 
	  
  SELECT @VehicleType = InsuranceName  
  FROM Insurance_InsuranceType with(nolock) where InsuranceTypeId = @VehicleTypeId  
  
  --SET @NCBValue = (SELECT NCBValue FROM Insurance_NCB WHERE NCBId = @NCBId)  

  IF(ISNULL( @PolicyTypeId, '' ) = '' )    
  BEGIN    
   SELECT @PolicyTypeId = PreviousPolicyTypeId FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D' --COMPREHENSIVE    
  END   

  SELECT @PolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = @PolicyTypeId


  IF(ISNULL( @NCBId, '' ) = '')
			BEGIN
			SET @NCBValue = 0
			END
			ELSE
			BEGIN
			SET @NCBValue = (SELECT NCBValue FROM Insurance_NCB WITH(NOLOCK) WHERE NCBId = @NCBId)    
			END
  
  SELECT @VehicleMakeCode AS VehicleMakeCode, 
  @VehicleModelCode AS VehicleModelCode, 
  @RTOLocationCode AS RTOLocationCode,      
  @ExShowRoomPrice AS ExShowRoomPrice, 
  @ManufactureDate AS ManufactureDate,
  @GSTToState AS GSTToState,
  @regDate AS regDate,     
  @PreviousPolicyType AS PreviousPolicyType, 
  @Vehicleclass as vehicleclass,
  @VehicleType as VehicleType,
  @NCBValue AS NCBValue,
  @SAODInsurer AS SAODInsurer, 
  @SATPInsurer AS SATPInsurer,
  @IsRecommended AS IsRecommended, 
  @RecommendedDescription AS RecommendedDescription,	
  @ChasisNo AS chassis,
  @EngineNo AS engine,
  @PolicyType AS CurrentPolicyType,
  @LicensePlateNumber AS LicensePlateNumber,
  @PreviousPolicyNumber AS PreviousInsurancePolicyNumber
		
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
