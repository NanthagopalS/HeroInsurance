-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--[GetProposalDynamicMasterData] 'Insurance_Gender','GenderValue,GenderName'
-- =============================================
CREATE   PROC [dbo].[GetProposalDynamicMasterData](
@TableName VARCHAR(100),
@ColumnName VARCHAR(1000)
)
AS
BEGIN

	DECLARE @GetProposalMasterData  AS TABLE (
        FieldValue VARCHAR(50),
        FieldName VARCHAR(50)
		)

	DECLARE @QUERY NVARCHAR(MAX)

	SET @QUERY = 'SELECT '+ @ColumnName +' FROM ' + @TableName +' WITH(NOLOCK)'
	PRINT @QUERY

	INSERT INTO @GetProposalMasterData
	EXEC sp_executesql @QUERY

	SELECT * FROM @GetProposalMasterData
END