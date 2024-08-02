   /*
   select * from dbo.[Insurance_GetVariantSuggestionForBajaj] ('4CDB28C1-740C-4E3C-8877-7F137D1D43A8')  
   */
CREATE        function [dbo].[Insurance_GetVariantSuggestionForBajaj]   
(      
@VariantId VARCHAR(100)
)      
returns @BajajVariantMapping TABLE(
	ICName VARCHAR(100),
	ScoreRank int,
	MScore float,
	VScore float,
	MoScore float,
	MMVScore float,
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
as   
BEGIN       
	
DECLARE @BajajMappingExist bit = 0,
		@FuelType VARCHAR(10) = ''
		,@IsAMT BIT = 0
		,@IsDark BIT = 0
		,@IsTourbo BIT = 0
		,@IsDualTone BIT = 0
		,@HasDecimal BIT = 0
		,@Is4x4 BIT = 0
		,@HasAbs BIT = 0

		SELECT @BajajMappingExist = CASE 
			WHEN VariantId = @VariantId
				THEN 1
			ELSE 0
			END
	FROM HeroInsurance.MOTOR.Bajaj_VehicleMaster WITH (NOLOCK)
	WHERE VariantId = @VariantId

	SELECT @FuelType =  CASE 
			WHEN FuelName = 'PETROL'
				THEN 'P'
			WHEN FuelName = 'Diesel'
				THEN 'D'
			WHEN FuelName = 'CNG'
				THEN 'C'
			WHEN FuelName = 'Electric'
				THEN 'B'
			ELSE 'PH'
			END
		,@IsAMT = CASE 
			WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%AMT%'
				THEN 1
			ELSE 0
			END
		,@HasDecimal = CASE 
			WHEN CHARINDEX('.', VariantName) > 0
				THEN 1
			ELSE 0
			END
		,@Is4x4 = CASE 
			WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%4X4%'
				THEN 1
			ELSE 0
			END
		,@IsDark = CASE 
			WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DARK%'
				THEN 1
			ELSE 0
			END
		,@IsTourbo = CASE 
			WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%TOURBO%'
				THEN 1
			ELSE 0
			END
		,@IsDualTone = CASE 
			WHEN (
					dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DUAL%'
					OR dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DT%'
					)
				THEN 1
			ELSE 0
			END
		,@HasAbs = CASE 
			WHEN VariantName LIKE '%ABS%'
				THEN 1
			ELSE 0
			END
	FROM Insurance_Variant VARIANT WITH (NOLOCK)
	LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
	WHERE VariantId = @VariantId



	INSERT INTO @BajajVariantMapping  
			SELECT 'Bajaj' ICName
			,100 ScoreRank
			,100.00 MScore
			,100.00 MoScore
			,100.00 VScore
			,100.00 MMVScore
			,VARIANT.VariantId HeroVarientID
			,IC.VehicleCode ICVehicleCode
			,MAKE.MakeName HEROMake
			,MODEL.ModelName HeroModel
			,VARIANT.VariantName HeroVarient
			,VARIANT.SeatingCapacity HeroSC
			,VARIANT.CubicCapacity HCC
			,FUEL.FuelName HeroFuel
			,IC.VehicleMake ICMake
			,IC.VehicleModel ICModel
			,IC.VehicleSubType ICVarient
			,IC.CarryingCapacity ICSC
			,IC.CubicCapacity CCC
			,CASE 
				WHEN IC.Fuel LIKE ('P%')
					THEN 'Petrol'
				WHEN IC.Fuel = 'D'
					THEN 'Diesel'
				WHEN IC.Fuel = 'B'
					THEN 'Electric'
				WHEN IC.Fuel = 'C'
					THEN 'CNG'
				ELSE IC.Fuel
				END ICFuel,
				VARIANT.GVW HeroGVW,
				'0' ICGVW,
	IC.IsManuallyMapped
		FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
		INNER JOIN MOTOR.Bajaj_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VariantId
		LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
		LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
		LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
		WHERE VARIANT.VariantId = @VariantId
			AND @BajajMappingExist = 1
			UNION ALL
		SELECT 'Bajaj' ICName
			,*
		FROM (
			SELECT ROW_NUMBER() OVER (
					PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC
					) ScoreRank
				,*
			FROM (
				SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleMake), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore
					,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleSubType), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore
					,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore
					,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
								IC.VehicleMake
								,IC.VehicleModel
								,IC.VehicleSubType
								)), dbo.RemoveNonAlphanumeric(CONCAT (
								Hero.MakeName
								,Hero.ModelName
								,Hero.VariantName
								)))*100 MMVScore
					,Hero.VariantId HeroVarientID
					,IC.VehicleCode ICVehicleCode
					,Hero.MakeName HEROMake
					,Hero.ModelName HeroModel
					,Hero.VariantName HeroVarient
					,Hero.SeatingCapacity HeroSC
					,Hero.CubicCapacity HCC
					,Hero.FuelName HeroFuel
					,IC.VehicleMake ICMake
					,IC.VehicleModel ICModel
					,IC.VehicleSubType ICVarient
					,IC.CarryingCapacity ICSC
					,IC.CubicCapacity CCC
					,CASE 
						WHEN IC.Fuel  ='P'
							THEN 'Petrol'
						WHEN IC.Fuel = 'D'
							THEN 'Diesel'
						WHEN IC.Fuel = 'B'
							THEN 'Electric'
						WHEN IC.Fuel = 'C'
							THEN 'CNG'
						ELSE IC.Fuel
						END ICFuel,
						Hero.GVW HeroGVW,
						'0' ICGVW,
	IC.IsManuallyMapped
				FROM (
					SELECT *
					FROM MOTOR.Bajaj_VehicleMaster WITH (NOLOCK)
					WHERE VariantId IS NULL
					AND @BajajMappingExist <> 1
						AND Fuel = @FuelType
						AND (
							VehicleSubType LIKE CASE 
								WHEN @IsAMT = 1
									THEN '%AMT%'
								END
							OR ISNULL(@IsAMT, 0) = 0
							)
						AND (
							VehicleSubType LIKE CASE 
								WHEN @IsDark = 1
									THEN '%DARK%'
								END
							OR ISNULL(@IsDark, 0) = 0
							)
						AND (
							VehicleSubType LIKE CASE 
								WHEN @IsTourbo = 1
									THEN '%TOURBO%'
								END
							OR ISNULL(@IsTourbo, 0) = 0
							)
						AND (
							VehicleSubType LIKE CASE 
								WHEN @HasAbs = 1
									THEN '%ABS%'
								END
							OR ISNULL(@HasAbs, 0) = 0
							)
						AND (
							CHARINDEX('.', VehicleSubType) > 0
							OR ISNULL(@HasDecimal, 0) = 0
							)
						AND (
							VehicleSubType LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4*4%'
								END
							OR VehicleSubType LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%4x4%'
								END
							OR VehicleSubType LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%4WD%'
								END
							OR ISNULL(@Is4x4, 0) = 0
							)
						AND (
							VehicleSubType LIKE CASE 
								WHEN @HasAbs = 1
									THEN '%ABS%'
								END
							OR ISNULL(@HasAbs, 0) = 0
							)
						AND (
							VehicleSubType LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%DUAL TONE%'
								END
							OR VehicleSubType LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%DT%'
								END
							OR ISNULL(@IsDualTone, 0) = 0
							)
					) IC
				CROSS JOIN (
					SELECT VariantId
						,MakeName
						,ModelName
						,VariantName
						,SeatingCapacity
						,CubicCapacity
						,FuelName
						,GVW
					FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
					LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
					LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId
					LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
					WHERE VariantId = @VariantId
					) Hero
				) v
			WHERE  HeroSC = ICSC
				AND HCC = CCC
			
			) v
		WHERE ScoreRank = 1
return  
END