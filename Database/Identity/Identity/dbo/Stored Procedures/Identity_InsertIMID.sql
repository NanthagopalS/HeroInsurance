
-- EXEC  [dbo].[Identity_InsertIMID] 'IM-1303316','POS/HERO/10009'

-- =============================================
-- Author:		<Author,,Nanthagopal >
-- Create date: <Create Date,,17/04/2023>
-- Description:	<Description,GetPOSPDetails>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_InsertIMID]
@IMID VARCHAR(100) = NULL,
@POSPId VARCHAR(100) = NULL
AS
BEGIN
   BEGIN TRY  
		IF(@IMID IS NOT NULL)
			BEGIN
				UPDATE Identity_User SET IMID = @IMID, UpdatedOn = GETDATE() WHERE POSPId = @POSPId
			END
		SELECT IMID FROM Identity_User WITH(NOLOCK) WHERE POSPId = @POSPId
   END TRY  
   BEGIN CATCH  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
