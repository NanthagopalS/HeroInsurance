-- =============================================
-- Author:		Ravi Anand
-- Create date: 21-Oct-2023
-- Description:	
-- EXEC  [dbo].[SPGetAutoMatchingVMM] 'b9264799-8944-437a-b56c-20b2074e0042',0.50,0.50,0.0,0.0
-- =============================================
CREATE PROCEDURE [dbo].[SPGetAutoMatchingVMM] 
	@VarientId VARCHAR(100),
	@MMVScore FLOAT=0.8,
	@Mscore FLOAT=0.85,
	@Moscore FLOAT=0.0,
	@Vscore FLOAT=0.0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DECLARE @BajajMappingExist BIT=0,@ICICIMappingExist BIT=0,
@RelianceMappingExist BIT=0,@IFFCOMappingExist BIT=0,
@DIGITMappingExist BIT=0,@CHOLAMappingExist BIT=0,
@HDFCMappingExist BIT=0,@TATAMappingExist BIT=0

SELECT @BajajMappingExist= CASE WHEN ISNULL(VariantId,'')<>'' THEN 1 ELSE 0 END FROM HeroInsurance.MOTOR.Bajaj_VehicleMaster WHERE VariantId=@VarientId
SELECT @ICICIMappingExist= CASE WHEN ISNULL(VariantId,'')<>'' THEN 1 ELSE 0 END FROM HeroInsurance.MOTOR.ICICI_VehicleMaster WHERE VariantId=@VarientId
SELECT @RelianceMappingExist= CASE WHEN ISNULL(VarientId,'')<>'' THEN 1 ELSE 0 END FROM HeroInsurance.MOTOR.Reliance_VehicleMaster WHERE VarientId=@VarientId
SELECT @IFFCOMappingExist= CASE WHEN ISNULL(VariantId,'')<>'' THEN 1 ELSE 0 END FROM HeroInsurance.MOTOR.ITGI_VehicleMaster WHERE VariantId=@VarientId
SELECT @DIGITMappingExist= CASE WHEN ISNULL(VariantId,'')<>'' THEN 1 ELSE 0 END FROM HeroInsurance.MOTOR.GoDigit_VehicleMaster WHERE VariantId=@VarientId
SELECT @CHOLAMappingExist= CASE WHEN ISNULL(VarientId,'')<>'' THEN 1 ELSE 0 END FROM HeroInsurance.MOTOR.Chola_VehicleMaster WHERE VarientId=@VarientId
SELECT @HDFCMappingExist= CASE WHEN ISNULL(VariantId,'')<>'' THEN 1 ELSE 0 END FROM HeroInsurance.MOTOR.HDFC_VehicleMaster WHERE VariantId=@VarientId
SELECT @TATAMappingExist= CASE WHEN ISNULL(VarientId,'')<>'' THEN 1 ELSE 0 END FROM HeroInsurance.MOTOR.TATA_VehicleMaster WHERE VarientId=@VarientId


--	DECLARE @VarientId VARCHAR(100)='5ec937dc-633f-4059-8108-37c8ae33e34b';
--DECLARE @MMVScore AS FLOAT=.00
--DECLARE @Mscore  AS FLOAT=.85
--Bajaj

SELECT 'Bajaj'ICName,100 ScoreRank, 100.00 MScore, 100.00 MoScore,100.00 VScore,100.00 MMVScore,
Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.VehicleMake ICMake,IC.VehicleModel ICModel,IC.VehicleSubType ICVarient,IC.VehicleSubTypeCode ICVariantCode,IC.CarryingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.Fuel LIKE ('P%') THEN 'Petrol' WHEN  IC.Fuel='D' THEN 'Diesel' WHEN IC.Fuel='B' THEN 'Electric' WHEN IC.Fuel='C' THEN 'CNG' ELSE IC.Fuel END  ICFuel
FROM HeroInsurance.dbo.HeroBaseVehicleMaster Hero
INNER JOIN HeroInsurance.MOTOR.Bajaj_VehicleMaster IC ON Hero.VariantId=IC.VariantId
WHERE Hero.VariantId=@VarientId AND @BajajMappingExist=1

