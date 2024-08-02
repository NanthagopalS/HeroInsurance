-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--[Insurance_GetCKYCFields]'FD3677E5-7938-46C8-9CD2-FAE188A1782C',1
-- =============================================
CREATE PROCEDURE [dbo].[Insurance_GetCKYCDocumentField]  
@InsurerID varchar(50),
@DocumentName varchar(50)
AS
BEGIN 
BEGIN TRY
SELECT Section
	,FieldKey
	,FieldText
	,FieldType
	,IsMandatory
	,Validation
	,IsMaster
	,MasterData,MasterRef,ColumnRef
	FROM Insurance_CKYCDocumentField with(nolock) 
	WHERE InsurerId=@InsurerID AND Section = @DocumentName AND IsActive=1
	END TRY
	BEGIN CATCH         
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
