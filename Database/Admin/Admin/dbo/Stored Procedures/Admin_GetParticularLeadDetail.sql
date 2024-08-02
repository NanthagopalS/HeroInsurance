       
CREATE     PROCEDURE [dbo].[Admin_GetParticularLeadDetail]       
(  
	@LeadId VARCHAR(100)  
 ) 
AS      
	
BEGIN      
 BEGIN TRY
	DECLARE @StageId VARCHAR(50), @ProposalRequest NVARCHAR(MAX) = NULL, @VehicleNumber VARCHAR(50),
	@PaymentStatus VARCHAR(50) = NULL

	SELECT @StageId = StageId , @VehicleNumber = VehicleNumber FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] WHERE LeadId = @LeadId

	--Personal Detail

	IF(@StageId = 'ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2' OR @StageId = 'D07216AF-ACAD-4EEA-8CFF-2910BA77E5EE' OR @StageId = 'CD0D06FB-3AFC-4AF2-9FE6-3DBF98F0A105' OR @StageId = '405F4696-CDFB-4065-B364-9410B56BC78D')
	BEGIN
		SELECT @ProposalRequest = ProposalRequestBody FROM [HeroInsurance].[dbo].[Insurance_ProposalDynamicDetails] WHERE LeadId = @LeadId AND VehicleNumber = @VehicleNumber
	END

	SELECT LD.LeadId, LD.LeadName, LD.PhoneNumber, LD.Email, LD.Gender, LD.DOB,LD.PANNumber AS PANNumber, LD.AadharNumber AS AadharNumber,
		CONCAT(LAD.Address1, ' ', LAD.Address2) as Address, '' as Education,
		LD.Profession,
		@ProposalRequest AS ProposalRequestBody
	FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD
	LEFT JOIN [HeroInsurance].[dbo].[Insurance_LeadAddressDetails] LAD on LAD.LeadID = LD.LeadId
	WHERE LD.LeadId = @LeadId

	--Vehicle detail

	SELECT LEADDETAILS.VehicleNumber AS regNo, LEADDETAILS.RegistrationDate AS regDate, MAKE.MakeName as Maker, MODEL.ModelName as Model, VARIANT.VariantName AS Variant,VEHTYPE.VehicleType, VEHREG.[owner] as AccountHolder, LEADDETAILS.EngineNumber AS EngineNumber, LEADDETAILS.ChassisNumber AS ChassisNumber
	FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LEADDETAILS WITH(NOLOCK)
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_Variant] VARIANT WITH(NOLOCK) ON LEADDETAILS.VariantId = VARIANT.VariantId
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_Model] MODEL WITH(NOLOCK) ON MODEL.ModelId = VARIANT.ModelId
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_Make] MAKE WITH(NOLOCK) ON MODEL.MakeId = MAKE.MakeId
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_VehicleRegistration] VEHREG WITH(NOLOCK) ON VARIANT.VariantId = VEHREG.VariantId
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_VehicleType] VEHTYPE WITH(NOLOCK) ON VEHTYPE.InsuranceTypeId = LEADDETAILS.VehicleTypeId
	WHERE LEADDETAILS.LeadId = @LeadId
	ORDER BY LEADDETAILS.CreatedOn DESC

	IF(@StageId = 'D07216AF-ACAD-4EEA-8CFF-2910BA77E5EE')
	BEGIN
	--Policy Detail
		SELECT CASE 
			WHEN PAYMENT.Status = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8' THEN 'successful'
			WHEN PAYMENT.Status = '0151C6E3-8DC5-4BBD-860A-F1501A7647B2' THEN 'Pending'
			WHEN PAYMENT.Status = 'EBA950EF-6739-4236-8DF0-EA8E69E65C66' THEN 'Cancelled'
			END AS PolicyStatus,
		PAYSTATUS.PaymentStatus AS PaymentStatus,
		INSURETYPE.InsuranceName AS PolicyType,
		INSURER.InsurerName AS InsuranceCompanyName,
		PAYMENT.PolicyNumber AS PolicyNumber,
		PAYMENT.PaymentTransactionNumber AS TransactionNumber
		FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] IL WITH (NOLOCK)
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_PaymentTransaction] PAYMENT WITH (NOLOCK) ON PAYMENT.LeadId = IL.LeadId
			AND IL.QuoteTransactionID = PAYMENT.QuoteTransactionId
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_InsuranceType] INSURETYPE WITH (NOLOCK) ON INSURETYPE.InsuranceTypeId = IL.VehicleTypeId
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_PaymentStatus] PAYSTATUS WITH (NOLOCK) ON PAYSTATUS.PaymentId = PAYMENT.Status
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_Variant] VARIANT WITH (NOLOCK) ON VARIANT.VariantId = IL.VariantId
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_Insurer] INSURER WITH(NOLOCK) ON PAYMENT.InsurerId = INSURER.InsurerId
		WHERE IL.LeadId = @LeadId
	END
	ELSE
	BEGIN
	 SELECT @PaymentStatus AS PaymentStatus 
	END

 END TRY                      
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END 
