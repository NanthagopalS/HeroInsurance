
-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,27-DEC-2022>
-- Description:	<Description,,Identity_InsertBUDetails>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_InsertBUDetails] 
(
	@Roletypeid int,
	@BULevelID int,
	@BUName varchar(50),
	@IsActive bit ,
	@RoleId varchar(100),
	@CreatedBy varchar(50)
	
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
		INSERT INTO [dbo].[Identity_BU]
           ([RoleTypeID]
           ,[BULevelID]
           ,[BUName]
           ,[IsActive]
		   ,[RoleId]
           ,[CreatedBy]
           )
     VALUES
           (
		     @Roletypeid
			,@BULevelID 
			,@BUName
		    ,@IsActive
			,@RoleId
			,@CreatedBy 	
		   )
	 	IF @@TRANCOUNT > 0
            COMMIT
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
