-- =============================================  
-- Author:  <Firoz S>  
-- Create date: <02-08-2023>  
-- Description: <Insurance_GetTATAQuoteMasterMapping>  
-- =============================================  
CREATE PROCEDURE [dbo].[Insurance_GetTATAQuoteMasterMapping]    
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
@VehicleNumber VARCHAR(50) = NULL,
@IsBrandNew bit = NULL,
@RegistrationYear VARCHAR(50) = NULL,
@LeadId VARCHAR(50) = NULL,
@IDV INT = NULL,
@RegistrationDate DATE = NULL
AS    
BEGIN    
 BEGIN TRY    
	
	  DECLARE @RTOLocationCode varchar(50), @RTOLocationName varchar(50),@NCBValue varchar(50),@VehicleMake varchar(50),
	  @VehicleMakeCode varchar(50), @VehicleModel varchar(50), @VehicleModelCode varchar(50),@VehicleVariant varchar(50), 
	  @VehicleVariantCode varchar(50),@YearId VARCHAR(50),@IDVValue INT,@MinIdv INT,@MaxIdv INT,@RecommendedIdv INT, 
	  @chassis varchar(50), @engine varchar(50), @Customer_State_CD varchar(5), @vehicleSeatCapacity varchar(10),  
	  @fuelType varchar(20), @RTOCode varchar(10), @CurrentPolicyType varchar(50),@CurrentPolicyTypeId nvarchar(50),
	  @PreviousPolicyType varchar(50), @PreviousPolicyTypeId varchar(50), @BusinessType varchar(50), @BusinessTypeId varchar(10),
	  @IsRecommended VARCHAR(10), @RecommendedDescription NVARCHAR(MAX), @ExtensionName varchar(500), 
	  @PolicyType VARCHAR(50), @PreviousPolicyNumber varchar(50),@PreviousInsurerCode varchar(50), @PackageName VARCHAR(50), @PackageFlag VARCHAR(50), @PackageValidityInDays INT = 0, @VehicleSegment VARCHAR(50), @VehicleAgeInDays int, @PackagePriority int
	  
	  SET NOCOUNT ON;    
		
		IF(ISNULL( @PolicyTypeId, '' ) = '' )  
		BEGIN  
			SELECT @PolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D' --COMPREHENSIVE BUNDLE(NB)  
		END

		IF(ISNULL( @NCBId, '' ) = '')
		BEGIN
			SET @NCBValue = 0
		END
		ELSE
		BEGIN
			SET @NCBValue = (SELECT NCBValue FROM Insurance_NCB WITH(NOLOCK) WHERE NCBId = @NCBId)    
		END

		IF(@RegistrationYear = YEAR(GETDATE()))
		BEGIN
			SELECT @YearId = '0'
		END
		ELSE
		BEGIN
			SELECT @YearId = YearId FROM Insurance_Year WITH(NOLOCK) WHERE Year = @RegistrationYear
		END
	
		SELECT PACoverId, PACoverCode AS CoverName 
		FROM Insurance_PACover MP WITH(NOLOCK)
		WHERE MP.PACoverId IN (SELECT VALUE FROM string_split(@PACoverId,','))
	
		SELECT AccessoryId, AccessoryCode AS Accessory 
		FROM Insurance_Accessory MP  WITH(NOLOCK)   
		WHERE MP.AccessoryId IN (SELECT VALUE FROM string_split(@AccessoryId,','))
  
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

		SELECT @RTOLocationCode = BRCM.Code, @RTOLocationName=BRCM.Name, @RTOCode=rto.RTOCode  
		FROM Insurance_RTO rto WITH(NOLOCK)   
		LEFT OUTER JOIN MOTOR.TATA_RTOMaster BRCM WITH(NOLOCK) ON BRCM.RTOId=rto.RTOId  
		WHERE (rto.RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4))  


		IF(@VehicleTypeId='6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056')  
		BEGIN  
			SELECT @VehicleMake = TXT_MANUFACTURERNAME,@VehicleMakeCode = NUM_MANUFACTURE_CD, @VehicleModel = TXT_MODEL,
			@VehicleModelCode=NUM_MODEL_CODE, @VehicleVariant = TXT_MODEL_VARIANT,@VehicleVariantCode = NUM_MODEL_VARIANT_CODE,
			@vehicleSeatCapacity = (NUM_SEATING_CAPACITY),@fuelType=TXT_FUEL_TYPE
			FROM [MOTOR].[TATA_VehicleMaster] WITH(NOLOCK) WHERE VarientId = @VariantId  AND NUM_PRODUCT_CODE = '3187'

			IF (@IsBrandNew = 1)
			BEGIN
				SELECT @CurrentPolicyType='Package (1 year OD + 5 years TP)'  
				SELECT @CurrentPolicyTypeId = '04'
				SELECT @BusinessType = 'New Business'
				SELECT @BusinessTypeId = '01'
			END
			ELSE
			BEGIN
				IF (@PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B')  
				BEGIN  
					SELECT @CurrentPolicyType='Standalone TP (1 year)' 
					SELECT @CurrentPolicyTypeId = '01'
					SELECT @BusinessType = 'Roll Over'
					SELECT @BusinessTypeId = '03'
				END  
				ELSE IF (@PolicyTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896')  
				BEGIN  
					SELECT @CurrentPolicyType = 'Standalone OD (1 year)'  
					SELECT @CurrentPolicyTypeId = '05'
					SELECT @BusinessType = 'Roll Over'
					SELECT @BusinessTypeId = '03'
				END  
				ELSE  
				BEGIN  
					SELECT @CurrentPolicyType = 'Package (1 year OD + 1 year TP)'  
					SELECT @CurrentPolicyTypeId = '02'
					SELECT @BusinessType = 'Roll Over'
					SELECT @BusinessTypeId = '03'
				END  
			END
		END  
		ELSE IF(@VehicleTypeId='2d566966-5525-4ed7-bd90-bb39e8418f39')  
		BEGIN  
			SELECT @VehicleMake = TXT_MANUFACTURERNAME,@VehicleMakeCode = NUM_MANUFACTURE_CD, @VehicleModel = TXT_MODEL,
			@VehicleModelCode=NUM_MODEL_CODE, @VehicleVariant = TXT_MODEL_VARIANT,@VehicleVariantCode = NUM_MODEL_VARIANT_CODE,
			@vehicleSeatCapacity = (NUM_SEATING_CAPACITY),@fuelType=TXT_FUEL_TYPE, @VehicleSegment = TXT_SEGMENT
			FROM [MOTOR].[TATA_VehicleMaster] WITH(NOLOCK) WHERE VarientId = @VariantId  AND NUM_PRODUCT_CODE = '3184'
			
			IF(@IsBrandNew = 1)
				BEGIN
				SELECT @CurrentPolicyType='PackagePolicy'  
				SELECT @CurrentPolicyTypeId = '04'
				SELECT @BusinessType = 'New Business'
				SELECT @BusinessTypeId = '01'
			END
			ELSE
			BEGIN
				IF (@PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B')  
				BEGIN  
					SELECT @CurrentPolicyType='Standalone TP' 
					SELECT @CurrentPolicyTypeId = '01'
					SELECT @BusinessType = 'Roll Over'
					SELECT @BusinessTypeId = '03'
				END  
				ELSE IF (@PolicyTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896')  
				BEGIN  
					SELECT @CurrentPolicyType = 'Standalone OD'  
					SELECT @CurrentPolicyTypeId = '05'
					SELECT @BusinessType = 'Roll Over'
					SELECT @BusinessTypeId = '03'
				END  
				ELSE  
				BEGIN  
					SELECT @CurrentPolicyType = 'PackagePolicy'  
					SELECT @CurrentPolicyTypeId = '02'
					SELECT @BusinessType = 'Roll Over'
					SELECT @BusinessTypeId = '03'
				END  
			END
			--Package Implementation
			SELECT TOP 1 @PackageName = PackageName, @PackageFlag = PackageFlag, @PackagePriority = [Priority]
			FROM(
					SELECT
						PackageName,
						[Priority],
						MAX(ROW1) ADDONCOUNT,
						PackageFlag
					FROM
						(
							SELECT AddOn.PackageName,AddOn.[Priority],AddOn.PackageFlag,
							ROW_NUMBER() OVER(PARTITION BY AddOn.PackageName ORDER BY AddOn.[Priority])ROW1
							FROM Tata_PackageMaster AddOn
							LEFT JOIN Tata_AddonPackageMaster Package on AddOn.PackageId=Package.Package_Id
							WHERE PolicyTypeId = (CASE WHEN @PolicyTypeId ='20541BE3-D76E-4E73-9AB1-240CCB33DA5D' THEN '517D8F9C-F532-4D45-8034-ABECE46693E3' ELSE @PolicyTypeId END) 
							AND IsBrandNew = @IsBrandNew AND Addon_Id IN (SELECT * FROM string_split(@AddonId,','))
					)AS t
					GROUP BY PackageName,[Priority],PackageFlag
			)A
			ORDER BY ADDONCOUNT DESC,Priority

			IF(@IsBrandNew = 1)
			BEGIN
				IF(ISNULL(@PackageName,'')='')
				BEGIN
					SELECT @PackageName = 'SILVER', @PackageFlag = 'P1' --No need to check for NCB condition
				END
			END
			ELSE
			BEGIN
				IF(ISNULL(@PackageName,'')='')
				BEGIN
					SELECT @PackageName = 'SILVER', @PackageFlag = 'P1'
				END
				ELSE
				BEGIN
					SELECT @PackageName = VAL.PackageName, @PackageFlag = VAL.PackageFlag, @PackageValidityInDays = VAL.PackageValidityInDays 
					FROM [MOTOR].[TATA_AddonPackageAgeValidationMaster] VAL WITH(NOLOCK)
					WHERE PackageName = @PackageName AND IsNCBZero = CASE WHEN @NCBValue = 0 THEN 1 WHEN @NCBValue > 0 THEN 0 END 
					AND IsActive = '1' -- NCB condition has to be checked
					
					-- Bundle age and vehicle age has to be checked(if age greather then need to assign next lower plan)
					SELECT @VehicleAgeInDays = DATEDIFF(DAY, @RegistrationDate, GETDATE())
					IF(@VehicleAgeInDays > @PackageValidityInDays)
					BEGIN
						SELECT TOP 1 @PackageName = VAL.PackageName, @PackageFlag = VAL.PackageFlag
						FROM [MOTOR].[TATA_AddonPackageAgeValidationMaster] VAL WITH(NOLOCK)
						INNER JOIN Tata_PackageMaster PAC WITH(NOLOCK) ON PAC.PackageName = VAL.PackageName
						WHERE  VAL.IsNCBZero = CASE WHEN @NCBValue = 0 THEN 1 WHEN @NCBValue > 0 THEN 0 END 
						AND VAL.PackageValidityInDays > @VehicleAgeInDays AND PAC.[Priority] < @PackagePriority
						AND VAL.IsActive = '1'
						ORDER BY PAC.Priority DESC
					END
					
				END
			END
			--Package Addon Response configuration for Prev Policy Details capture in quoteconfirm screen 
			SELECT @AddonId = @AddonId + ',' + Addon_Id
			FROM Tata_AddonPackageMaster TAPM 
			INNER JOIN Tata_PackageMaster TPM ON TPM.PackageId = TAPM.Package_Id
			WHERE TPM.PackageName = @PackageName AND TAPM.PolicyTypeId = (CASE WHEN @PolicyTypeId ='20541BE3-D76E-4E73-9AB1-240CCB33DA5D' THEN '517D8F9C-F532-4D45-8034-ABECE46693E3' ELSE @PolicyTypeId END) 
			AND TAPM.IsBrandNew = @IsBrandNew
		END  
		

		SELECT AddonId, AddOnCode AS AddOns 
		FROM Insurance_AddOns MP  WITH(NOLOCK)  
		WHERE AddonId IN (SELECT VALUE FROM string_split(@AddonId,','))  

		SELECT @PreviousPolicyTypeId = PrevPolicyTypeId FROM Insurance_LeadDetails WHERE LeadId=@LeadId
		IF(@IsBrandNew = '0')
		BEGIN
			IF(@PreviousPolicyTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3')
			BEGIN
				SELECT @PreviousPolicyType = 'Package'
			END
			ELSE IF(@PreviousPolicyTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896')
			BEGIN
				SELECT @PreviousPolicyType = 'Package'
			END
			ELSE
			BEGIN
				SELECT @PreviousPolicyType = 'Liability'
			END
		END
  
		SELECT  @chassis=chassis,   
		@engine=engine,   
		@VehicleNumber=vehicleNumber,
		@PreviousPolicyNumber=vehicleInsurancePolicyNumber
		FROM Insurance_VehicleRegistration with(nolock) where VariantId=@VariantId AND regNo=@VehicleNumber  
  
		
		SELECT TOP 1 @RecommendedIdv = QUOTE.RecommendedIDV, @MinIdv=QUOTE.MinIDV,@MaxIdv=QUOTE.MaxIDV 
		FROM Insurance_QuoteTransaction QUOTE WITH(NOLOCK)
		INNER JOIN Insurance_LeadDetails LEAD WITH(NOLOCK)
		ON QUOTE.LeadId = LEAD.LeadId
		WHERE QUOTE.InsurerId=@InsurerId AND QUOTE.LeadId=@LeadId AND LEAD.VariantId = @VariantId 
		AND LEAD.PolicyTypeId = (CASE WHEN @PolicyTypeId ='20541BE3-D76E-4E73-9AB1-240CCB33DA5D' THEN '517D8F9C-F532-4D45-8034-ABECE46693E3' ELSE @PolicyTypeId END)
		AND LEAD.YearId= @YearId  AND (QUOTE.RTOId = @RTOId OR LEFT(QUOTE.VehicleNumber,4) =LEFT(@VehicleNumber,4))
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

		
		
		SELECT @IsRecommended = IsRecommended, @RecommendedDescription = RecommendedDescription FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId  
		
		SELECT @PreviousInsurerCode=Code FROM MOTOR.TATA_PrevInsuranceMaster WITH(NOLOCK) WHERE InsurerId=@InsurerId

		SELECT @PolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = @PolicyTypeId

		SELECT  @RTOLocationCode as RTOLocationCode, 
		@RTOLocationName as RTOLocationName,
		@NCBValue as NCBValue,   
		@VehicleMake VehicleMake,
		@VehicleMakeCode VehicleMakeCode,
		@VehicleModel VehicleModel,
		@VehicleModelCode VehicleModelCode,  
		@VehicleVariant VehicleVariant,
		@VehicleVariantCode VehicleVariantCode,
		@chassis chassis,  
		@engine engine,  
		@vehicleSeatCapacity vehicleSeatCapacity,  
		@fuelType Fuel,
		@VehicleSegment VehicleSegment,
		@RTOCode RTOCode,  
		@CurrentPolicyType CurrentPolicyType,
		@CurrentPolicyTypeId CurrentPolicyTypeId,
		@BusinessType BusinessType,
		@BusinessTypeId BusinessTypeId,
		@PreviousPolicyType PreviousPolicyType,   
		@IsRecommended IsRecommended, 
		@RecommendedDescription RecommendedDescription,
		@PolicyType OriginalPreviousPolicyType,
		@PreviousPolicyNumber PreviousInsurancePolicyNumber,
		@PreviousInsurerCode PreviousInsurerCode,
		@IDVValue IDVValue,
		@MinIdv MinIdv,
		@MaxIdv MaxIdv,
		@RecommendedIdv RecommendedIdv 


		SELECT ConfigName, ConfigValue FROM Insurance_ICConfig WITH(NOLOCK) WHERE InsurerId=@InsurerId AND PolicyTypeId=@PolicyTypeId AND VehicleTypeId=@VehicleTypeId  
		
		SELECT @PackageName AS PackageName, @PackageFlag AS PackageFlag, @PackageValidityInDays AS PackageValidityInDays
 END TRY                    
 BEGIN CATCH                        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
END