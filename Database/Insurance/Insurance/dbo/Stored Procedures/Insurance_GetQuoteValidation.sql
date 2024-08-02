


/*
EXEC Insurance_GetQuoteValidation @AddonId = '',
@DiscountId = '',
@PAcoverId = '1CA67DE4-782D-423C-8938-86E826754EB1,26894628-03A6-4156-932B-097A86A210C9',
@AccessoryId = '25C59740-45D0-4D08-AD86-4F3F1D50C801,2B631A39-7B53-4249-9F78-257952A81060',
@VarientId = 'AB5FED07-6B34-40E2-8F7C-00079200D35',
@VehicleTypeId = '80981D72-4C05-465E-B554-20B64BEF7536',
@PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B',
@RTOId = '7BAA9E1D-39D4-481A-A414-004B087634D'
*/

CREATE PROCEDURE [dbo].[Insurance_GetQuoteValidation]

@AddonId VARCHAR(2000),
@DiscountId VARCHAR(2000),
@PACoverId VARCHAR(2000),
@AccessoryId VARCHAR(2000),
@VarientId VARCHAR(50),
@VehicleTypeId VARCHAR(50),
@PolicyTypeId VARCHAR(50),
@RTOId VARCHAR(50),
@VehicleNumber VARCHAR(20)
AS
BEGIN
	BEGIN TRY
		DECLARE @IsAddonIdValid BIT =0, 
		@IsDiscountIdValid BIT = 0,
		@ISPacoverIdValid BIT = 0,
		@IsAccessoryIdValid BIT = 0,
		@IsVarientIdValid BIT =1,
		@IsVehicleTypeIdValid BIT =0,
		@IsPolicyTypeIdValid BIT =0,
		@IsRTOIdValid BIT =1

		--SET NOCOUNT ON;

		IF EXISTS(SELECT AddOnId FROM Insurance_AddOns WITH(NOLOCK) WHERE ISNULL(@AddonId,'')='' OR AddOnId IN (SELECT VALUE FROM string_split(@AddonId,',')))
		BEGIN
			SET @IsAddonIdValid = 1
		END

		IF EXISTS(SELECT DiscountId FROM Insurance_Discounts WITH(NOLOCK) WHERE ISNULL(@DiscountId,'')='' OR DiscountId IN (SELECT VALUE FROM string_split(@DiscountId,',')))
		BEGIN
			SET @IsDiscountIdValid = 1
		END

		IF EXISTS(SELECT PACoverId FROM Insurance_PACover WITH(NOLOCK) WHERE ISNULL(@PACoverId,'')='' OR PACoverId IN (SELECT VALUE FROM string_split(@PACoverId,',')))
		BEGIN
			SET @ISPacoverIdValid = 1
		END

		IF EXISTS(SELECT AccessoryId FROM Insurance_Accessory WITH(NOLOCK) WHERE ISNULL(@AccessoryId,'')='' OR AccessoryId IN (SELECT VALUE FROM string_split(@AccessoryId,',')))
		BEGIN
			SET @IsAccessoryIdValid = 1
		END

		IF NOT EXISTS( SELECT * FROM Insurance_Variant WITH(NOLOCK) WHERE VariantId = @VarientId)
		BEGIN
			SET @IsVarientIdValid = 0
		END

		IF EXISTS(SELECT * FROM Insurance_InsuranceType WITH(NOLOCK) WHERE InsuranceTypeId = @VehicleTypeId)
		BEGIN
			SET @IsVehicleTypeIdValid = 1
		END

		IF EXISTS(SELECT * FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE ISNULL(@PolicyTypeId,'')='' OR PreviousPolicyTypeId = @PolicyTypeId)
		BEGIN
			SET @IsPolicyTypeIdValid = 1
		END

		IF(ISNULL(@VehicleNumber,'')='')
		BEGIN
			IF NOT EXISTS(SELECT * FROM Insurance_RTO WITH(NOLOCK) WHERE ISNULL(@PolicyTypeId,'')='' OR RTOId = @RTOId)
			BEGIN
				SET @IsRTOIdValid = 0
			END
		END
		ELSE
		BEGIN
			SET @IsRTOIdValid = 1
		END
		

		SELECT @IsAddonIdValid AS IsAddonIdValid, 
		@IsDiscountIdValid AS IsDiscountIdValid,
		@ISPacoverIdValid AS ISPacoverIdValid,
		@IsAccessoryIdValid AS IsAccessoryIdValid,
		@IsVarientIdValid AS IsVarientIdValid,
		@IsVehicleTypeIdValid AS IsVehicleTypeIdValid,
		@IsPolicyTypeIdValid AS IsPolicyTypeIdValid,
		@IsRTOIdValid AS IsRTOIdValid

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
