/*
EXEC [Insurance_GetQuoteMasterMapping] '8FB2819E-E63D-4A72-9D21-78E579F95C9E ,CED94D1E-88B3-4A64-AF21-72F50C276827 ,9B79EB34-3FD8-4B47-9EFE-A70D21E2D933 ,26894628-03A6-4156-932B-097A86A210C9 ,46C98A32-4C0D-492C-BA20-A4B9A243F4FA ,6A584A17-2420-46AD-8A31-70377DEE78EC',
'2B631A39-7B53-4249-9F78-257952A81060 ,6FEA6FC4-7418-4F01-9061-5173233BB019 ,25C59740-45D0-4D08-AD86-4F3F1D50C801 ',
'','78190CB2-B325-4764-9BD9-5B9806E99621',
'E090B03F-B1A7-4F8E-8C87-C105A046094A',
'06028417-A123-4535-A701-7E5832DCDFF2',
'6BEFB5F4-1D2C-4BAD-87A5-20A45079ADFC',
'517D8F9C-F532-4D45-8034-ABECE46693E3',
'2d566966-5525-4ed7-bd90-bb39e8418f39',
'',
'794C38B8-16AF-4CD4-A57A-D515180DD371',
' , , , , ,B31B05E3-71E1-4F7C-909F-76A6DF8D0C9D,91CE0189-3823-415C-BDBD-060D92C9C91F ',
'3AB0125E-5FAA-4628-B28E-3300DE8A31FC '
*/
CREATE PROCEDURE [dbo].[Insurance_GetQuoteMasterMapping]
@PACoverId VARCHAR(2000) = NULL,
@AccessoryId VARCHAR(2000)= NULL,
@AddonId VARCHAR(2000)= NULL,
@InsurerId VARCHAR(50)= NULL,
@RTOId VARCHAR(50)= NULL,
@VariantId VARCHAR(50)= NULL,
@NCBId VARCHAR(50)= NULL,
@PolicyTypeId VARCHAR(50)= NULL,
@VehicleTypeId VARCHAR(50)= NULL,
@VehicleNumber VARCHAR(20)= NULL,
@DiscountId VARCHAR(2000)= NULL,
@PACoverExtensionId VARCHAR(2000)= NULL,
@DiscountExtensionId VARCHAR(2000)= NULL,
@AddOnsExtensionId VARCHAR(2000)= NULL,
@LeadId VARCHAR(50) = NULL,
@IDV INT = NULL,
@IsBrandNew bit = NULL,
@PrevSAODInsurer VARCHAR(50) = NULL,
@PrevTPInsurer VARCHAR(50) = NULL,
@RegistrationYear VARCHAR(50) = NULL
AS
BEGIN
	BEGIN TRY

		DECLARE @RTOCode VARCHAR(50), @PreviousInsurerCode VARCHAR(50), @VehicleCode VARCHAR(50), @NCBVaule VARCHAR(50),
		@PreviousInsurancePolicyNumber VARCHAR(100),@ChasisNo VARCHAR(100),@EngineNo VARCHAR(100),
		@OriginalPreviousPolicyType VARCHAR(20),
		@PlanType VARCHAR(100),@VehicleSeatCapacity VARCHAR(10), @PolicyType VARCHAR(50),
		@IDVValue INT,@MinIdv INT,@MaxIdv INT,@RecommendedIdv INT, @YearId VARCHAR(50)
		
		SET NOCOUNT ON;
		IF(ISNULL( @PolicyTypeId, '' ) = '' )
		BEGIN
			SELECT @PolicyTypeId = PreviousPolicyTypeId FROM Insurance_PreviousPolicyType WITH(NOLOCK) 
			WHERE PreviousPolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D' --COMPREHENSIVE BUNDLE
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
		
		SET @RTOCode = (SELECT DISTINCT RTOCode FROM Insurance_RTO WITH(NOLOCK) WHERE (RTOId = @RTOId OR RTOCode =LEFT(@VehicleNumber,4)))

		IF(ISNULL( @PrevSAODInsurer, '' ) = '')
		BEGIN
			SET @PreviousInsurerCode = (SELECT InsurerCode FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @PrevTPInsurer)
		END
		ELSE
		BEGIN
			SET @PreviousInsurerCode = (SELECT InsurerCode FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @PrevSAODInsurer)
		END
		
		SET @VehicleCode = (SELECT VehicleCode FROM Insurance_Variant WITH(NOLOCK) WHERE VariantId = @VariantId)
		
		
		IF(ISNULL( @NCBId, '' ) = '')
			BEGIN
			SET @NCBVaule = 'ZERO'
			END
			ELSE
			BEGIN
			SET @NCBVaule = (SELECT NCBName FROM Insurance_NCB WITH(NOLOCK) WHERE NCBId = @NCBId)    
			END
  

		SELECT @PreviousInsurancePolicyNumber =vehicleInsurancePolicyNumber,@ChasisNo=chassis,@EngineNo=engine FROM Insurance_VehicleRegistration WITH(NOLOCK) WHERE regNo = @VehicleNumber

		SELECT @VehicleSeatCapacity = (SeatingCapacity - 1) FROM MOTOR.GoDigit_VehicleMaster WITH(NOLOCK) WHERE VariantId=@VariantId

		IF(@VehicleTypeId='2d566966-5525-4ed7-bd90-bb39e8418f39')
		BEGIN
			IF(@IsBrandNew = 1)
			BEGIN
				SELECT @PolicyTypeId='20541BE3-D76E-4E73-9AB1-240CCB33DA5D'
				SET  @OriginalPreviousPolicyType='1OD_3TP'
			END
			ELSE
			BEGIN
				IF(@PolicyTypeId IN ('517D8F9C-F532-4D45-8034-ABECE46693E3')) --COMPREHENSIVE
				BEGIN
					SET  @OriginalPreviousPolicyType='1OD_1TP'
				END
				ELSE IF(@PolicyTypeId IN ('2AA7FDCA-9E36-4A8D-9583-15ADA737574B')) --TP
				BEGIN
					SET @OriginalPreviousPolicyType='0OD_1TP'
				END
				ELSE IF(@PolicyTypeId IN ('48B01586-C66A-4A4A-AAFB-3F07F8A31896')) --SAOD
				BEGIN
					SET @OriginalPreviousPolicyType='1OD_0TP'
				END
			END	
		END
		ELSE IF(@VehicleTypeId='6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056')
		BEGIN
			IF(@IsBrandNew = 1)
			BEGIN
				SELECT @PolicyTypeId='20541BE3-D76E-4E73-9AB1-240CCB33DA5D'
				SET  @OriginalPreviousPolicyType='1OD_5TP'
			END
			ELSE
			BEGIN
				IF(@PolicyTypeId IN ('517D8F9C-F532-4D45-8034-ABECE46693E3')) --COMPREHENSIVE
				BEGIN
					SET @OriginalPreviousPolicyType='1OD_1TP'
				END
				ELSE IF(@PolicyTypeId IN ('2AA7FDCA-9E36-4A8D-9583-15ADA737574B')) --TP
				BEGIN
					SET @OriginalPreviousPolicyType='0OD_1TP'
				END
				ELSE IF(@PolicyTypeId IN ('48B01586-C66A-4A4A-AAFB-3F07F8A31896')) --SAOD
				BEGIN
					SET @OriginalPreviousPolicyType='1OD_0TP'
				END
			END
			
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

		SELECT @RTOCode AS RTOCode,
		@PreviousInsurerCode AS PreviousInsurerCode,
		@VehicleCode AS VehicleCode,
		@NCBVaule as NCBValue,
		@PreviousInsurancePolicyNumber PreviousInsurancePolicyNumber,
		@ChasisNo chassis,
		@EngineNo engine,
		@OriginalPreviousPolicyType OriginalPreviousPolicyType,
		@VehicleSeatCapacity AS VehicleSeatCapacity,
		@OriginalPreviousPolicyType AS PlanType,
		@PolicyType AS CurrentPolicyType,
		@IDVValue IDVValue,
		@MinIdv MinIdv,
		@MaxIdv MaxIdv,
		@RecommendedIdv RecommendedIdv 

		SELECT ConfigName, ConfigValue FROM Insurance_ICConfig WITH(NOLOCK) WHERE InsurerId=@InsurerId AND PolicyTypeId=@PolicyTypeId AND VehicleTypeId=@VehicleTypeId
		
		SELECT EXT.PACoverExtensionId, EXT.PACoverExtension, EXT.PACoverId
		FROM Insurance_PACoverExtension EXT WITH(NOLOCK) 
		INNER JOIN Insurance_PACover PA WITH(NOLOCK) ON PA.PACoverId = EXT.PACoverId 
		WHERE EXT.IsActive = 1 AND PA.PACoverId IN (SELECT VALUE FROM string_split(@PACoverId,',')) 
		AND EXT.PACoverExtensionId IN (SELECT VALUE FROM string_split(@PACoverExtensionId,','))

		SELECT EXT.DiscountExtensionId, EXT.DiscountExtension, EXT.DiscountId,Ext.DiscountValueInWords
		FROM Insurance_DiscountExtension EXT WITH(NOLOCK)
		INNER JOIN Insurance_Discounts PA WITH(NOLOCK) ON PA.DiscountId = EXT.DiscountId 
		WHERE EXT.IsActive = 1 AND PA.DiscountId IN (SELECT VALUE FROM string_split(@DiscountId,',')) 
		AND EXT.DiscountExtensionId IN (SELECT VALUE FROM string_split(@DiscountExtensionId,','))

		SELECT EXT.AddOnsExtensionId, EXT.AddOnsExtension, EXT.AddOnsId
		FROM Insurance_AddOnsExtension EXT WITH(NOLOCK)
		INNER JOIN Insurance_AddOns PA WITH(NOLOCK) ON PA.AddOnId = EXT.AddOnsId 
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