UNION ALL

SELECT 'Bajaj'ICName,* FROM (
SELECT  
ROW_NUMBER()OVER(partition By (HEROMake+HEROModel+HeroVarient) ORDER BY MMVScore Desc)ScoreRank,  *
FROM(
select 
dbo.fn_calculateJaroWinkler((IC.VehicleMake),(Hero.MakeName)) MScore,
dbo.fn_calculateJaroWinkler((IC.VehicleSubType),(Hero.VariantName)) VScore,
dbo.fn_calculateJaroWinkler((IC.VehicleModel),(Hero.ModelName)) MoScore,
dbo.fn_calculateJaroWinkler((IC.VehicleMake+IC.VehicleModel+IC.VehicleSubType),(Hero.MakeName+Hero.ModelName+Hero.VariantName)) MMVScore,

Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.VehicleMake ICMake,IC.VehicleModel ICModel,IC.VehicleSubType ICVarient,IC.VehicleSubTypeCode ICVariantCode,IC.CarryingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.Fuel LIKE ('P%') THEN 'Petrol' WHEN  IC.Fuel='D' THEN 'Diesel' WHEN IC.Fuel='B' THEN 'Electric' WHEN IC.Fuel='C' THEN 'CNG' ELSE IC.Fuel END  ICFuel
from
(SELECT * FROM HeroInsurance.MOTOR.Bajaj_VehicleMaster WHERE @BajajMappingExist<>1
)IC 
CROSS JOIN
(SELECT * FROM HeroInsurance.dbo.HeroBaseVehicleMaster WHERE VariantId=@VarientId
)Hero
)v WHERE v.MMVScore>@MMVScore AND v.MScore>@MScore AND v.VScore>@Vscore AND v.MoScore>@Moscore AND
HeroSC=ICSC AND HCC=CCC  AND HeroFuel LIKE ICFuel+'%' 
)v WHERE ScoreRank=1

UNION ALL

--Reliance
SELECT 'Reliance'ICName,100 ScoreRank, 100.00 MScore, 100.00 MoScore,100.00 VScore,100.00 MMVScore,
Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.MakeName ICMake,IC.ModelName ICModel,IC.Variance ICVarient,IC.VarientId ICVariantCode,IC.CarryingCapacity ICSC,IC.CC CCC,
CASE WHEN IC.OperatedBy LIKE ('P%') THEN 'Petrol' WHEN  IC.OperatedBy='D' THEN 'Diesel' WHEN IC.OperatedBy='B' THEN 'Electric' WHEN IC.OperatedBy='C' THEN 'CNG' ELSE IC.OperatedBy END  ICFuel
FROM HeroInsurance.dbo.HeroBaseVehicleMaster Hero
INNER JOIN HeroInsurance.MOTOR.Reliance_VehicleMaster IC ON Hero.VariantId=IC.VarientId
WHERE Hero.VariantId=@VarientId AND @RelianceMappingExist=1

UNION ALL

