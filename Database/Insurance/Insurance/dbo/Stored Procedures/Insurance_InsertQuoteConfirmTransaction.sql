/* exec [dbo].[Insurance_InsertQuoteConfirmTransaction] N'78190CB2-B325-4764-9BD9-5B9806E99621',
		N'{"enquiryId":"PVT_CAR_PACKAGE_2023040623","contract":{"insuranceProductCode":"20103","subInsuranceProductCode":"PB","startDate":"2023-04-10","endDate":"2024-04-09","policyHolderType":"INDIVIDUAL","currentNoClaimBonus":"THIRTY_FIVE","isNCBTransfer":false,"quotationDate":"2023-04-10","coverages":{"thirdPartyLiability":{"selection":false,"netPremium":null,"isTPPD":false},"ownDamage":{"selection":false,"netPremium":"INR 4136.58","traiffPremium":"INR 7170.00","withZeroDepNetPremium":"INR 3910.71","ncbDiscountWithZeroDep":"INR 1368.75","policyNetPremiumWithZeroDep":"INR 4725.96","policyGrossPremiumWithZeroDep":"INR 5576.64","withoutZeroDepNetPremium":"INR 4136.58","ncbDiscountWithoutZeroDep":"INR 1447.80","policyNetPremiumWithoutZeroDep":"INR 2688.78","policyGrossPremiumWithoutZeroDep":"INR 3172.76","netPremiumWithZeroDepWithPreInspection":"INR 3910.71","ncbDiscountWithZeroDepWithPreInspection":"INR 1368.75","policyNetPremiumWithZeroDepWithPreInspection":"INR 4725.96","policyGrossPremiumWithZeroDepWithPreInspection":"INR 5576.64","netPremiumWithZeroDepWithoutPreInspection":"INR 3910.71","ncbDiscountWithZeroDepWithoutPreInspection":"INR 1368.75","policyNetPremiumWithZeroDepWithoutPreInspection":"INR 4725.96","policyGrossPremiumWithZeroDepWithoutPreInspection":"INR 5576.64","netPremiumWithoutZeroDepWithPreInspection":"INR 4136.58","ncbDiscountWithoutZeroDepWithPreInspection":"INR 1447.80","policyNetPremiumWithoutZeroDepWithPreInspection":"INR 2688.78","policyGrossPremiumWithoutZeroDepWithPreInspection":"INR 3172.76","netPremiumWithoutZeroDepWithoutPreInspection":"INR 4136.58","ncbDiscountWithoutZeroDepWithoutPreInspection":"INR 1447.80","policyNetPremiumWithoutZeroDepWithoutPreInspection":"INR 2688.78","policyGrossPremiumWithoutZeroDepWithoutPreInspection":"INR 3172.76","discount":{"userSpecialDiscountPercent":0.0,"defaultSpecialDiscountPercent":62.5,"defaultSpecialDiscountPercentWithZeroDepWithPreInspection":64.55,"defaultSpecialDiscountPercentWithoutZeroDepWithPreInspection":62.5,"defaultSpecialDiscountPercentWithZeroDepWithoutPreInspection":64.55,"defaultSpecialDiscountPercentWithoutZeroDepWithoutPreInspection":62.5,"effectiveSpecialDiscountPercent":0.0,"effectiveSpecialDiscountPercentWithZeroDep":0.0,"effectiveSpecialDiscountPercentWithoutZeroDep":0.0,"effectiveSpecialDiscountPercentWithZeroDepWithPreInspection":0.0,"effectiveSpecialDiscountPercentWithoutZeroDepWithPreInspection":0.0,"effectiveSpecialDiscountPercentWithZeroDepWithoutPreInspection":0.0,"effectiveSpecialDiscountPercentWithoutZeroDepWithoutPreInspection":0.0,"minSpecialDiscountPercent":0.0,"maxSpecialDiscountPercent":85.0,"discounts":[{"discountType":"NCB_DISCOUNT","discountPercent":35.0,"discountAmount":"INR 1447.80"}],"discountsWithZeroDep":[{"discountType":"NCB_DISCOUNT","discountPercent":35.0,"discountAmount":"INR 1368.75"}],"discountsWithoutZeroDep":[{"discountType":"NCB_DISCOUNT","discountPercent":35.0,"discountAmount":"INR 1447.80"}],"discountsWithZeroDepWithPreInspection":[{"discountType":"NCB_DISCOUNT","discountPercent":35.0,"discountAmount":"INR 1368.75"}],"discountsWithoutZeroDepWithPreInspection":[{"discountType":"NCB_DISCOUNT","discountPercent":35.0,"discountAmount":"INR 1447.80"}],"discountsWithZeroDepWithoutPreInspection":[{"discountType":"NCB_DISCOUNT","discountPercent":35.0,"discountAmount":"INR 1368.75"}],"discountsWithoutZeroDepWithoutPreInspection":[{"discountType":"NCB_DISCOUNT","discountPercent":35.0,"discountAmount":"INR 1447.80"}]},"surcharge":{"loadings":[]}},"fire":{"selection":false},"theft":{"selection":false},"personalAccident":{"selection":false,"coverTerm":0,"coverAvailability":null,"netPremium":null},"accessories":{"cng":{"selection":false,"insuredAmount":10000.0,"minAllowed":10000,"maxAllowed":80000},"electrical":{"selection":false,"insuredAmount":34.0,"minAllowed":34,"maxAllowed":50400},"nonElectrical":{"selection":false,"insuredAmount":34.0,"minAllowed":34,"maxAllowed":33600}},"addons":{"partsDepreciation":{"claimsCovered":"TWO","selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 2184.00","netPremiumWithZeroDepWithPreInspection":"INR 2184.00","netPremiumWithZeroDepWithoutPreInspection":"INR 2184.00","netPremiumWithoutZeroDepWithPreInspection":"INR 2184.00","netPremiumWithoutZeroDepWithoutPreInspection":"INR 2184.00"},"personalBelonging":{"claimsCovered":"ONE_PB","selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 246.00","netPremiumWithZeroDepWithPreInspection":"INR 246.00","netPremiumWithZeroDepWithoutPreInspection":"INR 246.00","netPremiumWithoutZeroDepWithPreInspection":"INR 246.00","netPremiumWithoutZeroDepWithoutPreInspection":"INR 246.00"},"keyAndLockProtect":{"claimsCovered":"ONE_KP","selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 246.00","netPremiumWithZeroDepWithPreInspection":"INR 246.00","netPremiumWithZeroDepWithoutPreInspection":"INR 246.00","netPremiumWithoutZeroDepWithPreInspection":"INR 246.00","netPremiumWithoutZeroDepWithoutPreInspection":"INR 246.00"},"roadSideAssistance":{"selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 109.00"},"engineProtection":{"selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 352.80","netPremiumWithZeroDepWithPreInspection":"INR 352.80","netPremiumWithZeroDepWithoutPreInspection":"INR 352.80","netPremiumWithoutZeroDepWithPreInspection":"INR 352.80","netPremiumWithoutZeroDepWithoutPreInspection":"INR 352.80"},"tyreProtection":{"selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 46.63","netPremiumWithZeroDepWithPreInspection":"INR 46.63","netPremiumWithZeroDepWithoutPreInspection":"INR 46.63","netPremiumWithoutZeroDepWithPreInspection":"INR 46.63","netPremiumWithoutZeroDepWithoutPreInspection":"INR 46.63"},"rimProtection":{"selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 290.74","netPremiumWithZeroDepWithPreInspection":"INR 290.74","netPremiumWithZeroDepWithoutPreInspection":"INR 290.74","netPremiumWithoutZeroDepWithPreInspection":"INR 290.74","netPremiumWithoutZeroDepWithoutPreInspection":"INR 290.74"},"returnToInvoice":{"selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 1362.86","netPremiumWithZeroDepWithPreInspection":"INR 1362.86","netPremiumWithZeroDepWithoutPreInspection":"INR 1362.86","netPremiumWithoutZeroDepWithPreInspection":"INR 1362.86","netPremiumWithoutZeroDepWithoutPreInspection":"INR 1362.86"},"consumables":{"selection":false,"coverAvailability":"AVAILABLE","coverOfferingType":"INDIVIDUAL","netPremium":"INR 142.80","netPremiumWithZeroDepWithPreInspection":"INR 142.80","netPremiumWithZeroDepWithoutPreInspection":"INR 142.80","netPremiumWithoutZeroDepWithPreInspection":"INR 142.80","netPremiumWithoutZeroDepWithoutPreInspection":"INR 142.80"}},"legalLiability":{"paidDriverLL":{"selection":false,"netPremium":null,"insuredCount":0},"employeesLL":{"selection":false,"netPremium":null,"insuredCount":0},"unnamedPaxLL":{"selection":false,"netPremium":null,"insuredCount":0},"cleanersLL":{"selection":false},"nonFarePaxLL":{"selection":false},"workersCompensationLL":{"selection":false}},"unnamedPA":{"unnamedPax":{"selection":false,"insuredAmount":0.0,"netPremium":null},"unnamedPaidDriver":{"selection":false,"insuredAmount":0.0,"netPremium":null},"unnamedHirer":{"selection":false},"unnamedPillionRider":{"selection":false},"unnamedCleaner":{"selection":false},"unnamedConductor":{"selection":false}},"isGeoExt":false,"isTheftAndConversionRiskIMT43":false,"isIMT23":false,"isOverturningExclusionIMT47":false}},"vehicle":{"isVehicleNew":false,"vehicleMaincode":"1112613237","licensePlateNumber":"MH01QW9873","registrationAuthority":"MH01","vehicleIdentificationNumber":"MAKGM65DFHN300524","manufactureDate":"2021-04-01","registrationDate":"2021-04-01","vehicleIDV":{"idv":336000.0,"defaultIdv":336000.0,"minimumIdv":268800,"maximumIdv":403200},"trailers":[],"make":"MARUTI SUZUKI","model":"WAGON R"},"previousInsurer":{"isPreviousInsurerKnown":true,"previousPolicyExpiryDate":"2023-04-07","isClaimInLastYear":false,"previousNoClaimBonus":"TWENTY_FIVE","currentThirdPartyPolicy":{}},"pospInfo":{"isPOSP":false},"preInspection":{"isPreInspectionOpted":false,"isPreInspectionRequired":false,"isPreInspectionEligible":false,"isPreInspectionEligibleWithZeroDep":false,"isPreInspectionEligibleWithoutZeroDep":false,"isPreInspectionWaived":true,"isSchoolBusWaiverEligibleWithTPlusFortyEight":false,"preInspectionReasons":["BREAKIN_PI"],"piType":null},"motorTransits":[],"motorBreakIn":{"isBreakin":true,"isPreInspectionWaived":true,"isPreInspectionCompleted":false},"netPremium":"INR 2688.78","grossPremium":"INR 3172.76","discounts":{"discountType":null,"discountPercent":0,"discountAmount":null,"specialDiscountAmount":"INR 0.00","otherDiscounts":[{"discountType":"NCB_DISCOUNT","discountPercent":35.0,"discountAmount":"INR 1447.80"}]},"loadings":{"totalLoadingAmount":"INR 0.00","otherLoadings":[]},"serviceTax":{"cgst":"INR 241.99","sgst":"INR 241.99","igst":"INR 0.00","utgst":"INR 0.00","totalTax":"INR 483.98","taxType":"SGST_CGST"},"cesses":[],"informationMessages":[],"error":{"errorCode":0,"httpCode":200,"validationMessages":[],"errorLink":null,"errorStackTrace":null}}',
		N'{"enquiryId":"PVT_CAR_PACKAGE_2023040623","contract":{"insuranceProductCode":"20103","subInsuranceProductCode":"PB","startDate":"2023-04-11","endDate":"2024-04-10","policyHolderType":"INDIVIDUAL","externalPolicyNumber":null,"isNCBTransfer":false,"quotationDate":null,"coverages":{"voluntaryDeductible":"ZERO","thirdPartyLiability":null,"ownDamage":null,"personalAccident":{"selection":false,"coverTerm":0,"coverAvailability":null},"accessories":{"cng":{"selection":false,"insuredAmount":0,"minAllowed":0,"maxAllowed":0},"electrical":{"selection":false,"insuredAmount":0,"minAllowed":0,"maxAllowed":0},"nonElectrical":{"selection":false,"insuredAmount":0}},"addons":{"partsDepreciation":{"claimsCovered":null,"selection":false},"roadSideAssistance":{"selection":false},"engineProtection":{"selection":false},"tyreProtection":{"selection":false},"rimProtection":{"selection":false},"returnToInvoice":{"selection":false},"consumables":{"selection":false}},"legalLiability":null,"unnamedPA":{"unnamedPax":{"selection":false,"insuredAmount":0.0,"netPremium":null},"unnamedPaidDriver":{"selection":false,"insuredAmount":0.0,"netPremium":null},"unnamedHirer":{"selection":false,"insuredAmount":0,"insuredCount":0},"unnamedPillionRider":{"selection":false,"insuredAmount":0,"insuredCount":0},"unnamedCleaner":{"selection":false,"insuredAmount":0,"insuredCount":0},"unnamedConductor":{"selection":false,"insuredAmount":0,"insuredCount":0}},"isGeoExt":false,"isTheftAndConversionRiskIMT43":false,"isIMT23":false,"isOverturningExclusionIMT47":false}},"vehicle":{"isVehicleNew":false,"vehicleMaincode":"1112613237","licensePlateNumber":"MH01QW9873","vehicleIdentificationNumber":"MAKGM65DFHN300524","engineNumber":"L15Z14406601","manufactureDate":"2021-04-01","registrationDate":"2021-04-01","vehicleIDV":{"idv":0.0,"defaultIdv":0.0,"minimumIdv":0.0,"maximumIdv":0.0},"usageType":null,"permitType":null,"motorType":null,"trailers":null,"make":null,"model":null},"previousInsurer":{"isPreviousInsurerKnown":true,"previousInsurerCode":"113","previousPolicyNumber":"ASS223","previousPolicyExpiryDate":"2023-04-07","isClaimInLastYear":false,"originalPreviousPolicyType":"1OD_0TP","previousPolicyType":null,"previousNoClaimBonus":"TWENTY_FIVE","previousNoClaimBonusValue":null,"currentThirdPartyPolicy":{"isCurrentThirdPartyPolicyActive":false,"currentThirdPartyPolicyInsurerCode":"113","currentThirdPartyPolicyNumber":"ASS223","currentThirdPartyPolicyStartDateTime":"2021-04-01","currentThirdPartyPolicyExpiryDateTime":"2024-03-31"}},"pospInfo":{"isPOSP":false},"pincode":null}',
		N'{"InsurerStatusCode":200,"NewPremium":"3173","OldPremium":"3661","PremiumDifference":null,"Logo":null,"InsurerName":"GoDigit","TransactionId":null,"InsurerId":"78190CB2-B325-4764-9BD9-5B9806E99621","IDV":336000.0,"NCB":"35","TotalPremium":"2689","GrossPremium":"3173","Tax":{"cgst":null,"sgst":null,"igst":null,"utgst":null,"totalTax":"484","taxType":null},"ValidationMessage":null}',
		N'Quote',
		N'HERO/ENQ/102285',
		403200.0,
		268800.0,
		336000.0,
		NULL,
		N'2d566966-5525-4ed7-bd90-bb39e8418f39',
		N'7AF535AA-CBE6-4C88-9777-286BB23D8D17',
		N'0340F56C-02C3-4B02-9A21-E2AAA771D7AA',
		N'KA53',
		N'01-04-2021',
		N'01-04-2021',
		N'INDIVIDUAL',
		true,
		N'48B01586-C66A-4A4A-AAFB-3F07F8A31896',
		N'2023-04-07',
		N'ASS223',
		N'2023-04-07',
		N'ASS223',
		false,
		N'96BA4122-A900-468D-BB68-8BDA36F9ADA6',
		false,
		null,
		false,
		N'11-Apr-2023',
		N'10-Apr-2024'*/
