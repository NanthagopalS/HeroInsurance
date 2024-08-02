 --[dbo].[Insurance_GetIFFCOProposalDetails] '','','HERO/ENQ/110116'
CREATE   PROCEDURE [dbo].[Insurance_GetIFFCOProposalDetails]
@RTOCityCode VARCHAR(100) = NULL,        
@FinancierName VARCHAR(100) = NULL,        
@LeadId VARCHAR(50) = NULL
AS
BEGIN        
BEGIN TRY 
	DECLARE 
	@FinancierCode VARCHAR(50),
	@Zone VARCHAR(50),
    @TPStartDate VARCHAR(100),
    @TPEndDate VARCHAR(100),
	@ODStartDate VARCHAR(100),
    @ODEndDate VARCHAR(100),
	@TPPreviousInsurer VARCHAR(100),
	@ODPreviousInsurer VARCHAR(100),
    @TPPreviouspolicyNumber VARCHAR(100),
    @ODPreviouspolicyNumber VARCHAR(100),
	@PolictTypeId VARCHAR(100)

  SET NOCOUNT ON;     
  
	SELECT @FinancierCode = Code FROM MOTOR.ITGI_FinancierMaster WITH(NOLOCK) WHERE Financier = @FinancierName
	SELECT @Zone = IRDA_ZONE FROM MOTOR.ITGI_RTO_CityMaster WITH(NOLOCK) WHERE RTO_CITY_CODE = @RTOCityCode
	SELECT @PolictTypeId = PolicyTypeId FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadId

	IF(@PolictTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B')
	BEGIN

		SELECT @ODStartDate = PreviousSAODPolicyStartDate, 
		@ODEndDate = PrevPolicyExpiryDate,
		@ODPreviouspolicyNumber = PrevPolicyNumber,
		@TPStartDate = PreviousSATPPolicyStartDate,
		@TPEndDate = PrevPolicyExpiryDate,
		@TPPreviouspolicyNumber = PrevPolicyNumber
		FROM Insurance_LeadDetails WITH(NOLOCK)
		WHERE LeadId = @LeadId

		SELECT @TPPreviousInsurer = INS.PreviousInsurer
		FROM Insurance_LeadDetails LEADS WITH(NOLOCK) INNER JOIN
		MOTOR.ITGI_PreviousInsurerMaster INS ON LEADS.PreviousSATPInsurer = INS.InsurerId
		WHERE LeadId = @LeadId

		SET @ODPreviousInsurer = @TPPreviousInsurer

	END
	ELSE IF(@PolictTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896')
	BEGIN

		SELECT @ODStartDate = PreviousSAODPolicyStartDate, 
		@ODEndDate = PreviousPolicyExpirySAOD,
		@ODPreviouspolicyNumber = PreviousPolicyNumberSAOD,
		@TPStartDate = PreviousSATPPolicyStartDate,
		@TPEndDate = PrevPolicyExpiryDate,
		@TPPreviouspolicyNumber = PrevPolicyNumber
		FROM Insurance_LeadDetails  WITH(NOLOCK)
		WHERE LeadId = @LeadId
		
		SELECT @ODPreviousInsurer = INS.PreviousInsurer
		FROM Insurance_LeadDetails LEADS WITH(NOLOCK) INNER JOIN
		MOTOR.ITGI_PreviousInsurerMaster INS ON LEADS.PreviousSAODInsurer = INS.InsurerId
		WHERE LeadId = @LeadId

		SELECT @TPPreviousInsurer = INS.PreviousInsurer
		FROM Insurance_LeadDetails LEADS WITH(NOLOCK) INNER JOIN
		MOTOR.ITGI_PreviousInsurerMaster INS ON LEADS.PreviousSATPInsurer = INS.InsurerId
		WHERE LeadId = @LeadId
	END
	ELSE IF(@PolictTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3')
	BEGIN

		SELECT @ODStartDate = PreviousSAODPolicyStartDate, 
		@ODEndDate = PreviousPolicyExpirySAOD,
		@ODPreviouspolicyNumber = PreviousPolicyNumberSAOD,
		@TPStartDate = PreviousSAODPolicyStartDate,
		@TPEndDate = PreviousPolicyExpirySAOD,
		@TPPreviouspolicyNumber = PreviousPolicyNumberSAOD
		FROM Insurance_LeadDetails WITH(NOLOCK)
		WHERE LeadId = @LeadId

		SELECT @ODPreviousInsurer = INS.PreviousInsurer
		FROM Insurance_LeadDetails LEADS WITH(NOLOCK) INNER JOIN
		MOTOR.ITGI_PreviousInsurerMaster INS ON LEADS.PreviousSAODInsurer = INS.InsurerId
		WHERE LeadId = @LeadId

		SELECT @TPPreviousInsurer = INS.PreviousInsurer
		FROM Insurance_LeadDetails LEADS WITH(NOLOCK) INNER JOIN
		MOTOR.ITGI_PreviousInsurerMaster INS ON LEADS.PreviousSATPInsurer = INS.InsurerId
		WHERE LeadId = @LeadId
	END

	SELECT @FinancierCode FinancierCode ,
	@Zone Zone,
    @TPStartDate TPStartDate,
    @TPEndDate TPEndDate,
	@ODStartDate ODStartDate,
    @ODEndDate ODEndDate,
	@TPPreviousInsurer TPPreviousInsurer,
	@ODPreviousInsurer ODPreviousInsurer,
    @TPPreviouspolicyNumber TPPreviouspolicyNumber,
    @ODPreviouspolicyNumber ODPreviouspolicyNumber,
	@PolictTypeId PolictTypeId

 END TRY                        
 BEGIN CATCH                  
			   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                
 END CATCH
 END