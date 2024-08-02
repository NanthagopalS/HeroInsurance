
/*
   select * from dbo.[Insurance_GetAllVariantForModel] ('16413879-6316-4C1E-93A4-FF8318B14D37','6D1A5B83-FF5A-4401-A661-4F488373F636')  
   */
CREATE FUNCTION [dbo].[Insurance_GetAllVariantForModel] (
	@InsurerId VARCHAR(100),
	@ModelId VARCHAR(100)
	)
RETURNS @ModelVariantMapping TABLE (
	ICName VARCHAR(100),
	MScore FLOAT,
	MoScore FLOAT,
	MMVScore FLOAT,
	HeroVarientID VARCHAR(100),
	ICVehicleCode VARCHAR(100),
	HEROMake VARCHAR(100),
	HeroModel VARCHAR(100),
	HeroVarient VARCHAR(100),
	HeroSC VARCHAR(100),
	HCC VARCHAR(100),
	HeroFuel VARCHAR(100),
	ICMake VARCHAR(100),
	ICModel VARCHAR(100),
	ICVarient VARCHAR(100),
	ICSC VARCHAR(100),
	CCC VARCHAR(100),
	ICFuel VARCHAR(100),
	HeroGVW VARCHAR(100),
	ICGVW VARCHAR(100),
	IsManuallyMapped bit
	)
