
CREATE    PROCEDURE [dbo].[Insurance_GetAllReport]             
(        
	 @EnquiryId VARCHAR(100) = "0DA11F5A-B506-4D0A-AC16-02C6A7516379",       
	 @ProductType VARCHAR(100) = NULL,          
	 @Insurertype VARCHAR(100) = NULL,  
	 @Stage VARCHAR(100) = NULL,
	 @StartDate VARCHAR(100) = NULL,        
	 @EndDate VARCHAR(100) = NULL,    
	 @CurrentPageIndex INT = 1,      
	 @CurrentPageSize INT = 500
 )       
AS            
BEGIN            
 BEGIN TRY

   DECLARE @TotalRecord INT =0
  
	select  LEADDETAILS.LeadName, LEADDETAILS.LeadId as EnquiryId, LEADDETAILS.PhoneNumber,
		LEADDETAILS.GrossPremium as Premium, CAST(LEADDETAILS.CreatedOn as Date) as GeneratedOn,
		INSTYPE.InsuranceName as Product, INSURER.InsurerName as InsuranceCompany,
		STAGEMASTER.Stage
		INTO #ReportTemp
		FROM [Insurance_LeadDetails] LEADDETAILS WITH(NOLOCK)
		LEFT JOIN [Insurance_InsuranceType] INSTYPE WITH(NOLOCK) ON LEADDETAILS.VehicleTypeId = INSTYPE.InsuranceTypeId
		LEFT JOIN [Insurance_PaymentTransaction] PT WITH(NOLOCK) on LEADDETAILS.QuoteTransactionID = PT.QuoteTransactionId
		LEFT JOIN [Insurance_Insurer] INSURER WITH(NOLOCK) ON LEADDETAILS.InsurerId=INSURER.InsurerId 
		LEFT JOIN Insurance_PreviousPolicyType POLICYTYPE WITH(NOLOCK) ON LEADDETAILS.PolicyTypeId=POLICYTYPE.PreviousPolicyTypeId
		LEFT JOIN [Insurance_StageMaster] STAGEMASTER WITH(NOLOCK) ON STAGEMASTER.StageID = LEADDETAILS.StageId
		WHERE  --CAST(LEADDETAILS.CreatedOn AS date) >=CAST(@StartDate AS date) 
		--AND CAST(LEADDETAILS.CreatedOn AS date) <=CAST(@EndDate AS date)AND		 
		((@ProductType IS NULL OR @ProductType = '') OR (INSTYPE.InsuranceTypeId = @ProductType))
		OR ((@Insurertype IS NULL OR @Insurertype = '') OR ( INSURER.InsurerId = @Insurertype))
		OR ((@EnquiryId IS NULL OR @EnquiryId = '') OR ( LEADDETAILS.LeadId = @EnquiryId))

		SELECT @TotalRecord = COUNT(1) FROM #ReportTemp WITH(NOLOCK)
		
		SELECT *,@TotalRecord as TotalRecord FROM #ReportTemp
	
	END TRY                            
BEGIN CATCH              
	DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
	SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
	SET @ErrorDetail=ERROR_MESSAGE()                                        
	EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
END CATCH            
         
END