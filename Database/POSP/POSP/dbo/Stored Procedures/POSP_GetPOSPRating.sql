

CREATE PROCEDURE [dbo].[POSP_GetPOSPRating] 
(
	@UserId VARCHAR(50)
)
AS
BEGIN
	Declare @Exists Int
 BEGIN TRY        
	If Exists (SELECT Id, UserId, Rating, Description, IsActive, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn
	 FROM [dbo].[POSP_Rating] WITH(NOLOCK) WHERE UserId = @UserId AND IsActive = 1)
	 Begin
		Set @Exists = 1
	End
	Else
	Begin
		Set @Exists = 0
	End
	

	SELECT @Exists as IsPOSPRatingAvailable

  END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END
