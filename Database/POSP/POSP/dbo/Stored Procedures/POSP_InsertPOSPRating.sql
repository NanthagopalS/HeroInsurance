

CREATE PROCEDURE [dbo].[POSP_InsertPOSPRating] 
(
@UserId varchar(50),
@Rating int,
@Description varchar(200)
)
AS
BEGIN
	BEGIN TRY
	BEGIN
			INSERT INTO POSP_Rating (UserId, Rating, Description)
			VALUES(@UserId, @Rating, @Description)
	END
	BEGIN
		SELECT UserId, Rating, Description
		FROM [dbo].[POSP_Rating] WITH(NOLOCK) WHERE UserId = @UserId
		
	END
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
