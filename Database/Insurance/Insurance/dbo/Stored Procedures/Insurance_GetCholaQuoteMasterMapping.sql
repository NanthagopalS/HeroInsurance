CREATE PROCEDURE [dbo].[Insurance_GetCholaQuoteMasterMapping]  
@PACoverId VARCHAR(2000),        
@AccessoryId VARCHAR(2000),        
@AddonId VARCHAR(2000),       
@DiscountId VARCHAR(2000),      
@InsurerId VARCHAR(50),        
@RTOId VARCHAR(50),        
@VariantId VARCHAR(50),        
@NCBId VARCHAR(50),      
@PolicyTypeId VARCHAR(50),      
@VehicleTypeId VARCHAR(50),  
@VehicleNumber VARCHAR(50),
@PACoverExtensionId NVARCHAR(MAX),
@DiscountExtensionId NVARCHAR(MAX),
@AddOnsExtensionId NVARCHAR(MAX)
AS        
BEGIN        
 BEGIN TRY        
		
  DECLARE @VehicleMake VARCHAR(50), @VehicleModel VARCHAR(50), @VehicleModelCode VARCHAR(50),   
  @Fuel VARCHAR(20), @VehicleCubicCapacity VARCHAR(10), @ExShowRoomPrice VARCHAR(20),   
  @RTOLocationCode VARCHAR(10), @RegistrationStateCode VARCHAR(20), @RegistrationRTOCode VARCHAR(20) ,@RTOCityName VARCHAR(50),@RTOStateName VARCHAR(50), @VehicleClass VARCHAR(20)  
  , @IsRecommended VARCHAR(10), @RecommendedDescription NVARCHAR(MAX) , @VehicleSeatCapacity VARCHAR(10) ,
   @PolicyType VARCHAR(100), @NCBValue VARCHAR(10),@RTOCode varchar(50)
	  
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
  
  IF(ISNULL(@RTOId, '' ) = '') 
	BEGIN
		SELECT DISTINCT @RTOId = RTOId FROM Insurance_RTO WITH(NOLOCK)   
		WHERE RTOCode = LEFT(@VehicleNumber,4) 
	END

  IF(@PolicyTypeId='2AA7FDCA-9E36-4A8D-9583-15ADA737574B')
  BEGIN
	SELECT TOP 1 @VehicleMake = Make, @VehicleModel = VehicleModel, @VehicleModelCode = ModelCode,  
	@Fuel = FuelType, @VehicleCubicCapacity = CubicCapacity, @ExShowRoomPrice = ExShowRoom,   
	@VehicleClass = VehicleClass, @VehicleSeatCapacity = SeatingCapacity  
	FROM MOTOR.Chola_VehicleMaster VM WITH(NOLOCK) 
	INNER JOIN MOTOR.Chola_RTOMaster RTO WITH(NOLOCK) 
	ON VM.NumStateCode = RTO.NumStateCode
	WHERE VM.VarientId = @VariantId AND RTO.RTOId= @RTOId
	AND VM.PolicyTypeId ='2AA7FDCA-9E36-4A8D-9583-15ADA737574B'  
  END
  ELSE
  BEGIN
	SELECT TOP 1 @VehicleMake = Make, @VehicleModel = VehicleModel, @VehicleModelCode = ModelCode,  
	@Fuel = FuelType, @VehicleCubicCapacity = CubicCapacity, @ExShowRoomPrice = ExShowRoom,   
	@VehicleClass = VehicleClass, @VehicleSeatCapacity = SeatingCapacity  
	FROM MOTOR.Chola_VehicleMaster VM WITH(NOLOCK) 
	INNER JOIN MOTOR.Chola_RTOMaster RTO WITH(NOLOCK) 
	ON VM.NumStateCode = RTO.NumStateCode
	WHERE VM.VarientId = @VariantId AND RTO.RTOId= @RTOId
	AND VM.PolicyTypeId IS NULL OR VM.PolicyTypeId <> '2AA7FDCA-9E36-4A8D-9583-15ADA737574B' 
  END
  
   
SET @RTOId = (SELECT DISTINCT RTOId FROM Insurance_RTO WITH(NOLOCK) WHERE (RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4)))

  SELECT TOP 1 @RTOLocationCode = TXTRTOLocationCode, 
  @RegistrationStateCode = TXTRegistrationStateCode,  
  @RegistrationRTOCode = NumRegistrationRTOCode, 
  @RTOCityName = DistrictName, 
  @RTOStateName = StateName,
  @RTOCode = RTOCode--TXTRegistrationStateCode+NumRegistrationRTOCode
  FROM MOTOR.Chola_RTOMaster WITH(NOLOCK)
  WHERE RTOId = @RTOId AND NumVehicleClassCode = @VehicleClass  
  
	IF(ISNULL( @PolicyTypeId, '' ) = '' )  
  BEGIN  
   SELECT @PolicyTypeId = PreviousPolicyTypeId FROM Insurance_PreviousPolicyType WITH(NOLOCK)   
   WHERE PreviousPolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D' --COMPREHENSIVE  
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

	SELECT @IsRecommended = IsRecommended, @RecommendedDescription = RecommendedDescription FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId  

  SELECT @VehicleMake AS VehicleMake,   
  @VehicleModel AS VehicleModel,   
  @VehicleModelCode AS VehicleModelCode,  
  @Fuel AS Fuel,   
  @VehicleCubicCapacity AS VehicleCubicCapacity,   
  @ExShowRoomPrice AS ExShowRoomPrice,  
  @RTOLocationCode AS RTOLocationCode,   
  @RegistrationStateCode AS RegistrationStateCode,  
  @RegistrationRTOCode AS RegistrationRTOCode,   
  @RTOCityName AS RTOCityName,  
  @RTOStateName AS RTOStateName,  
  @RTOCode AS RTOCode,
  @VehicleClass AS VehicleClass,  
  @IsRecommended IsRecommended, 
  @RecommendedDescription RecommendedDescription,
  @VehicleSeatCapacity VehicleSeatCapacity, 
  @PolicyType CurrentPolicyType,
  @NCBValue NCBValue 
	
  SELECT ConfigName, ConfigValue FROM Insurance_ICConfig WITH(NOLOCK) WHERE InsurerId=@InsurerId AND PolicyTypeId=@PolicyTypeId AND VehicleTypeId=@VehicleTypeId  

  SELECT EXT.PACoverExtensionId, EXT.PACoverExtension, EXT.PACoverId  
  FROM Insurance_PACoverExtension as EXT WITH(NOLOCK)   
  INNER JOIN Insurance_PACover as PA WITH(NOLOCK) ON PA.PACoverId = EXT.PACoverId   
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
  
 END TRY                  
 BEGIN CATCH            
		 
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList VARCHAR(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  