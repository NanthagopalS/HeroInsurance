-- =============================================  
-- Author:  <Firoz S>  
-- Create date: <01-12-2022>  
-- Description: <Insurance_GetBajajQuoteMasterMapping>  
-- =============================================  
CREATE PROCEDURE [dbo].[Insurance_GetBajajQuoteMasterMapping]    
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
			@Fuel varchar(50), @vehicleId varchar(50), @regNo varchar(50), @class varchar(50), @chassis varchar(50), @engine varchar(50),  
			@vehicleColour varchar(50), @regDate varchar(50), @vehicleCubicCapacity varchar(50), @vehicleSeatCapacity varchar(50),  
			@Zone Varchar(5),@ManufacturingYear VARCHAR(10), @IsRecommended VARCHAR(10), @RecommendedDescription NVARCHAR(MAX),
			@PackageName varchar(50), @PolicyType VARCHAR(50), @PreviousPolicyNumber varchar(50),
			@IDVValue INT,@MinIdv INT,@MaxIdv INT,@RecommendedIdv INT, @YearId VARCHAR(50)


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

			-- Pakcage Implementation
			SELECT TOP 1  @PackageName =PackageFlag FROM (
			SELECT ROW_NUMBER() OVER (PARTITION BY PackageFlag ORDER BY PackageFlag)ROW1,* 
			FROM Insurance_AddOnsPackage ADDON WITH(NOLOCK)
			WHERE ADDON.InsurerId = @InsurerId AND ADDON.AddOnId IN (SELECT * FROM string_split(@AddonId,','))
			)A 
			ORDER BY ROW1 DESC,PackageFlag

			--Package Addon Response configuration for Prev Policy Details capture in quoteconfirm screen
			IF(ISNULL(@PackageName,'')='')
			BEGIN
				SELECT @AddonId = @AddonId + ',' + AddOnId
				FROM Insurance_AddOnsPackage 
				WHERE VehicleTypeId = @VehicleTypeId AND PolicyTypeId = (CASE WHEN @PolicyTypeId ='20541BE3-D76E-4E73-9AB1-240CCB33DA5D' THEN '517D8F9C-F532-4D45-8034-ABECE46693E3' ELSE @PolicyTypeId END) 
				AND PackageName ='Drive Assure Prime'
			END
			ELSE
			BEGIN
				SELECT @AddonId = @AddonId + ',' + AddOnId
				FROM Insurance_AddOnsPackage 
				WHERE PackageFlag = @PackageName AND PolicyTypeId = (CASE WHEN @PolicyTypeId ='20541BE3-D76E-4E73-9AB1-240CCB33DA5D' THEN '517D8F9C-F532-4D45-8034-ABECE46693E3' ELSE @PolicyTypeId END) 
			END
			
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
 
			SELECT @IsRecommended = IsRecommended, @RecommendedDescription = RecommendedDescription FROM Insurance_Insurer WHERE InsurerId = @InsurerId  
  

			SELECT @RTOCode = rto.RTOCode, @CityName=ct.CityName,@Zone=BRCM.Zone 
			FROM Insurance_RTO rto WITH(NOLOCK)   
			LEFT OUTER JOIN Insurance_City ct WITH(NOLOCK) ON ct.CityId=rto.CityId  
			LEFT OUTER JOIN MOTOR.Bajaj_RTOCityMaster BRCM WITH(NOLOCK) on BRCM.RTOId=rto.RTOId  
			WHERE (rto.RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4))  
  
			SET @PreviousInsurerCode = (SELECT CompanyCode as InsurerCode FROM MOTOR.Bajaj_PrevInsuranceCompanyCode WITH(NOLOCK) WHERE PreviousInsurerId = @PreviousInsurer)    
     
			SELECT @VehicleCode=VehicleCode,   
			@VehicleType=VehicleType,   
			@VehicleMakeCode=VehicleMakeCode,   
			@VehicleMake=VehicleMake,  
			@VehicleModelCode=VehicleModelCode,   
			@VehicleModel=VehicleModel,   
			@VehicleSubTypeCode=VehicleSubTypeCode,  
			@VehicleSubType=VehicleSubType,   
			@Fuel=Fuel,  
			@vehicleSeatCapacity=CarryingCapacity,  
			@vehicleCubicCapacity=CubicCapacity  
			FROM [MOTOR].[Bajaj_VehicleMaster] WITH(NOLOCK) 
			WHERE Variantid = @VariantId  
  
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
			
    
			SELECT @PolicyType = PreviousPolicyType FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = @PolicyTypeId
			
			IF(@RegistrationYear = YEAR(GETDATE()))
			BEGIN
				SELECT @YearId = '0'
			END
			ELSE
			BEGIN
				SELECT @YearId = YearId FROM Insurance_Year WITH(NOLOCK) WHERE Year = @RegistrationYear
			END

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
			

			SELECT @RTOCode as RTOCode,   
			@PreviousInsurerCode as PreviousInsurerCode,  
			@VehicleCode as VehicleCode,  
			@NCBValue as NCBValue,   
			@CityName CityName,   
			TRIM(@Zone) AS Zone,  
			@VehicleType VehicleType,   
			@VehicleMakeCode VehicleMakeCode,   
			@VehicleMake VehicleMake,   
			@VehicleModelCode VehicleModelCode,   
			@VehicleModel VehicleModel,   
			@VehicleSubTypeCode VehicleSubTypeCode,  
			@VehicleSubType VehicleSubType,   
			@Fuel Fuel,   
			@vehicleId vehicleId,   
			@regNo regNo,   
			@class vehicleclass,   
			@chassis chassis,  
			@engine engine,   
			@vehicleColour vehicleColour,   
			REPLACE(Convert(varchar, Convert(date,@regDate,103),106),' ','-') regDate, --19/04/2021  
			@vehicleCubicCapacity vehicleCubicCapacity,   
			@vehicleSeatCapacity vehicleSeatCapacity,   
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
			
			
			SELECT @PackageName AS Package_Name


 END TRY                    
 BEGIN CATCH              
           
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
END 