-- =============================================
-- Author: <Author, ANKIT GHOSH>
-- Create date: <Create Date, 31-March-2023>
-- Description:	<Description,[POSP_GetPospLastActivityDetails]>
--[POSP_GetPospLastActivityDetails]
-- =============================================
CREATE    PROCEDURE [dbo].[POSP_GetPospLastActivityDetails]
(      
	@UserId VARCHAR(100) = Null     
 ) 
AS
BEGIN
	BEGIN TRY

		SELECT [LastActivityOn]	FROM [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) where UserId = @UserId
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
	END CATCH
END
