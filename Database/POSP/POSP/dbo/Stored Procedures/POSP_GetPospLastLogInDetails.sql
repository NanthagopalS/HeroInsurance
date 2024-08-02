-- =============================================
-- Author: <Author, ANKIT GHOSH>
-- Create date: <Create Date, 31-March-2023>
-- Description:	<Description,[POSP_GetPospLastLogInDetails]>
--[POSP_GetPospLastLogInDetails]
-- =============================================
CREATE    PROCEDURE [dbo].[POSP_GetPospLastLogInDetails]
(      
	@UserId VARCHAR(100) = Null     
 ) 
AS
BEGIN
	BEGIN TRY

		SELECT TOP (1) [LogInDateTime] as LastLogIn	FROM [HeroIdentity].[dbo].[Identity_UserLog] WITH(NOLOCK)
		where UserId = @UserId and IsActive = 0 order by [LogInDateTime] desc
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
	END CATCH
END
