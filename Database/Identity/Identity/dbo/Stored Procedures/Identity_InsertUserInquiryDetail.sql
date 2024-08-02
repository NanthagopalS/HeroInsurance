



-- =============================================
-- Author:		<Author, VISHAL KANJARIYA>
-- Create date: <Create Date,05-DEC-2022>
-- Description:	<Description,,INSERT USER INQUIRY DETAIL>
-- =============================================
CREATE   PROCEDURE [dbo].[Identity_InsertUserInquiryDetail] 
(
	@UserName VARCHAR(100) = NULL,
	@PhoneNumber VARCHAR(10) = NULL,
	@InquiryDescription VARCHAR(1000) = NULL
)
AS
BEGIN
	BEGIN TRY
		
		INSERT INTO Identity_UserInquiry (UserName, PhoneNumber, InquiryDescription)
		VALUES(@UserName, @PhoneNumber, @InquiryDescription)

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
