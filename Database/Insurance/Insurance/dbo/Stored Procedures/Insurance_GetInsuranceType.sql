-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,25-Nov-2022>
-- Description:	<Description,[Insurance_GetInsuranceType]>
--[Insurance_GetInsuranceType] 'MOTOR'
-- =============================================
CREATE     PROCEDURE [dbo].[Insurance_GetInsuranceType]
(
	@InsuranceType VARCHAR(20)=NULL
)
AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		SELECT CAST(InsuranceTypeId AS VARCHAR(50))InsuranceTypeId,
		InsuranceName,InsuranceType,IsOfferApplicable,OfferContent
		FROM  Insurance_InsuranceType MAKE WITH(NOLOCK)
		WHERE IsActive=1 AND InsuranceType = ISNULL(@InsuranceType,'')
		ORDER BY InsuranceName

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