CREATE PROCEDURE [dbo].[Insurance_InsertQuoteConfirmTransaction]      
@CoverMasterData dbo.CoverMasterDetailsType READONLY,
@InsurerId VARCHAR(50) = NULL,      
@ResponseBody VARCHAR(max) = NULL,       
@RequestBody VARCHAR(max) = NULL,       
@CommonResponse VARCHAR(max) = NULL,
@Stage varchar(20) = NULL,
@LeadId NVARCHAR(50) = NULL,
@MaxIDV decimal(18,2) = NULL,
@MinIDV decimal(18,2) = NULL,
@RecommendedIDV decimal(18,2) = NULL,
@TransactionId VARCHAR(100) = NULL,
@VehicleTypeId VARCHAR(100) = NULL,
@QuoteTransactionId VARCHAR(100) = NULL,
@UserId VARCHAR(100) = NULL,
@VehicleNumber VARCHAR(20) = NULL,
@ManufacturingMonthYear VARCHAR(20) = NULL,
@RegistrationDate VARCHAR(20) = NULL,
@CustomerType VARCHAR(30) = NULL,
@IsPreviousPolicy BIT = NULL,
@PreviousPolicyTypeId VARCHAR(100) = NULL,
@SAODPolicyExpiryDate VARCHAR(20) = NULL,
@SAODPolicyNumber VARCHAR(20) = NULL,
@SATPPolicyExpiryDate VARCHAR(20) = NULL,
@SATPPolicyNumber VARCHAR(20) = NULL,
@IsPreviousYearClaim BIT = NULL,
@PreviousNCBId VARCHAR(50) = NULL,
@IsPACover BIT = NULL,
@PACoverTenure INT = NULL,
@IsBrandNew BIT = NULL,
@CompanyName VARCHAR(100) = NULL,
@DateOfIncorporation VARCHAR(20) = NULL,
@GSTNumber NVARCHAR(50) = NULL,
@PolicyStartDate VARCHAR(20) = NULL,
@PolicyEndDate VARCHAR(20) = NULL,
@IsBreakin BIT = NULL,
@TotalPremium VARCHAR(50) = NULL,
@GrossPremiume VARCHAR(50) = NULL,
@Tax NVARCHAR(MAX) = NULL,
@NCBPercentage VARCHAR(50) = NULL,
@IsSelfInspection BIT = NULL,
@isPolicyExpired BIT = NULL,
@RTOCode VARCHAR(100) = NULL,
@PreviousSAODInsurer VARCHAR(100) = NULL,
@PreviousSATPInsurer VARCHAR(100) = NULL,
@IsQuoteDeviation BIT = NULL,
@IsApprovalRequired BIT = NULL,
@ProposalId VARCHAR(50) = NULL,
@PolicyId VARCHAR(50) = NULL,
@PreviousSATPPolicyStartDate VARCHAR(20) = NULL,
@PreviousSAODPolicyStartDate VARCHAR(20) = NULL,
@ResponseReferanceFlag VARCHAR(10) = NULL

