

-- =============================================
-- Author:		<Author, AMBI GUPTA>
-- Create date: <Create Date,29-NOV-2022>
-- Description:	<Description,,INSERT USER DETAIL>
--Identity_InsertUser 'ambi.gupta','a.a@a.com','9987848972'
-- =============================================
CREATE PROCEDURE [dbo].[Identity_InsertBenefitsDetail] 
(
@BenefitsTitle varchar(200),
@BenefitsDescription varchar(200) = NULL,
@IsActive bit = NULL
)
AS
BEGIN
	BEGIN TRY
		IF EXISTS(SELECT TOP 1 BenefitsTitle FROM Identity_BenefitsDetail WITH(NOLOCK) WHERE BenefitsTitle = @BenefitsTitle)
		BEGIN
			SELECT Id, 1 FROM Identity_BenefitsDetail WITH(NOLOCK) WHERE BenefitsTitle = @BenefitsTitle
		END
		ELSE
		BEGIN

			INSERT INTO [dbo].[Identity_BenefitsDetail] (BenefitsTitle, BenefitsDescription, IsActive)
			VALUES(@BenefitsTitle, @BenefitsDescription, @IsActive)

			SELECT Id, 1 FROM Identity_BenefitsDetail WITH(NOLOCK) WHERE BenefitsTitle = @BenefitsTitle
		END

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
