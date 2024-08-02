
-- =============================================                      
-- Author:  <Suraj Kumar Gupta>                      
-- Create date: <09-11-2023>                      
-- Description: <getheroVariantListWithMappedNotMapped>                      
/*      
EXEC [Admin_GetAllVariantForCustomModel] '85F8472D-8255-4E80-B34A-61DB8678135C','RENAULT','TRIBER',null,null,null,null,null
*/
-- =============================================                      
CREATE
	

 PROCEDURE [dbo].[Admin_GetAllVariantForCustomModel] (
	@InsurerId VARCHAR(100),
	@MakeName VARCHAR(100),
	@ModelName1 VARCHAR(100) = NULL,
	@ModelName2 VARCHAR(100) = NULL,
	@ModelName3 VARCHAR(100) = NULL,
	@ModelName4 VARCHAR(100) = NULL,
	@ModelName5 VARCHAR(100) = NULL,
	@ModelName6 VARCHAR(100) = NULL
	)
AS
BEGIN
	DECLARE @ModelVariantMapping AS TABLE (
		ICName VARCHAR(100),
		HeroVarientID VARCHAR(100),
		ICVehicleCode VARCHAR(100),
		HEROMake VARCHAR(100),
		HeroModel VARCHAR(100),
		HeroVarient VARCHAR(100),
		HeroSC VARCHAR(100),
		HCC VARCHAR(100),
		HeroFuel VARCHAR(100),
		HeroGVW VARCHAR(100),
		ICMake VARCHAR(100),
		ICModel VARCHAR(100),
		ICVarient VARCHAR(100),
		ICSC VARCHAR(100),
		CCC VARCHAR(100),
		ICFuel VARCHAR(100),
		ICGVW VARCHAR(100),
		IsManuallyMapped BIT
		)

	IF (@InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C') -- TATA  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'TATA' AS IcName,
			IC.VarientId HeroVarientID,
			IC.SR_NO ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.TXT_MANUFACTURERNAME ICMake,
			IC.TXT_MODEL ICModel,
			IC.TXT_MODEL_VARIANT ICVarient,
			IC.NUM_SEATING_CAPACITY ICSC,
			IC.NUM_CUBIC_CAPACITY CCC,
			IC.TXT_FUEL_TYPE ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.TATA_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VarientId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.TXT_MANUFACTURERNAME LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR CONCAT (
					IC.TXT_MODEL,
					' ',
					IC.TXT_MODEL_VARIANT
					) LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR CONCAT (
					IC.TXT_MODEL,
					' ',
					IC.TXT_MODEL_VARIANT
					) LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR CONCAT (
					IC.TXT_MODEL,
					' ',
					IC.TXT_MODEL_VARIANT
					) LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR CONCAT (
					IC.TXT_MODEL,
					' ',
					IC.TXT_MODEL_VARIANT
					) LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR CONCAT (
					IC.TXT_MODEL,
					' ',
					IC.TXT_MODEL_VARIANT
					) LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR CONCAT (
					IC.TXT_MODEL,
					' ',
					IC.TXT_MODEL_VARIANT
					) LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.SR_NO
	END

	IF (@InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37') -- BAJAJ  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'Bajaj' AS IcName,
			IC.VariantId HeroVarientID,
			IC.VehicleCode ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.VehicleMake ICMake,
			IC.VehicleModel ICModel,
			IC.VehicleSubType ICVarient,
			IC.CarryingCapacity ICSC,
			IC.CubicCapacity CCC,
			IC.Fuel ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.Bajaj_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VariantId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.VehicleMake LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.VehicleSubType
					) LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.VehicleSubType
					) LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.VehicleSubType
					) LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.VehicleSubType
					) LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.VehicleSubType
					) LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.VehicleSubType
					) LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.VehicleCode
	END

	IF (@InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA') -- CHOLA  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'CHOLA' AS IcName,
			IC.VarientId HeroVarientID,
			IC.ModelCode ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.Make ICMake,
			IC.VehicleModel ICModel,
			IC.VehicleModel ICVarient,
			IC.SEATINGCAPACITY ICSC,
			IC.CubicCapacity CCC,
			IC.FuelType ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.Chola_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VarientId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.Make LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.ModelCode
	END

	IF (@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621') -- GO DIGIT  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'Go Digit' AS IcName,
			IC.VariantId HeroVarientID,
			IC.[Vehicle Code] ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.Make ICMake,
			IC.Model ICModel,
			IC.Variant ICVarient,
			IC.SEATINGCAPACITY ICSC,
			IC.CubicCapacity CCC,
			IC.FuelType ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.GoDigit_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VariantId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.Make LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR CONCAT (
					IC.Model,
					' ',
					IC.Variant
					) LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR CONCAT (
					IC.Model,
					' ',
					IC.Variant
					) LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR CONCAT (
					IC.Model,
					' ',
					IC.Variant
					) LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR CONCAT (
					IC.Model,
					' ',
					IC.Variant
					) LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR CONCAT (
					IC.Model,
					' ',
					IC.Variant
					) LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR CONCAT (
					IC.Model,
					' ',
					IC.Variant
					) LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.[Vehicle Code]
	END

	IF (@InsurerId = '0A326B77-AFD5-44DA-9871-1742624CFF16') -- HDFC  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'HDFC' AS IcName,
			IC.VariantId HeroVarientID,
			IC.VEHICLEMODELCODE ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.MANUFACTURER ICMake,
			IC.VehicleModel ICModel,
			IC.TXT_VARIANT ICVarient,
			IC.SEATINGCAPACITY ICSC,
			IC.CubicCapacity CCC,
			IC.TXT_FUEL ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.HDFC_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VariantId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.MANUFACTURER LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.TXT_VARIANT
					) LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.TXT_VARIANT
					) LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.TXT_VARIANT
					) LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.TXT_VARIANT
					) LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.TXT_VARIANT
					) LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR CONCAT (
					IC.VehicleModel,
					' ',
					IC.TXT_VARIANT
					) LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.VEHICLEMODELCODE
	END

	IF (@InsurerId = '372B076C-D9D9-48DC-9526-6EB3D525CAB6') -- Reliance  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'Reliance' AS IcName,
			IC.VarientId HeroVarientID,
			IC.ModelId ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.MakeName ICMake,
			IC.ModelName ICModel,
			IC.Variance ICVarient,
			IC.CarryingCapacity ICSC,
			IC.CC CCC,
			IC.OperatedBy ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.Reliance_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VarientId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.MakeName LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR CONCAT (
					IC.ModelName,
					' ',
					IC.Variance
					) LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR CONCAT (
					IC.ModelName,
					' ',
					IC.Variance
					) LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR CONCAT (
					IC.ModelName,
					' ',
					IC.Variance
					) LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR CONCAT (
					IC.ModelName,
					' ',
					IC.Variance
					) LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR CONCAT (
					IC.ModelName,
					' ',
					IC.Variance
					) LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR CONCAT (
					IC.ModelName,
					' ',
					IC.Variance
					) LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.ModelID
	END

	IF (@InsurerId = 'FD3677E5-7938-46C8-9CD2-FAE188A1782C') -- ICICI  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'ICICI' AS IcName,
			IC.VariantId HeroVarientID,
			IC.VehicleModelCode ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.Manufacture ICMake,
			IC.VehicleModel ICModel,
			IC.VehicleModel ICVarient,
			IC.SeatingCapacity ICSC,
			IC.CubicCapacity CCC,
			IC.FuelType ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.ICICI_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VariantId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.Manufacture LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR IC.VehicleModel LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.VehicleModelCode
	END

	IF (@InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3') -- Iffco  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'IFFCO' AS IcName,
			IC.VariantId HeroVarientID,
			IC.MAKE_CODE ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.Manufacture ICMake,
			IC.MODEL ICModel,
			IC.VARIANT ICVarient,
			IC.SEATING_CAPACITY ICSC,
			IC.CC CCC,
			IC.FUEL_TYPE ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.ITGI_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VariantId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.MANUFACTURE LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR CONCAT (
					IC.MODEL,
					' ',
					IC.VariantId
					) LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR CONCAT (
					IC.MODEL,
					' ',
					IC.VariantId
					) LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR CONCAT (
					IC.MODEL,
					' ',
					IC.VariantId
					) LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR CONCAT (
					IC.MODEL,
					' ',
					IC.VariantId
					) LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR CONCAT (
					IC.MODEL,
					' ',
					IC.VariantId
					) LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR CONCAT (
					IC.MODEL,
					' ',
					IC.VariantId
					) LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.MAKE_CODE
	END

	IF (@InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9') -- ORIANTAL  
	BEGIN
		INSERT INTO @ModelVariantMapping
		SELECT TOP 400 'IFFCO' AS IcName,
			IC.VariantId HeroVarientID,
			IC.VEH_MODEL ICVehicleCode,
			MA.MakeName HEROMake,
			MO.ModelName HeroModel,
			V.VariantName HeroVarient,
			V.SeatingCapacity HeroSC,
			V.CubicCapacity HCC,
			FE.FuelName HeroFuel,
			V.GVW HeroGVW,
			IC.VEH_MAKE_DESC ICMake,
			IC.VEH_MODEL_DESC ICModel,
			IC.VEH_MODEL_DESC ICVarient,
			IC.VEH_SEAT_CAP ICSC,
			IC.VEH_CC CCC,
			IC.VEH_FUEL_DESC ICFuel,
			0 ICGVW,
			IC.IsManuallyMapped
		FROM HeroInsurance.MOTOR.Oriental_VehicleMaster IC WITH (NOLOCK)
		LEFT JOIN HeroInsurance.dbo.Insurance_Variant V ON IC.VariantId = V.VariantId
		LEFT JOIN HeroInsurance.dbo.Insurance_Model MO ON V.ModelId = MO.ModelId
		LEFT JOIN HeroInsurance.dbo.Insurance_Make MA ON MO.MakeId = MA.MakeId
		LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FE ON V.FuelId = FE.FuelId
		WHERE IC.VEH_MAKE_DESC LIKE '%' + @MakeName + '%'
			AND (
				ISNULL(@ModelName1, '') = ''
				OR IC.VEH_MODEL_DESC LIKE '%' + @ModelName1 + '%'
				)
			AND (
				ISNULL(@ModelName2, '') = ''
				OR IC.VEH_MODEL_DESC LIKE '%' + @ModelName2 + '%'
				)
			AND (
				ISNULL(@ModelName3, '') = ''
				OR IC.VEH_MODEL_DESC LIKE '%' + @ModelName3 + '%'
				)
			AND (
				ISNULL(@ModelName4, '') = ''
				OR IC.VEH_MODEL_DESC LIKE '%' + @ModelName4 + '%'
				)
			AND (
				ISNULL(@ModelName5, '') = ''
				OR IC.VEH_MODEL_DESC LIKE '%' + @ModelName5 + '%'
				)
			AND (
				ISNULL(@ModelName6, '') = ''
				OR IC.VEH_MODEL_DESC LIKE '%' + @ModelName6 + '%'
				)
		ORDER BY IC.VEH_MODEL
	END

	SELECT *
	FROM @ModelVariantMapping
END