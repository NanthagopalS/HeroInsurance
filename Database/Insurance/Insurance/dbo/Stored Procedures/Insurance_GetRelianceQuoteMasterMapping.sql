-- =============================================  
-- Author:  <Firoz S>  
-- Create date: <12-07-2023>  
-- Description: [Insurance_GetRelianceQuoteMasterMapping]
-- =============================================  
CREATE   PROCEDURE [dbo].[Insurance_GetRelianceQuoteMasterMapping]    
@PACoverId VARCHAR(2000)= NULL,    
@AccessoryId VARCHAR(2000)= NULL,    
@AddonId VARCHAR(2000)= NULL,    
@DiscountId VARCHAR(2000)= NULL,  
@InsurerId VARCHAR(50)= NULL,    
@RTOId varchar(50)= NULL,    
@VariantId varchar(50)= NULL,    
@NCBId varchar(50)= NULL,  
@PolicyTypeId varchar(50)= NULL,  
@VehicleTypeId varchar(50)= NULL,  
@PACoverExtensionId VARCHAR(2000)= NULL,  
@DiscountExtensionId VARCHAR(2000)= NULL,  
@AddOnsExtensionId VARCHAR(2000)= NULL,
@VehicleNumber VARCHAR(50)= NULL,
@LeadId VARCHAR(50) = NULL,
@IDV INT = NULL,
@PreviousInsurer VARCHAR(50) = NULL,
@RegistrationYear VARCHAR(50) = NULL
AS    
BEGIN    
 BEGIN TRY    
    
			DECLARE @RTOCode varchar(50), @PreviousInsurerCode varchar(50), @VehicleCode varchar(50), @NCBValue varchar(50),   
			@CityName varchar(50)  ,@VehicleType varchar(50), @VehicleMakeCode varchar(50), @VehicleMake varchar(50),  
			@VehicleModelCode varchar(50), @VehicleModel varchar(50), @VehicleSubTypeCode varchar(50), @VehicleSubType varchar(50),  
			@Fuel varchar(50), @FuelId VARCHAR(10), @vehicleId varchar(50), @regNo varchar(50), @class varchar(50), @chassis varchar(50), @engine varchar(50),  
			@vehicleColour varchar(50), @regDate varchar(50), @vehicleCubicCapacity varchar(50), @vehicleSeatCapacity varchar(50),  
			@Zone Varchar(5),@ManufacturingYear VARCHAR(10), @IsRecommended VARCHAR(10), @RecommendedDescription NVARCHAR(MAX), @PolicyType VARCHAR(50), @PreviousPolicyNumber varchar(50),
			@IDVValue INT,@MinIdv INT,@MaxIdv INT,@RecommendedIdv INT, @YearId VARCHAR(50), @RTORegionCode VARCHAR(50), @VehicleVariant VARCHAR(50), @State_Id VARCHAR(50), @NoOfWheels VARCHAR(10), @NCBValueId VARCHAR(10)


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
			FROM Insurance_PACoverExtension EXT WITH(NOLOCK)   
			INNER JOIN Insurance_PACover PA WITH(NOLOCK) ON PA.PACoverId = EXT.PACoverId   
			WHERE EXT.IsActive = 1 AND PA.PACoverId IN (SELECT VALUE FROM string_split(@PACoverId,','))   
			AND EXT.PACoverExtensionId IN (SELECT VALUE FROM string_split(@PACoverExtensionId,','))  
  
			SELECT EXT.DiscountExtensionId, EXT.DiscountExtension, EXT.DiscountId  
			FROM Insurance_DiscountExtension EXT WITH(NOLOCK)   
			INNER JOIN Insurance_Discounts PA WITH(NOLOCK) ON PA.DiscountId = EXT.DiscountId   
			WHERE EXT.IsActive = 1 AND PA.DiscountId IN (SELECT VALUE FROM string_split(@DiscountId,','))   
			AND EXT.DiscountExtensionId IN (SELECT VALUE FROM string_split(@DiscountExtensionId,','))  

			SELECT EXT.AddOnsExtensionId, EXT.AddOnsExtension, EXT.AddOnsId
			FROM Insurance_AddOnsExtension EXT WITH(NOLOCK) 
			INNER JOIN Insurance_AddOns PA WITH(NOLOCK)  ON PA.AddOnId = EXT.AddOnsId 
			WHERE EXT.IsActive = 1 AND PA.AddOnId IN (SELECT VALUE FROM string_split(@AddonId,',')) 
			AND EXT.AddOnsExtensionId IN (SELECT VALUE FROM string_split(@AddOnsExtensionId,','))
 
			SELECT @IsRecommended = IsRecommended, @RecommendedDescription = RecommendedDescription FROM Insurance_Insurer WHERE InsurerId = @InsurerId  -- Need to check
  

			SELECT @RTOCode = rto.RTOCode,@RTORegionCode = BRCM.ModelRegionID, @CityName=ct.CityName,@Zone=BRCM.ModelZoneName,
			@State_Id = BRCM.StateID
			FROM Insurance_RTO rto WITH(NOLOCK)   
			LEFT OUTER JOIN Insurance_City ct WITH(NOLOCK) ON ct.CityId=rto.CityId  
			LEFT OUTER JOIN MOTOR.Reliance_RTOMaster BRCM WITH(NOLOCK) on BRCM.RTOId=rto.RTOId  
			WHERE (rto.RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4))  
  
			SET @PreviousInsurerCode = (SELECT InsuranceCompanyID as InsurerCode 
			FROM MOTOR.Reliance_PreviousInsurerMaster WITH(NOLOCK) WHERE InsurerId = @PreviousInsurer)    
     
			SELECT 
			@VehicleCode=VA.VehicleCode,   
			@VehicleType=VM.VehTypeName,   
			@VehicleMakeCode=VM.MakeID,   
			@VehicleMake=VM.MakeName,  
			@VehicleModelCode=VM.ModelID,   
			@VehicleModel=ModelName,   
			@Fuel=VM.OperatedBy,  
			@vehicleSeatCapacity=VM.SeatingCapacity, 
			@vehicleCubicCapacity=VA.CubicCapacity,
			@VehicleVariant = VM.Variance,
			@NoOfWheels = VM.Wheels
			FROM [MOTOR].[Reliance_VehicleMaster] VM WITH(NOLOCK) 
			LEFT JOIN Insurance_Variant VA WITH(NOLOCK)  ON VA.VariantId = @VariantId
			WHERE VarientId = @VariantId  
			
			IF(@Fuel = 'PETROL')
			BEGIN
				SET @FuelId = '1'
			END
			ELSE IF(@Fuel = 'DIESEL')
			BEGIN
				SET @FuelId = '2'
			END
			ELSE IF(@Fuel = 'CNG')
			BEGIN
				SET @FuelId = '3'
			END
			ELSE IF(@Fuel = 'LPG')
			BEGIN
				SET @FuelId = '4'
			END
			ELSE IF(@Fuel = 'BATTERY OPERATED')
			BEGIN
				SET @FuelId = '6'
			END
			ELSE
			BEGIN
				SET @FuelId = '5' --(PETROL+LPG)/(PETROL+CNG)
			END

			SELECT @vehicleId=vehicleId,   
			@regNo=regNo,   
			@class=class,   
			@chassis=chassis,   
			@engine=engine,   
			@vehicleColour=vehicleColour,  
			@regDate=regDate,   
			@VehicleNumber=vehicleNumber,   
			@ManufacturingYear=RIGHT(vehicleManufacturingMonthYear,4),
			@PreviousPolicyNumber=vehicleInsurancePolicyNumber
			FROM Insurance_VehicleRegistration with(nolock) 
			where VariantId=@VariantId AND regNo=@VehicleNumber  
  
			IF(ISNULL( @ManufacturingYear, '' ) = '' )  
			BEGIN  
			SELECT @ManufacturingYear = RIGHT(@regDate,4)  
			END  
			
			IF(ISNULL( @NCBId, '' ) = '')
			BEGIN
			SET @NCBValue = 0
			END
			ELSE
			BEGIN
			SET @NCBValue = (SELECT NCBValue FROM Insurance_NCB WITH(NOLOCK) WHERE NCBId = @NCBId)    
			END
			
			SELECT @NCBValueId = CASE WHEN @NCBValue= 0 THEN 0 WHEN @NCBValue = 20 THEN 1 WHEN @NCBValue = 25 THEN 2 
			WHEN @NCBValue = 35 THEN 3 WHEN @NCBValue = 45 THEN 4 WHEN @NCBValue = 50 THEN 5 END;
    
			SELECT @PolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = @PolicyTypeId
			
			IF(@RegistrationYear = YEAR(GETDATE()))
			BEGIN
				SELECT @YearId = '0'
			END
			ELSE
			BEGIN
				SELECT @YearId = YearId FROM Insurance_Year WITH(NOLOCK) WHERE YearId = @YearId
			END

			SELECT TOP 1 @RecommendedIdv = QUOTE.RecommendedIDV, @MinIdv=QUOTE.MinIDV,@MaxIdv=QUOTE.MaxIDV 
			FROM Insurance_QuoteTransaction QUOTE WITH(NOLOCK)
			INNER JOIN Insurance_LeadDetails LEAD WITH(NOLOCK)
			ON QUOTE.LeadId = LEAD.LeadId
			WHERE QUOTE.InsurerId=@InsurerId AND QUOTE.LeadId=@LeadId AND LEAD.VariantId = @VariantId 
			AND LEAD.PolicyTypeId = (CASE WHEN @PolicyTypeId ='20541BE3-D76E-4E73-9AB1-240CCB33DA5D' THEN '517D8F9C-F532-4D45-8034-ABECE46693E3' ELSE @PolicyTypeId END)
			AND LEAD.YearId= @YearId
			ORDER BY QUOTE.CreatedOn DESC

			IF(@IDV =1)
			BEGIN
				SET @IDVValue=@RecommendedIdv
			END
			ELSE IF(@IDV=2)
			BEGIN
				SET @IDVValue=@MinIdv
			END
			ELSE IF(@IDV=3)
			BEGIN
				SET @IDVValue=@MaxIdv
			END
			ELSE
			BEGIN
				SET @IDVValue=@IDV
			END
			

			SELECT @RTOCode as RTOCode,   
			@RTORegionCode as RTOLocationCode,
			@State_Id as State_Id,
			@PreviousInsurerCode as PreviousInsurerCode,  
			@VehicleCode as VehicleCode,  
			@NCBValue as NCBValue,
			@NCBValueId as NCBValueId,
			@CityName CityName,   
			TRIM(@Zone) AS Zone,  
			@VehicleType VehicleType,   
			@VehicleMakeCode VehicleMakeCode,   
			@VehicleMake VehicleMake,   
			@VehicleModelCode VehicleModelCode,   
			@VehicleModel VehicleModel,   
			@VehicleSubTypeCode VehicleSubTypeCode,  
			@VehicleSubType VehicleSubType,   
			@VehicleVariant VehicleVariant,
			@Fuel Fuel,  
			@FuelId FuelId,
			@vehicleId vehicleId,   
			@regNo regNo,   
			@class vehicleclass,   
			@chassis chassis,  
			@engine engine,   
			@vehicleColour vehicleColour,   
			REPLACE(Convert(varchar, Convert(date,@regDate,103),106),' ','-') regDate, --19/04/2021  
			@vehicleCubicCapacity vehicleCubicCapacity,   
			@vehicleSeatCapacity vehicleSeatCapacity,   
			@NoOfWheels NoOfWheels,
			@RTOCode vehicleNumber,  
			@ManufacturingYear ManufactureDate,  
			@IsRecommended IsRecommended, 
			@RecommendedDescription RecommendedDescription,
			@PolicyType CurrentPolicyType,
			@PreviousPolicyNumber PreviousInsurancePolicyNumber,
			@IDVValue IDVValue,
			@MinIdv MinIdv,
			@MaxIdv MaxIdv,
			@RecommendedIdv RecommendedIdv 

  
			SELECT ConfigName, ConfigValue 
			FROM Insurance_ICConfig WITH(NOLOCK) 
			WHERE InsurerId=@InsurerId AND PolicyTypeId=@PolicyTypeId AND VehicleTypeId=@VehicleTypeId  
			
 END TRY                    
 BEGIN CATCH              
           
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
END