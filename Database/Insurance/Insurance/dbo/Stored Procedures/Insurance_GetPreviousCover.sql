-- =============================================
-- Author:		<Author,,YASH SINGH>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[Insurance_GetPreviousCover]  
(
	@InsurerId VARCHAR(50) = NULL,
	@VehicalTypeId VARCHAR(50)= NULL,
	@PolicyTypeId VARCHAR(100) =NULL
)
AS
BEGIN 
	BEGIN TRY
		

		SELECT CoverId,
		       CoverName,
			   CoverCode,
			   CoverText as Text,
			   CoverFlag as Flag FROM Insurance_GetPreviousCoverMaster 
			   WHERE InsurerId = @InsurerId AND VehicalTypeId = @VehicalTypeId AND PolicyTypeId = @PolicyTypeId
	END TRY   
	
BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END