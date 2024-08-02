-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,,25-NOV-2022>
-- Description:	<Description,[GetNCB]>
--[Insurance_GetNCB] '2022-06-10',1
-- =============================================
CREATE     PROCEDURE [dbo].[Insurance_GetNCB]
(
	@PolicyExpiryDate VARCHAR(12)=NULL,
	@IsPreviousPolicy BIT=NULL
)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		IF(@IsPreviousPolicy =0)
		BEGIN
			SELECT CAST(NCBId AS VARCHAR(50))NCBId,
			(CAST(NCBValue AS VARCHAR(10))+'%') NCBName,
			NCBValue , 1 AS IsBreakinRequired
			FROM Insurance_NCB WITH(NOLOCK) 
			WHERE IsActive=1 AND NCBValue=0 
			ORDER BY NCBValue
		END
		ELSE
		BEGIN
			IF(DATEDIFF(DAY, CAST(@PolicyExpiryDate AS DATE),CAST(GETDATE() AS DATE)) > 90)
			BEGIN
				SELECT CAST(NCBId AS VARCHAR(50))NCBId,
				(CAST(NCBValue AS VARCHAR(10))+'%') NCBName,
				NCBValue , 1 AS IsBreakinRequired
				FROM Insurance_NCB WITH(NOLOCK) 
				WHERE IsActive=1 AND NCBValue=0 
				ORDER BY NCBValue
			END
			ELSE
			BEGIN
				SELECT CAST(NCBId AS VARCHAR(50))NCBId,
				(CAST(NCBValue AS VARCHAR(10))+'%') NCBName,
				NCBValue , 0 AS IsBreakinRequired
				FROM Insurance_NCB WITH(NOLOCK) 
				WHERE IsActive=1 ORDER BY NCBValue
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
