
-- =============================================                      
-- Author:  <Suraj Kumar Gupta>                      
-- Create date: <09-11-2023>                      
-- =============================================                      
CREATE
	

 PROCEDURE [dbo].[Admin_ResetMvvMappingForIcVariant] (
	@InsurerId VARCHAR(100),
	@HeroVariantId VARCHAR(100),
	@IcVariantId VARCHAR(100),
	@UpdatedBy VARCHAR(100)
	)
AS
BEGIN

	IF (@InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C') -- TATA  
	BEGIN
		UPDATE HeroInsurance.MOTOR.TATA_VehicleMaster SET VarientId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VarientId = @HeroVariantId AND SR_NO = @IcVariantId
	END

	IF (@InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37') -- BAJAJ  
	BEGIN
		UPDATE HeroInsurance.MOTOR.Bajaj_VehicleMaster SET VariantId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VariantId = @HeroVariantId AND VehicleCode = @IcVariantId
	END

	IF (@InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA') -- CHOLA  
	BEGIN
		UPDATE HeroInsurance.MOTOR.Chola_VehicleMaster SET VarientId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VarientId = @HeroVariantId AND ModelCode = @IcVariantId
	END

	IF (@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621') -- GO DIGIT  
	BEGIN
		UPDATE HeroInsurance.MOTOR.GoDigit_VehicleMaster SET VariantId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VariantId = @HeroVariantId AND [Vehicle Code] = @IcVariantId
	END

	IF (@InsurerId = '0A326B77-AFD5-44DA-9871-1742624CFF16') -- HDFC  
	BEGIN
		UPDATE HeroInsurance.MOTOR.HDFC_VehicleMaster SET VariantId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VariantId = @HeroVariantId AND VEHICLEMODELCODE = @IcVariantId
	END

	IF (@InsurerId = '372B076C-D9D9-48DC-9526-6EB3D525CAB6') -- Reliance  
	BEGIN
		UPDATE HeroInsurance.MOTOR.Reliance_VehicleMaster SET VarientId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VarientId = @HeroVariantId AND ModelID = @IcVariantId
	END

	IF (@InsurerId = 'FD3677E5-7938-46C8-9CD2-FAE188A1782C') -- ICICI  
	BEGIN
		UPDATE HeroInsurance.MOTOR.ICICI_VehicleMaster SET VariantId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VariantId = @HeroVariantId AND VehicleModelCode = @IcVariantId
	END

	IF (@InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3') -- Iffco  
	BEGIN
		UPDATE HeroInsurance.MOTOR.ITGI_VehicleMaster SET VariantId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VariantId = @HeroVariantId AND MAKE_CODE = @IcVariantId		
	END

	IF (@InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9') -- ORIANTAL  
	BEGIN
		UPDATE HeroInsurance.MOTOR.Oriental_VehicleMaster SET VariantId = NULL,UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE(),IsManuallyMapped = null WHERE VariantId = @HeroVariantId AND VEH_MODEL = @IcVariantId	
	END
END