AS      
BEGIN      
	BEGIN TRY      
  
	DECLARE @Logo Varchar(100), @QuoteTranID varchar(50) = null, @SelectedIDV VARCHAR(100) = (
	SELECT SelectedIDV FROM Insurance_QuoteTransaction WITH(NOLOCK) where QuoteTransactionId = @QuoteTransactionId AND InsurerId = @InsurerId),
	@StageID varchar(50) = (select stageid from Insurance_StageMaster where stage= @Stage) ;  
	SET @QuoteTranID = NEWID()  
	
	IF(ISNULL( @LeadId, '' ) = '' )  
	BEGIN  
	SELECT @LeadId = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuoteTransactionId
	END

	DELETE FROM Insurance_QuoteTransaction WHERE LeadId = @LeadId AND StageID = @StageID AND InsurerId = @InsurerId

	INSERT INTO dbo.Insurance_QuoteTransaction      
	(QuoteTransactionId, InsurerId,ResponseBody,RequestBody,CommonResponse,CreatedBy,StageID,LeadId,MaxIDV,MinIDV,RecommendedIDV,TransactionId,VehicleTypeId,PolicyTypeId,IsBrandNew,RefQuoteTransactionId,ProposalId,PolicyId)       
	VALUES (@QuoteTranID,@InsurerId,@ResponseBody,@RequestBody,@CommonResponse,@UserId,@StageID, @LeadId,@MaxIDV,@MinIDV,@RecommendedIDV, @TransactionId,@VehicleTypeId,@PreviousPolicyTypeId,@IsBrandNew,@QuoteTransactionId,@ProposalId,@PolicyId) 

	IF(@IsBrandNew=1)
	BEGIN
		UPDATE Insurance_LeadDetails SET 
		InsurerId = @InsurerId,
		IDV = @RecommendedIDV,
		MinIDV = @MinIDV,
		MaxIDV = @MaxIDV,
		VehicleTypeId=@VehicleTypeId,
		VehicleNumber=@VehicleNumber,
		MakeMonthYear=@ManufacturingMonthYear,
		RegistrationDate=@RegistrationDate,
		CarOwnedBy=@CustomerType,
		PolicyTypeId=@previousPolicyTypeId,
		PolicyStartDate=@PolicyStartDate,
		PolicyEndDate=@PolicyEndDate,
		IsPACover=@IsPACover,
		Tenure=@PACoverTenure,
		IsBrandNew=@IsBrandNew,
		CompanyName=@CompanyName,
		DateOfIncorporation=@DateOfIncorporation,
		GSTNo=@GSTNumber,
		UpdatedBy=@UserId,
		StageId = @StageID,
		UpdatedOn = GETDATE(),
		IsBreakin = @IsBreakin,
		TotalPremium = @TotalPremium,
		GrossPremium = @GrossPremiume,
		Tax = @Tax,
		NCBPercentage = @NCBPercentage,
		IsSelfInspection = @IsSelfInspection,
		QuoteTransactionID = @QuoteTranID,
		SelectedIDV = @SelectedIDV,
		RTOId = (SELECT RTOId FROM Insurance_RTO WHERE RTOCode = @RTOCode),
		IsQuoteDeviation = @IsQuoteDeviation,
		IsApprovalRequired = @IsApprovalRequired,
		ResponseReferanceFlag = @ResponseReferanceFlag
		WHERE LeadId = @LeadId
	END
	ELSE
	BEGIN
		UPDATE Insurance_LeadDetails SET 
		InsurerId = @InsurerId,
		IDV = @RecommendedIDV,
		MinIDV = @MinIDV,
		MaxIDV = @MaxIDV,
		VehicleTypeId=@VehicleTypeId,
		VehicleNumber=@VehicleNumber,
		MakeMonthYear=@ManufacturingMonthYear,
		RegistrationDate=@RegistrationDate,
		CarOwnedBy=@CustomerType,
		IsPrevPolicy=@IsPreviousPolicy,
		PolicyTypeId=@previousPolicyTypeId,
		PolicyStartDate=@PolicyStartDate,
		PolicyEndDate=@PolicyEndDate,
		PreviousPolicyNumberSAOD=@SAODPolicyNumber,
		PreviousPolicyExpirySAOD=@SAODPolicyExpiryDate,
		PrevPolicyNumber=@SATPPolicyNumber,
		PrevPolicyExpiryDate=@SATPPolicyExpiryDate,
		isPolicyExpired = @isPolicyExpired,
		PrevPolicyClaims= CASE WHEN @IsPreviousYearClaim=1 THEN 'YES' ELSE 'NO' END,
		PrevPolicyNCB= @PreviousNCBId,
		IsPACover=@IsPACover,
		Tenure=@PACoverTenure,
		IsBrandNew=@IsBrandNew,
		CompanyName=@CompanyName,
		DateOfIncorporation=@DateOfIncorporation,
		GSTNo=@GSTNumber,
		UpdatedBy=@UserId,
		StageId = @StageID,
		UpdatedOn = GETDATE(),
		IsBreakin = @IsBreakin,
		TotalPremium = @TotalPremium,
		GrossPremium = @GrossPremiume,
		Tax = @Tax,
		NCBPercentage = @NCBPercentage,
		IsSelfInspection = @IsSelfInspection,
		QuoteTransactionID = @QuoteTranID,
		SelectedIDV = @SelectedIDV,
		RTOId = (SELECT RTOId FROM Insurance_RTO WHERE RTOCode = @RTOCode),
		PreviousSAODInsurer = @PreviousSAODInsurer,
		PreviousSATPInsurer = @PreviousSATPInsurer,
		IsQuoteDeviation = @IsQuoteDeviation,
		IsApprovalRequired = @IsApprovalRequired,
		PreviousSATPPolicyStartDate = @PreviousSATPPolicyStartDate,
		PreviousSAODPolicyStartDate = @PreviousSAODPolicyStartDate,
		ResponseReferanceFlag = @ResponseReferanceFlag
		WHERE LeadId = @LeadId
	END
	 IF EXISTS (SELECT 1 FROM @CoverMasterData)
BEGIN
    EXEC Insurance_InsertPreviousCoverMasterDetails @CoverMasterData, @LeadId
END
	
	SELECT @Logo = Logo FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId = @InsurerId         
	
	SELECT @Logo Logo,
	@QuoteTranID QuoteTransactionId  

	
  
	END TRY      
 BEGIN CATCH                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH      
END
GO