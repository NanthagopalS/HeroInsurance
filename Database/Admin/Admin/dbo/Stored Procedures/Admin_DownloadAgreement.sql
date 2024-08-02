-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date, 11-May-2023>      
-- Description: <Description, Admin_DownloadAgreement>
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_DownloadAgreement]       
 (      
	@POSPId VARCHAR(100) = Null	
 ) 
 As
 Begin  
	 
	BEGIN TRY

		Select PA.PreSignedAgreementId as DocumentId from [HeroPOSP].[dbo].[POSP_Agreement] PA WITH(NOLOCK)
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK) on IU.UserId = PA.UserId
		where IU.POSPId = @POSPId

	END TRY
	
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END