AS
BEGIN
	DECLARE @HeroMakeName VARCHAR(100),
		@HeroModelName VARCHAR(100)

	SELECT @HeroMakeName = MAKE.MakeName,
		@HeroModelName = MODEL.ModelName
	FROM Insurance_Model MODEL WITH (NOLOCK)
	JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId
	WHERE MODEL.ModelId = @ModelId

	IF (@InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C') -- TATA  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'TATA' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.TXT_MANUFACTURERNAME), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.TXT_MODEL), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.TXT_MANUFACTURERNAME,
							IC.TXT_MODEL
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VarientId HeroVarientID,
				IC.SR_NO ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.TXT_MANUFACTURERNAME ICMake,
				IC.TXT_MODEL ICModel,
				IC.TXT_MODEL_VARIANT ICVarient,
				IC.NUM_SEATING_CAPACITY ICSC,
				IC.NUM_CUBIC_CAPACITY CCC,
				IC.TXT_FUEL_TYPE ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.TATA_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.TXT_MANUFACTURERNAME) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.TXT_MODEL) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	IF (@InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37') -- BAJAJ  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'Bajaj' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleMake), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.VehicleMake,
							IC.VehicleModel
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VariantId HeroVarientID,
				IC.VehicleCode ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.VehicleMake ICMake,
				IC.VehicleModel ICModel,
				IC.VehicleSubType ICVarient,
				IC.CarryingCapacity ICSC,
				IC.CubicCapacity CCC,
				IC.Fuel ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.Bajaj_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.VehicleMake) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.VehicleModel) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	IF (@InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA') -- CHOLA  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'CHOLA' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Make), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.Make,
							IC.VehicleModel
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VarientId HeroVarientID,
				IC.ModelCode ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.Make ICMake,
				IC.VehicleModel ICModel,
				IC.VehicleModel ICVarient,
				IC.SEATINGCAPACITY ICSC,
				IC.CubicCapacity CCC,
				IC.FuelType ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.Chola_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.Make) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.VehicleModel) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	IF (@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621') -- GO DIGIT  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'Go Digit' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Make), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Model), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.Make,
							IC.Model
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VariantId HeroVarientID,
				IC.[Vehicle Code] ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.Make ICMake,
				IC.Model ICModel,
				IC.Variant ICVarient,
				IC.SEATINGCAPACITY ICSC,
				IC.CubicCapacity CCC,
				IC.FuelType ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.GoDigit_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.Make) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.Model) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	IF (@InsurerId = '0A326B77-AFD5-44DA-9871-1742624CFF16') -- HDFC  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'HDFC' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.MANUFACTURER), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.MANUFACTURER,
							IC.VehicleModel
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VariantId HeroVarientID,
				IC.VEHICLEMODELCODE ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.MANUFACTURER ICMake,
				IC.VehicleModel ICModel,
				IC.TXT_VARIANT ICVarient,
				IC.SEATINGCAPACITY ICSC,
				IC.CubicCapacity CCC,
				IC.TXT_FUEL ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.HDFC_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.MANUFACTURER) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.VehicleModel) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	IF (@InsurerId = '372B076C-D9D9-48DC-9526-6EB3D525CAB6') -- Reliance  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'Reliance' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.MakeName), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.ModelName), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.MakeName,
							IC.ModelName
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VarientId HeroVarientID,
				IC.ModelId ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.MakeName ICMake,
				IC.ModelName ICModel,
				IC.Variance ICVarient,
				IC.CarryingCapacity ICSC,
				IC.CC CCC,
				IC.OperatedBy ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.Reliance_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.MakeName) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.ModelName) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	IF (@InsurerId = 'FD3677E5-7938-46C8-9CD2-FAE188A1782C') -- ICICI  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'ICICI' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Manufacture), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.Manufacture,
							IC.VehicleModel
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VariantId HeroVarientID,
				IC.VehicleModelCode ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.Manufacture ICMake,
				IC.VehicleModel ICModel,
				IC.VehicleModel ICVarient,
				IC.SeatingCapacity ICSC,
				IC.CubicCapacity CCC,
				IC.FuelType ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.ICICI_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.Manufacture) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.VehicleModel) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	IF (@InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3') -- Iffco  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'IFFCO' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Manufacture), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.MODEL), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.Manufacture,
							IC.MODEL
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VariantId HeroVarientID,
				IC.MAKE_CODE ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.Manufacture ICMake,
				IC.MODEL ICModel,
				IC.VARIANT ICVarient,
				IC.SEATING_CAPACITY ICSC,
				IC.CC CCC,
				IC.FUEL_TYPE ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.ITGI_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.Manufacture) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.MODEL) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	IF (@InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9') -- ORIANTAL  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT 'IFFCO' AS IcName,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VEH_MAKE_DESC), dbo.RemoveNonAlphanumeric(@HeroMakeName)) * 100 MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VEH_MODEL_DESC), dbo.RemoveNonAlphanumeric(@HeroModelName)) * 100 MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.VEH_MAKE_DESC,
							IC.VEH_MODEL_DESC
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							@HeroMakeName,
							@HeroModelName
							))) * 100 MMVScore,
				IC.VariantId HeroVarientID,
				IC.VEH_MODEL ICVehicleCode,
				'' HEROMake,
				'' HeroModel,
				'' HeroVarient,
				'' HeroSC,
				'' HCC,
				'' HeroFuel,
				IC.VEH_MAKE_DESC ICMake,
				IC.VEH_MODEL_DESC ICModel,
				IC.VEH_MODEL_DESC ICVarient,
				IC.VEH_SEAT_CAP ICSC,
				IC.VEH_CC CCC,
				IC.VEH_FUEL_DESC ICFuel,
				'' HeroGVW,
				0 ICGVW,
				IC.IsManuallyMapped
			FROM MOTOR.Oriental_VehicleMaster IC WITH (NOLOCK)
			WHERE dbo.RemoveNonAlphanumeric(IC.VEH_MAKE_DESC) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroMakeName)+'%'
			AND dbo.RemoveNonAlphanumeric(IC.VEH_MODEL_DESC) LIKE '%'+dbo.RemoveNonAlphanumeric(@HeroModelName)+'%'
			) RES
		WHERE MMVScore > 70
			AND MScore > 80
	END

	UPDATE @ModelVariantMapping
	SET HCC = VARIANT.CubicCapacity,
		HeroMake = MAKE.MakeName,
		HeroModel = MODEL.ModelName,
		HeroVarient = VARIANT.VariantName,
		HeroSC = VARIANT.SeatingCapacity,
		HeroFuel = FUEL.FuelName,
		HeroGVW = VARIANT.GVW
	FROM @ModelVariantMapping c
	LEFT JOIN INSURANCE_VARIANT VARIANT WITH (NOLOCK) ON C.HeroVarientID = VARIANT.VariantId
	LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
	LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId
	LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
	WHERE ISNULL(C.HeroVarientID, '') != ''

	RETURN
END