SELECT 'Reliance'ICName,* FROM (
SELECT  
ROW_NUMBER()OVER(partition By (HEROMake+HEROModel+HeroVarient) ORDER BY MMVScore Desc)ScoreRank,  *
FROM(
select
dbo.fn_calculateJaroWinkler((IC.MakeName),(Hero.MakeName)) MScore,
dbo.fn_calculateJaroWinkler((IC.ModelName),(Hero.ModelName)) MoScore,
dbo.fn_calculateJaroWinkler((IC.Variance),(Hero.VariantName)) VScore,
dbo.fn_calculateJaroWinkler((IC.MakeName+IC.ModelName+IC.Variance),(Hero.MakeName+Hero.ModelName+Hero.VariantName)) MMVScore,

Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.MakeName ICMake,IC.ModelName ICModel,IC.Variance ICVarient,IC.VarientId ICVariantCode,IC.CarryingCapacity ICSC,IC.CC CCC,
CASE WHEN IC.OperatedBy LIKE ('P%') THEN 'Petrol' WHEN  IC.OperatedBy='D' THEN 'Diesel' WHEN IC.OperatedBy='B' THEN 'Electric' WHEN IC.OperatedBy='C' THEN 'CNG' ELSE IC.OperatedBy END  ICFuel
from
(SELECT * FROM HeroInsurance.MOTOR.Reliance_VehicleMaster WHERE @RelianceMappingExist<>1
)IC 
CROSS JOIN
(SELECT * FROM HeroInsurance.dbo.HeroBaseVehicleMaster WHERE VariantId=@VarientId
)Hero
)v WHERE  v.MMVScore>@MMVScore AND v.MScore>@MScore AND v.VScore>@Vscore AND v.MoScore>@Moscore AND  
HeroSC=ICSC  AND HCC=CCC AND HeroFuel LIKE ICFuel+'%'
)v WHERE ScoreRank=1
UNION ALL
--ICICI
SELECT 'ICICI'ICName,100 ScoreRank, 100.00 MScore, 100.00 MoScore,100.00 VScore,100.00 MMVScore,
Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.Manufacture ICMake,IC.VehicleModel ICModel,IC.VehicleModel ICVarient,'' ICVariantCode,IC.SeatingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.FuelType LIKE ('P%') THEN 'Petrol' WHEN  IC.FuelType='D' THEN 'Diesel' WHEN IC.FuelType='B' THEN 'Electric' WHEN IC.FuelType='C' THEN 'CNG' ELSE IC.FuelType END  ICFuel
FROM HeroInsurance.dbo.HeroBaseVehicleMaster Hero
INNER JOIN HeroInsurance.MOTOR.ICICI_VehicleMaster IC ON Hero.VariantId=IC.VariantId
WHERE Hero.VariantId=@VarientId AND @ICICIMappingExist=1
UNION ALL
SELECT 'ICICI' ICName,* FROM (
SELECT  
ROW_NUMBER()OVER(partition By (HEROMake+HEROModel+HeroVarient) ORDER BY MMVScore Desc)ScoreRank,  *
FROM(
select 
dbo.fn_calculateJaroWinkler((IC.Manufacture),(Hero.MakeName)) MScore,
dbo.fn_calculateJaroWinkler((IC.VehicleModel),(Hero.ModelName)) MoScore,
dbo.fn_calculateJaroWinkler((IC.VehicleModel),(Hero.VariantName)) VScore,
dbo.fn_calculateJaroWinkler((IC.Manufacture+IC.VehicleModel),(Hero.MakeName+Hero.ModelName+Hero.VariantName)) MMVScore,

Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.Manufacture ICMake,IC.VehicleModel ICModel,IC.VehicleModel ICVarient,'' ICVariantCode,IC.SeatingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.FuelType LIKE ('P%') THEN 'Petrol' WHEN  IC.FuelType='D' THEN 'Diesel' WHEN IC.FuelType='B' THEN 'Electric' WHEN IC.FuelType='C' THEN 'CNG' ELSE IC.FuelType END  ICFuel
from
(SELECT * FROM HeroInsurance.MOTOR.ICICI_VehicleMaster WHERE @ICICIMappingExist<>1
)IC 
CROSS JOIN
(SELECT * FROM HeroInsurance.dbo.HeroBaseVehicleMaster WHERE VariantId=@VarientId
)Hero
)v WHERE v.MMVScore>@MMVScore AND v.MScore>@MScore AND v.VScore>@Vscore AND v.MoScore>@Moscore AND 
HeroSC=ICSC  AND HCC=CCC-- AND HeroFuel LIKE ICFuel+'%'
)v WHERE ScoreRank=1
UNION ALL
--IFFCO
SELECT 'IFFCO'ICName,100 ScoreRank, 100.00 MScore, 100.00 MoScore,100.00 VScore,100.00 MMVScore,
Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.Manufacture ICMake,IC.MODEL ICModel,IC.VARIANT ICVarient,IC.VARIANT ICVariantCode,IC.SEATING_CAPACITY ICSC,IC.CC CCC,
CASE WHEN IC.FUEL_TYPE LIKE ('P%') THEN 'Petrol' WHEN  IC.FUEL_TYPE='D' THEN 'Diesel' WHEN IC.FUEL_TYPE='B' THEN 'Electric' WHEN IC.FUEL_TYPE='C' THEN 'CNG' ELSE IC.FUEL_TYPE END  ICFuel
FROM HeroInsurance.dbo.HeroBaseVehicleMaster Hero
INNER JOIN HeroInsurance.MOTOR.ITGI_VehicleMaster IC ON Hero.VariantId=IC.VariantId
WHERE Hero.VariantId=@VarientId AND @IFFCOMappingExist=1
UNION ALL
SELECT 'IFFCO'ICName,* FROM (
SELECT  
ROW_NUMBER()OVER(partition By (HEROMake+HEROModel+HeroVarient) ORDER BY MMVScore Desc)ScoreRank,  *
FROM(
select 
dbo.fn_calculateJaroWinkler((IC.Manufacture),(Hero.MakeName)) MScore,
dbo.fn_calculateJaroWinkler((IC.MODEL),(Hero.ModelName)) MoScore,
dbo.fn_calculateJaroWinkler((IC.VARIANT),(Hero.VariantName)) VScore,
dbo.fn_calculateJaroWinkler((IC.Manufacture+IC.MODEL+IC.VARIANT),(Hero.MakeName+Hero.ModelName+Hero.VariantName)) MMVScore,

Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.Manufacture ICMake,IC.MODEL ICModel,IC.VARIANT ICVarient,IC.VARIANT ICVariantCode,IC.SEATING_CAPACITY ICSC,IC.CC CCC,
CASE WHEN IC.FUEL_TYPE LIKE 'P%' THEN 'Petrol' WHEN  IC.FUEL_TYPE='D' THEN 'Diesel' WHEN IC.FUEL_TYPE='B' THEN 'Electric' WHEN IC.FUEL_TYPE='C' THEN 'CNG' ELSE IC.FUEL_TYPE END  ICFuel
from
(SELECT * FROM HeroInsurance.MOTOR.ITGI_VehicleMaster WHERE @IFFCOMappingExist<>1
)IC 
CROSS JOIN
(SELECT * FROM HeroInsurance.dbo.HeroBaseVehicleMaster WHERE VariantId=@VarientId
)Hero
)v WHERE  v.MMVScore>@MMVScore AND v.MScore>@MScore AND v.VScore>@Vscore AND v.MoScore>@Moscore AND 
HeroSC=ICSC  AND HCC=CCC AND HeroFuel LIKE ICFuel+'%'
)v WHERE ScoreRank=1
UNION ALL
--TATA
SELECT 'TATA'ICName,100 ScoreRank, 100.00 MScore, 100.00 MoScore,100.00 VScore,100.00 MMVScore,
Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.TXT_MANUFACTURERNAME ICMake,IC.TXT_MODEL ICModel,IC.TXT_MODEL_VARIANT ICVarient,IC.NUM_MODEL_VARIANT_CODE ICVariantCode,IC.NUM_SEATING_CAPACITY ICSC,IC.NUM_CUBIC_CAPACITY CCC,
CASE WHEN IC.TXT_FUEL_TYPE LIKE ('P%') THEN 'Petrol' WHEN  IC.TXT_FUEL_TYPE='D' THEN 'Diesel' WHEN IC.TXT_FUEL_TYPE='B' THEN 'Electric' WHEN IC.TXT_FUEL_TYPE='C' THEN 'CNG' ELSE IC.TXT_FUEL_TYPE END  ICFuel
FROM HeroInsurance.dbo.HeroBaseVehicleMaster Hero
INNER JOIN HeroInsurance.MOTOR.TATA_VehicleMaster IC ON Hero.VariantId=IC.VarientId
WHERE Hero.VariantId=@VarientId AND @TATAMappingExist=1
UNION ALL
SELECT 'TATA'ICName,* FROM (
SELECT  
ROW_NUMBER()OVER(partition By (HEROMake+HEROModel+HeroVarient) ORDER BY MMVScore Desc)ScoreRank,  *
FROM(
select 
dbo.fn_calculateJaroWinkler((IC.TXT_MANUFACTURERNAME),(Hero.MakeName)) MScore,
dbo.fn_calculateJaroWinkler((IC.TXT_MODEL),(Hero.ModelName)) MoScore,
dbo.fn_calculateJaroWinkler((IC.TXT_MODEL_VARIANT),(Hero.VariantName)) VScore,
dbo.fn_calculateJaroWinkler((IC.TXT_MANUFACTURERNAME+IC.TXT_MODEL+IC.TXT_MODEL_VARIANT),(Hero.MakeName+Hero.ModelName+Hero.VariantName)) MMVScore,

Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.TXT_MANUFACTURERNAME ICMake,IC.TXT_MODEL ICModel,IC.TXT_MODEL_VARIANT ICVarient,IC.NUM_MODEL_VARIANT_CODE ICVariantCode,IC.NUM_SEATING_CAPACITY ICSC,IC.NUM_CUBIC_CAPACITY CCC,
CASE WHEN IC.TXT_FUEL_TYPE LIKE ('P%') THEN 'Petrol' WHEN  IC.TXT_FUEL_TYPE='D' THEN 'Diesel' WHEN IC.TXT_FUEL_TYPE='B' THEN 'Electric' WHEN IC.TXT_FUEL_TYPE='C' THEN 'CNG' ELSE IC.TXT_FUEL_TYPE END  ICFuel
from
(SELECT * FROM HeroInsurance.MOTOR.TATA_VehicleMaster WHERE @TATAMappingExist<>1
)IC 
CROSS JOIN
(SELECT * FROM HeroInsurance.dbo.HeroBaseVehicleMaster WHERE VariantId=@VarientId
)Hero
)v WHERE  v.MMVScore>@MMVScore AND v.MScore>@MScore AND v.VScore>@Vscore AND v.MoScore>@Moscore AND 
HeroSC=ICSC  AND HCC=CCC AND HeroFuel LIKE ICFuel+'%'
)v WHERE ScoreRank=1
UNION ALL
--DIGIT
SELECT 'DIGIT'ICName,100 ScoreRank, 100.00 MScore, 100.00 MoScore,100.00 VScore,100.00 MMVScore,
Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.Make ICMake,IC.Model,IC.Variant ICVarient,IC.Variant ICVariantCode,IC.SeatingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.FuelType LIKE ('P%') THEN 'Petrol' WHEN  IC.FuelType='D' THEN 'Diesel' WHEN IC.FuelType='B' THEN 'Electric' WHEN IC.FuelType='C' THEN 'CNG' ELSE IC.FuelType END  ICFuel
FROM HeroInsurance.dbo.HeroBaseVehicleMaster Hero
INNER JOIN HeroInsurance.MOTOR.GoDigit_VehicleMaster IC ON Hero.VariantId=IC.VariantId
WHERE Hero.VariantId=@VarientId AND @DIGITMappingExist=1
UNION ALL
SELECT 'DIGIT'ICName,* FROM (
SELECT  
ROW_NUMBER()OVER(partition By (HEROMake+HEROModel+HeroVarient) ORDER BY MMVScore Desc)ScoreRank,  *
FROM(
select
dbo.fn_calculateJaroWinkler((IC.Make),(Hero.MakeName)) MScore,
dbo.fn_calculateJaroWinkler((IC.Model),(Hero.ModelName)) MoScore,
dbo.fn_calculateJaroWinkler((IC.Variant),(Hero.VariantName)) VScore,
dbo.fn_calculateJaroWinkler((IC.Make+IC.Model+IC.Variant),(Hero.MakeName+Hero.ModelName+Hero.VariantName)) MMVScore,

Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.Make ICMake,IC.Model,IC.Variant ICVarient,IC.Variant ICVariantCode,IC.SeatingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.FuelType LIKE ('P%') THEN 'Petrol' WHEN  IC.FuelType='D' THEN 'Diesel' WHEN IC.FuelType='B' THEN 'Electric' WHEN IC.FuelType='C' THEN 'CNG' ELSE IC.FuelType END  ICFuel
from
(SELECT * FROM HeroInsurance.MOTOR.GoDigit_VehicleMaster WHERE @DIGITMappingExist<>1
)IC 
CROSS JOIN
(SELECT * FROM HeroInsurance.dbo.HeroBaseVehicleMaster WHERE VariantId=@VarientId
)Hero
)v WHERE v.MMVScore>@MMVScore AND v.MScore>@MScore AND v.VScore>@Vscore AND v.MoScore>@Moscore AND 
HeroSC=ICSC  AND HCC=CCC AND HeroFuel LIKE ICFuel+'%'
)v WHERE ScoreRank=1
UNION ALL
--HDFC
SELECT 'HDFC'ICName,100 ScoreRank, 100.00 MScore, 100.00 MoScore,100.00 VScore,100.00 MMVScore,
Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.MANUFACTURER ICMake,IC.VEHICLEMODEL,IC.TXT_VARIANT ICVarient,IC.TXT_VARIANT ICVariantCode,IC.SeatingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.TXT_FUEL LIKE ('P%') THEN 'Petrol' WHEN  IC.TXT_FUEL='D' THEN 'Diesel' WHEN IC.TXT_FUEL='B' THEN 'Electric' WHEN IC.TXT_FUEL='C' THEN 'CNG' ELSE IC.TXT_FUEL END  ICFuel
FROM HeroInsurance.dbo.HeroBaseVehicleMaster Hero
INNER JOIN HeroInsurance.MOTOR.HDFC_VehicleMaster IC ON Hero.VariantId=IC.VariantId
WHERE Hero.VariantId=@VarientId AND @HDFCMappingExist=1
UNION ALL
SELECT 'HDFC'ICName,* FROM (
SELECT  
ROW_NUMBER()OVER(partition By (HEROMake+HEROModel+HeroVarient) ORDER BY MMVScore Desc)ScoreRank,  *
FROM(
select 
dbo.fn_calculateJaroWinkler((IC.MANUFACTURER),(Hero.MakeName)) MScore,
dbo.fn_calculateJaroWinkler((IC.VEHICLEMODEL),(Hero.ModelName)) MoScore,
dbo.fn_calculateJaroWinkler((IC.TXT_VARIANT),(Hero.VariantName)) VScore,
dbo.fn_calculateJaroWinkler((IC.MANUFACTURER+IC.VEHICLEMODEL+IC.TXT_VARIANT),(Hero.MakeName+Hero.ModelName+Hero.VariantName)) MMVScore,

Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.MANUFACTURER ICMake,IC.VEHICLEMODEL,IC.TXT_VARIANT ICVarient,IC.TXT_VARIANT ICVariantCode,IC.SeatingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.TXT_FUEL LIKE ('P%') THEN 'Petrol' WHEN  IC.TXT_FUEL='D' THEN 'Diesel' WHEN IC.TXT_FUEL='B' THEN 'Electric' WHEN IC.TXT_FUEL='C' THEN 'CNG' ELSE IC.TXT_FUEL END  ICFuel
from
(SELECT * FROM HeroInsurance.MOTOR.HDFC_VehicleMaster WHERE @HDFCMappingExist<>1
)IC 
CROSS JOIN
(SELECT * FROM HeroInsurance.dbo.HeroBaseVehicleMaster WHERE VariantId=@VarientId
)Hero
)v WHERE v.MMVScore>@MMVScore AND v.MScore>@MScore AND v.VScore>@Vscore AND v.MoScore>@Moscore AND 
HeroSC=ICSC  AND HCC=CCC AND HeroFuel LIKE ICFuel+'%'
)v WHERE ScoreRank=1
UNION ALL
--Chola
SELECT DISTINCT 'Chola'ICName,100 ScoreRank, 100.00 MScore, 100.00 MoScore,100.00 VScore,100.00 MMVScore,
Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.Make ICMake,IC.VEHICLEMODEL,IC.VEHICLEMODEL ICVarient,'' ICVariantCode,IC.SeatingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.FuelType LIKE ('P%') THEN 'Petrol' WHEN  IC.FuelType='D' THEN 'Diesel' WHEN IC.FuelType='B' THEN 'Electric' WHEN IC.FuelType='C' THEN 'CNG' ELSE IC.FuelType END  ICFuel
FROM HeroInsurance.dbo.HeroBaseVehicleMaster Hero
INNER JOIN HeroInsurance.MOTOR.Chola_VehicleMaster IC ON Hero.VariantId=IC.VarientId
WHERE Hero.VariantId=@VarientId AND @CHOLAMappingExist=1
UNION ALL
SELECT 'Chola'ICName,* FROM (
SELECT  
ROW_NUMBER()OVER(partition By (HEROMake+HEROModel+HeroVarient) ORDER BY MMVScore Desc)ScoreRank,  *
FROM(
select 
dbo.fn_calculateJaroWinkler((IC.Make),(Hero.MakeName)) MScore,
dbo.fn_calculateJaroWinkler((IC.VEHICLEMODEL),(Hero.ModelName)) MoScore,
dbo.fn_calculateJaroWinkler((IC.VEHICLEMODEL),(Hero.VariantName)) VScore,
dbo.fn_calculateJaroWinkler((IC.Make+IC.VEHICLEMODEL),(Hero.MakeName+Hero.ModelName+Hero.VariantName)) MMVScore,

Hero.VariantId HeroVarientID, 
Hero.MakeName HEROMake,Hero.ModelName HeroModel,Hero.VariantName HeroVarient,Hero.SeatingCapacity HeroSC,Hero.CubicCapacity HCC,Hero.FuelName HeroFuel,
IC.Make ICMake,IC.VEHICLEMODEL,IC.VEHICLEMODEL ICVarient,'' ICVariantCode,IC.SeatingCapacity ICSC,IC.CubicCapacity CCC,
CASE WHEN IC.FuelType LIKE ('P%') THEN 'Petrol' WHEN  IC.FuelType='D' THEN 'Diesel' WHEN IC.FuelType='B' THEN 'Electric' WHEN IC.FuelType='C' THEN 'CNG' ELSE IC.FuelType END  ICFuel
from
(SELECT DISTINCT Make,VehicleModel,SeatingCapacity,CubicCapacity,FuelType FROM HeroInsurance.MOTOR.Chola_VehicleMaster WHERE @CHOLAMappingExist<>1
)IC 
CROSS JOIN
(SELECT * FROM HeroInsurance.dbo.HeroBaseVehicleMaster WHERE VariantId=@VarientId
)Hero
)v WHERE v.MMVScore>@MMVScore AND v.MScore>@MScore AND v.VScore>@Vscore AND v.MoScore>@Moscore AND 
HeroSC=ICSC  AND HCC=CCC AND HeroFuel LIKE ICFuel+'%'
)v WHERE ScoreRank=1

END