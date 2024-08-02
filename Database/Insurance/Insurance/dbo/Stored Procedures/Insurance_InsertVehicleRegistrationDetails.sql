-- =============================================  
-- Author:  <Author,,FIROZ S>  
-- Create date: <Create Date,01-DEC-2022>  
-- Description: <Description,[Insurance_VehicleRegistrationDetails]>  
-- Fix :<Reason,"Need to return VariantName from Hero Master Data on 31-08-2023" UpdatedBy: Firoz>  
-- Fix :<Reason,"Added tags to prepolute details 'expirydate, previouspolicynumber,previousinsurer and default ncb value as per vehicle age' 11-10-2023" UpdatedBy: Firoz> 
-- =============================================  
-- exec [Insurance_InsertVehicleRegistrationDetails] @regNo= 'TS07FS1797',@VehicleTypeId='2d566966-5525-4ed7-bd90-bb39e8418f39', @Condition = 'CHECKVEHICLEDETAILS'
CREATE   PROCEDURE [dbo].[Insurance_InsertVehicleRegistrationDetails]  
(  
 @regNo VARCHAR(25)=NULL,  
 @class VARCHAR(25) =NULL,  
 @chassis VARCHAR(30) =NULL,  
 @engine VARCHAR(50) =NULL,  
 @vehicleManufacturerName VARCHAR(100) =NULL,  
 @model VARCHAR(25) =NULL,  
 @vehicleColour VARCHAR(25) =NULL,  
 @type VARCHAR(30) =NULL,  
 @normsType VARCHAR(30) =NULL,  
 @bodyType VARCHAR(30) =NULL,  
 @ownerCount VARCHAR(25) =NULL,  
 @owner VARCHAR(30) =NULL,  
 @ownerFatherName VARCHAR(30) =NULL,  
 @mobileNumber VARCHAR(15) =NULL,  
 @status VARCHAR(15) =NULL,  
 @statusAsOn VARCHAR(30) =NULL,  
 @regAuthority VARCHAR(30) =NULL,  
 @regDate VARCHAR(15) =NULL,  
 @vehicleManufacturingMonthYear VARCHAR(15) =NULL,  
 @rcExpiryDate VARCHAR(15) =NULL,  
 @vehicleTaxUpto VARCHAR(15) =NULL,  
 @vehicleInsuranceCompanyName VARCHAR(50) =NULL,  
 @vehicleInsuranceUpto VARCHAR(15) =NULL,  
 @vehicleInsurancePolicyNumber VARCHAR(30) =NULL,  
 @rcFinancer VARCHAR(50) =NULL,  
 @presentAddress VARCHAR (500) =NULL,  
 @splitPresentAddress VARCHAR(2000) =NULL,  
 @permanentAddress VARCHAR (500) =NULL,  
 @splitPermanentAddress VARCHAR(2000) =NULL,  
 @vehicleCubicCapacity VARCHAR (10) =NULL,  
 @grossVehicleWeight VARCHAR(10) =NULL,  
 @unladenWeight VARCHAR(10) =NULL,  
 @vehicleCategory VARCHAR(25) =NULL,  
 @rcStandardCap VARCHAR(10) =NULL,  
 @vehicleCylindersNo VARCHAR(10) =NULL,  
 @vehicleSeatCapacity VARCHAR (10) =NULL,  
 @vehicleSleeperCapacity VARCHAR(10) =NULL,  
 @vehicleStandingCapacity VARCHAR(10) =NULL,  
 @wheelbase VARCHAR(10) =NULL,  
 @vehicleNumber VARCHAR(15) =NULL,  
 @puccNumber VARCHAR(15) =NULL,  
 @puccUpto VARCHAR (15) =NULL,  
 @blacklistStatus VARCHAR(15) =NULL,  
 @blacklistDetails VARCHAR(2000) =NULL,  
 @challanDetails VARCHAR(2000) =NULL,  
 @permitIssueDate VARCHAR(15) =NULL,  
 @permitNumber VARCHAR(15) =NULL,  
 @permitType VARCHAR(30) =NULL,  
 @permitValidFrom VARCHAR(50) =NULL,  
 @permitValidUpto VARCHAR (15) =NULL,  
 @nonUseStatus VARCHAR(25) =NULL,  
 @nonUseFrom VARCHAR (25) =NULL,  
 @nonUseTo VARCHAR(25) =NULL,  
 @nationalPermitNumber VARCHAR (15) =NULL,  
 @nationalPermitUpto VARCHAR(15) =NULL,  
 @nationalPermitIssuedBy VARCHAR(50) =NULL,  
 @isCommercial VARCHAR(10) =NULL,  
 @nocDetails VARCHAR(50) =NULL,  
 @Condition VARCHAR(50)=NULL,  
 @VehicleTypeId VARCHAR(100)=NULL,  
 @VariantId VARCHAR(50)=NULL  
)  
AS  
BEGIN  
 BEGIN TRY  

	DECLARE @PrevInsurerCompanyName VARCHAR(500),@PrevInsurerId VARCHAR(100),@YearDiff INT,
			@PrevNCBId VARCHAR(100),@PrevNCBPercent VARCHAR(50)

 IF(@Condition = 'CHECKVEHICLEDETAILS')  
 BEGIN  
	SELECT @vehicleInsuranceCompanyName = VehicleInsuranceCompanyName,@regDate = regDate
	FROM Insurance_VehicleRegistration WITH(NOLOCK) WHERE regNo = @regNo

	SELECT @PrevInsurerCompanyName = (SELECT TOP(1) InsurerName FROM Insurance_Insurer WHERE InsurerType = 'Motor' 
										AND @vehicleInsuranceCompanyName LIKE CONCAT('%', ShortCode, '%'))

	SELECT @PrevInsurerId = (SELECT TOP(1) InsurerId FROM Insurance_Insurer WHERE InsurerType = 'Motor' 
										AND @vehicleInsuranceCompanyName LIKE CONCAT('%', ShortCode, '%'))
	SET @YearDiff = (DATEDIFF(year, CONVERT(DATETIME, @regDate, 105), GETDATE()))

	SELECT @PrevNCBId = NCBId, @PrevNCBPercent = PrevNCB  FROM Insurance_PrevNCBVehicleAge WHERE VehicleAgeYear = CAST(@YearDiff AS VARCHAR(10))
	IF(@YearDiff > 5)
	BEGIN
		SELECT @PrevNCBId = NCBId, @PrevNCBPercent = PrevNCB  FROM Insurance_PrevNCBVehicleAge WHERE VehicleAgeYear = 'other'
	END
		
	SELECT  VehicleId,MAKE.MakeName VehicleMake,  
	MODEL.ModelName VehicleModel,  
	VARIANT.VariantName VariantName,  
	FUEL.FuelName VehiceFuelType,  
	vehicleCubicCapacity VehicleCC,  
	regDate RegistrationDate,  
	regAuthority RTOLocation,  
	regNo RegistrationNumber,  
	VEHREG.VehicleTypeId,  
	class VehicleClass,  
	VEHREG.VariantId,
	CONVERT(VARCHAR(10), CONVERT(DATETIME, vehicleInsuranceUpto,105),103) as TPExpiryDate,
	@PrevInsurerCompanyName as PrevInsurerCompanyName,
	@PrevInsurerId as PrevInsurerId,
	@PrevNCBId as PrevNCBId,
	@PrevNCBPercent as PrevNCBPercent,
	vehicleInsurancePolicyNumber as PrevInsurancePolicyNumber
	FROM Insurance_VehicleRegistration VEHREG WITH(NOLOCK)   
	LEFT JOIN INSURANCE_VARIANT VARIANT WITH(NOLOCK)  ON VEHREG.VARIANTID = VARIANT.VARIANTID  
	LEFT JOIN Insurance_Model MODEL WITH(NOLOCK) ON VARIANT.ModelId=MODEL.ModelId  
	LEFT JOIN Insurance_Make MAKE WITH(NOLOCK) ON model.MakeId=MAKE.MakeId  
	LEFT JOIN Insurance_Fuel FUEL WITH(NOLOCK) ON VARIANT.FuelId=FUEL.FuelId  
	WHERE regNo=@regNo   
	AND DATEDIFF(MONTH, CAST(VehicleAddUpdateOn AS DATE), CAST(GETDATE() AS DATE)) <=6  
	AND VEHREG.VehicleTypeId=@VehicleTypeId AND ISNULL(VEHREG.VARIANTID,'')!= ''  
 END  
 ELSE  
 BEGIN  
	DECLARE @IsVehicleExists BIT = 0  
	IF(@VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39' AND @vehicleCategory IN ('LMV'))  
	BEGIN  
	SELECT @IsVehicleExists = 1  
	END  
	ELSE IF(@VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056' AND @vehicleCategory IN ('2W','SCR','2WN'))  
	BEGIN  
	SELECT @IsVehicleExists = 1  
	END  
	IF(@IsVehicleExists = 1)  
	BEGIN  
    
	DECLARE @vehicleId VARCHAR(50)  
	IF EXISTS (SELECT TOP 1 regNo FROM Insurance_VehicleRegistration WITH(NOLOCK) WHERE regNo=@regNo)  
	BEGIN  
	SET @vehicleId=(SELECT vehicleId FROM Insurance_VehicleRegistration WITH (NOLOCK) WHERE regNo=@regNo)  
  
		UPDATE [Insurance_VehicleRegistration] SET regNo=@regNo, class=@class, chassis=REPLACE(@chassis,' ',''), engine=REPLACE(@engine,' ',''),   
		vehicleManufacturerName=@vehicleManufacturerName, model=@model, vehicleColour=@vehicleColour, type=@type,   
		normsType=@normsType, bodyType=@bodyType, ownerCount=@ownerCount, owner=@owner, ownerFatherName=@ownerFatherName,   
		mobileNumber=@mobileNumber, status=@status, statusAsOn=@statusAsOn, regAuthority=@regAuthority, regDate=@regDate,   
		vehicleManufacturingMonthYear=@vehicleManufacturingMonthYear, rcExpiryDate=@rcExpiryDate, vehicleTaxUpto=@vehicleTaxUpto,   
		vehicleInsuranceCompanyName=@vehicleInsuranceCompanyName, vehicleInsuranceUpto=@vehicleInsuranceUpto,   
		vehicleInsurancePolicyNumber=@vehicleInsurancePolicyNumber, rcFinancer=@rcFinancer, presentAddress=@presentAddress,   
		splitPresentAddress=@splitPresentAddress, permanentAddress=@permanentAddress, splitPermanentAddress=@splitPermanentAddress,   
		vehicleCubicCapacity=@vehicleCubicCapacity, grossVehicleWeight=@grossVehicleWeight, unladenWeight=@unladenWeight,   
		vehicleCategory=@vehicleCategory, rcStandardCap=@rcStandardCap, vehicleCylindersNo=@vehicleCylindersNo,   
		vehicleSeatCapacity=@vehicleSeatCapacity, vehicleSleeperCapacity=@vehicleSleeperCapacity,   
		vehicleStandingCapacity=@vehicleStandingCapacity, wheelbase=@wheelbase, vehicleNumber=@vehicleNumber, puccNumber=@puccNumber,  
		puccUpto=@puccUpto, blacklistStatus=@blacklistStatus, blacklistDetails=@blacklistDetails, challanDetails=@challanDetails,   
		permitIssueDate=@permitIssueDate, permitNumber=@permitNumber, permitType=@permitType, permitValidFrom=@permitValidFrom,   
		permitValidUpto=@permitValidUpto, nonUseStatus=@nonUseStatus, nonUseFrom=@nonUseFrom, nonUseTo=@nonUseTo,   
		nationalPermitNumber=@nationalPermitNumber, nationalPermitUpto=@nationalPermitUpto,   
		nationalPermitIssuedBy=@nationalPermitIssuedBy, isCommercial=@isCommercial,nocDetails=@nocDetails,  
		UpdatedBy='1',UpdatedOn=GETDATE(),VehicleAddUpdateOn=GETDATE(), VariantId=@VariantId, VehicleTypeId=@VehicleTypeId  
		WHERE vehicleId=@vehicleId  
	END  
	ELSE  
	BEGIN  
   
	INSERT INTO [Insurance_VehicleRegistration] (regNo, class, chassis, engine, vehicleManufacturerName, model, vehicleColour,  
	type, normsType, bodyType, ownerCount, owner, ownerFatherName, mobileNumber, status, statusAsOn, regAuthority, regDate,  
	vehicleManufacturingMonthYear, rcExpiryDate, vehicleTaxUpto, vehicleInsuranceCompanyName, vehicleInsuranceUpto, vehicleInsurancePolicyNumber,  
	rcFinancer, presentAddress, splitPresentAddress, permanentAddress, splitPermanentAddress, vehicleCubicCapacity, grossVehicleWeight,  
	unladenWeight, vehicleCategory, rcStandardCap, vehicleCylindersNo, vehicleSeatCapacity, vehicleSleeperCapacity, vehicleStandingCapacity,  
	wheelbase, vehicleNumber, puccNumber, puccUpto , blacklistStatus, blacklistDetails, challanDetails, permitIssueDate, permitNumber, permitType,  
	permitValidFrom, permitValidUpto, nonUseStatus, nonUseFrom, nonUseTo, nationalPermitNumber, nationalPermitUpto, nationalPermitIssuedBy,  
	isCommercial, nocDetails, CreatedBy, CreatedOn,VehicleAddUpdateOn,VariantId,VehicleTypeId)  
	VALUES(@regNo , @class, REPLACE(@chassis,' ',''), REPLACE(@engine,' ',''), @vehicleManufacturerName, @model, @vehicleColour, @type, @normsType, @bodyType, @ownerCount,   
	@owner, @ownerFatherName, @mobileNumber, @status, @statusAsOn, @regAuthority, @regDate, @vehicleManufacturingMonthYear, @rcExpiryDate,   
	@vehicleTaxUpto, @vehicleInsuranceCompanyName, @vehicleInsuranceUpto, @vehicleInsurancePolicyNumber, @rcFinancer, @presentAddress,     
	@splitPresentAddress, @permanentAddress, @splitPermanentAddress, @vehicleCubicCapacity, @grossVehicleWeight, @unladenWeight,    
	@vehicleCategory, @rcStandardCap, @vehicleCylindersNo, @vehicleSeatCapacity, @vehicleSleeperCapacity, @vehicleStandingCapacity,    
	@wheelbase, @vehicleNumber, @puccNumber, @puccUpto, @blacklistStatus, @blacklistDetails, @challanDetails, @permitIssueDate,   
	@permitNumber, @permitType, @permitValidFrom, @permitValidUpto, @nonUseStatus, @nonUseFrom, @nonUseTo, @nationalPermitNumber,    
	@nationalPermitUpto, @nationalPermitIssuedBy, @isCommercial, @nocDetails,'1',GETDATE(),GETDATE(),@VariantId,@VehicleTypeId)  
  
	SET @vehicleId = @@IDENTITY  
	END  
	SELECT @PrevInsurerCompanyName = (SELECT TOP(1) InsurerName FROM Insurance_Insurer WHERE InsurerType = 'Motor' 
										AND @vehicleInsuranceCompanyName LIKE CONCAT('%', ShortCode, '%'))

	SELECT @PrevInsurerId = (SELECT TOP(1) InsurerId FROM Insurance_Insurer WHERE InsurerType = 'Motor' 
										AND @vehicleInsuranceCompanyName LIKE CONCAT('%', ShortCode, '%'))
	SET @YearDiff = (DATEDIFF(year, CONVERT(DATETIME, @regDate, 105), GETDATE()))

	SELECT @PrevNCBId = NCBId, @PrevNCBPercent = PrevNCB  FROM Insurance_PrevNCBVehicleAge WHERE VehicleAgeYear = CAST(@YearDiff AS VARCHAR(10))
	IF(@YearDiff > 5)
	BEGIN
		SELECT @PrevNCBId = NCBId, @PrevNCBPercent = PrevNCB  FROM Insurance_PrevNCBVehicleAge WHERE VehicleAgeYear = 'other'
	END

	SELECT  VehicleId,MAKE.MakeName VehicleMake,  
	MODEL.ModelName VehicleModel,  
	VARIANT.VariantName VariantName,  
	FUEL.FuelName VehiceFuelType,  
	vehicleCubicCapacity VehicleCC,  
	regDate RegistrationDate,  
	regAuthority RTOLocation,  
	regNo RegistrationNumber,  
	VEHREG.VehicleTypeId,  
	class VehicleClass,  
	VEHREG.VariantId,
	CONVERT(VARCHAR(10), CONVERT(DATETIME, vehicleInsuranceUpto,105),103) as TPExpiryDate,
	@PrevInsurerCompanyName as PrevInsurerCompanyName,
	@PrevInsurerId as PrevInsurerId,
	@PrevNCBId as PrevNCBId,
	@PrevNCBPercent as PrevNCBPercent,
	vehicleInsurancePolicyNumber as PrevInsurancePolicyNumber
	FROM Insurance_VehicleRegistration VEHREG WITH(NOLOCK)   
	LEFT JOIN INSURANCE_VARIANT VARIANT WITH(NOLOCK)  ON VEHREG.VARIANTID = VARIANT.VARIANTID  
	LEFT JOIN Insurance_Model MODEL WITH(NOLOCK) ON VARIANT.ModelId=MODEL.ModelId  
	LEFT JOIN Insurance_Make MAKE WITH(NOLOCK) ON model.MakeId=MAKE.MakeId  
	LEFT JOIN Insurance_Fuel FUEL WITH(NOLOCK) ON VARIANT.FuelId=FUEL.FuelId  
	WHERE regNo=@regNo   
	AND DATEDIFF(MONTH, CAST(VehicleAddUpdateOn AS DATE), CAST(GETDATE() AS DATE)) <=6  
	AND VEHREG.VehicleTypeId=@VehicleTypeId AND ISNULL(VEHREG.VARIANTID,'')!= ''    
  END  
 END  
  
 END TRY                  
 BEGIN CATCH            
         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END
