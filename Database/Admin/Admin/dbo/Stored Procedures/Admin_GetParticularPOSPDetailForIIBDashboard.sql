CREATE     PROCEDURE [dbo].[Admin_GetParticularPOSPDetailForIIBDashboard]       
 (      
	@UserId VARCHAR(100)   
 )      
AS      
	
BEGIN      
 BEGIN TRY      
    
	SELECT TOP(1)
		IU.UserId, IU.UserName, IU.MobileNo as MobileNumber, IU.CreatedByMode, IU.POSPId, IU.IIBStatus, IU.IIBUploadStatus, IPan.PanNumber
	FROM [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK)
		INNER JOIN [HeroIdentity].[dbo].[Identity_PanVerification] IPan WITH(NOLOCK) on IPan.UserId = IU.UserId
		INNER JOIN [HeroPOSP].[dbo].[POSP_Exam] PE WITH(NOLOCK) ON PE.UserId = IU.UserId AND PE.IsActive = 1 AND PE.IsCleared = 1
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] PA WITH(NOLOCK) ON PA.UserId = IU.UserId AND PA.IsActive = 1 AND PA.AgreementId IS NOT NULL
	WHERE 
		IU.UserId = @UserId and IPan.IsActive =1
		
 END TRY                      
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END 
