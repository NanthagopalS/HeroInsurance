

-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,27-Dec-2022>
-- Description:	<Description,Identity_UpdateBUDetails>
-- Identity_UpdateBUDetails 2,true
-- =============================================
 CREATE PROCEDURE [dbo].[Identity_UpdateBUDetails] 
(
	@BUID int,
	@IsActive bit
)
AS
BEGIN
	BEGIN TRY
		
		BEGIN TRANSACTION
				
		BEGIN			
			UPDATE Identity_BU
				SET [IsActive] =  @IsActive
				WHERE BUID= @BUID
			IF @@TRANCOUNT > 0
            COMMIT
       END
		
	END TRY                
	BEGIN CATCH        
		IF @@TRANCOUNT > 0
        ROLLBACK    
		
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
