 --[dbo].[Insurance_GetProposalDetails]  '0788a6e1-240d-47bf-9473-992a1a33b9e5'
CREATE procedure [dbo].[Insurance_GetProposalDetails]  
@TransactionID varchar(50)  
AS
BEGIN 
  BEGIN TRY
	SELECT QT.InsurerId,
		QT.RequestBody,
		QT.ResponseBody,
		QT.CommonResponse ,
		QT.LeadId AS LeadID ,
		QT.QuoteTransactionId AS QuoteTransactionID, 
		CKYC.CKYCStatus,
		LD.VehicleTypeId,
		LD.PolicyTypeId,
		ISNULL(LD.UpdatedBy, LD.CreatedBy) UserId
	FROM Insurance_QuoteTransaction QT WITH(NOLOCK)
	LEFT JOIN Insurance_CKYCTransaction CKYC WITH(NOLOCK) ON QT.LeadId = CKYC.LeadId 
	LEFT JOIN Insurance_LeadDetails LD WITH(NOLOCK) ON LD.LeadId = QT.LeadId
	WHERE TransactionId=@TransactionID AND QT.StageID='ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2'
 END TRY      
 BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH    

END  
  
  