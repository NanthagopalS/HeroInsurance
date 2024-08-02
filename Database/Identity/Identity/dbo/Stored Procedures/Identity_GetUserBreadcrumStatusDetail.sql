
CREATE PROCEDURE [dbo].[Identity_GetUserBreadcrumStatusDetail]
(
	@UserId VARCHAR(100) = NULL
)
AS
BEGIN
	BEGIN TRY

		SELECT ROW_NUMBER() OVER(ORDER BY PriorityIndex) AS Number, StatusName, StatusValue, StatusId from 
		(
			SELECT DISTINCT BM.PriorityIndex, BM.StatusName, BM.StatusValue, UB.StatusId
			FROM Identity_UserBreadcrumStatusDetail UB WITH(NOLOCK)
			INNER JOIN Identity_UserBreadcrumStatusMaster BM WITH(NOLOCK) ON UB.StatusId = BM.Id
			WHERE UB.UserId = @UserId			
		)  t
	
	END TRY                
	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
