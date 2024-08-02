-- =============================================  
-- Author:  <Author, Vishal>  
-- Create date: <Create Date,13-Mar-2023>  
-- Description: <Description,Admin_UpdateAgreementStatusByUserId>   
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_UpdateAgreementStatusByUserId]   
(  
 @UserId varchar(100),
 @ProcessType varchar(100)
)  
AS  
BEGIN  
 BEGIN TRY  

	DECLARE @StampId VARCHAR(100) = NULL


	IF(@ProcessType = 'Revoke')
	BEGIN

		SET @StampId = (SELECT TOP(1) StampId FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH(NOLOCK) WHERE UserId = @UserId AND IsActive = 1)

		UPDATE [HeroAdmin].[dbo].[Admin_StampData] SET StampStatus = 'Available', CreatedOn= GETDATE() WHERE Id = @StampId

		--DELETE FROM [HeroPOSP].[dbo].[POSP_Agreement] WHERE UserId = @UserId AND StampId = @StampId

		UPDATE [HeroPOSP].[dbo].[POSP_Agreement] SET IsRevoked = 1, IsActive = 0, CreatedOn= GETDATE() WHERE UserId = @UserId AND StampId = @StampId

	END
	ELSE IF (@ProcessType = 'Reinitiate')
	BEGIN

		---Remove Revoked Records...
		DELETE FROM [HeroPOSP].[dbo].[POSP_Agreement] WHERE UserId = @UserId 
		--AND StampId = @StampId

		--Reinitiate Step...

		SET @StampId = (SELECT TOP (1) Id FROM [HeroAdmin].[dbo].[Admin_StampData] WITH(NOLOCK) WHERE StampStatus = 'Available' ORDER BY SrNo)

		UPDATE [HeroAdmin].[dbo].[Admin_StampData] SET StampStatus = 'Blocked', CreatedOn = GETDATE() WHERE Id = @StampId
	
		---POSP Agreement Initiate
		INSERT INTO [HeroPOSP].[dbo].[POSP_Agreement] (UserId, StampId) 
		VALUES (@UserId, @StampId)

	END
    
 END TRY                  
 BEGIN CATCH         
       
    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END
