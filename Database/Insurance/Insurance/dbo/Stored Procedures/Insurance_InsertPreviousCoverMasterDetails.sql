-- =============================================
-- Author:		<Author,,Madhu G>
-- Create date: <Create Date,,12-DEC-2023>
-- Description:	<Description,,INSERT PREVIOUSCOVERMASTER DETAIL>
-- =============================================
CREATE PROCEDURE [dbo].[Insurance_InsertPreviousCoverMasterDetails] 
(
    @CoverMasterData dbo.CoverMasterDetailsType READONLY,
	@LeadId VARCHAR(80) = NULL
)
AS
BEGIN
	BEGIN TRY
    -- Check if LeadId already exists
		IF EXISTS (SELECT 1 FROM dbo.Insurance_LeadPreviousCoverDetails WHERE LeadId = @LeadId)
		BEGIN
			-- If LeadId exists, delete all records with that LeadId
			DELETE FROM dbo.Insurance_LeadPreviousCoverDetails WHERE LeadId = @LeadId;
		END

		-- Insert new records
		INSERT INTO dbo.Insurance_LeadPreviousCoverDetails (LeadId, CoverId, IsChecked)
		SELECT @LeadId AS LeadId, CoverId, IsChecked
		FROM @CoverMasterData;
	END TRY
	BEGIN CATCH
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)
		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList
	END CATCH
END