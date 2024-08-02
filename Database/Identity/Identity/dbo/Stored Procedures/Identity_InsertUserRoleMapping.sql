
-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,29-DEC-2022>
-- Description:	<Description,,Identity_InsertUserRoleMapping> 
-- =============================================
CREATE PROCEDURE [dbo].[Identity_InsertUserRoleMapping] 
(

    @UserID nvarchar(50),
    @RoleID nvarchar(50),
    @ReportingUserID nvarchar(50),
    @CategoryID int,
    @BUID int,
    @RoleTypeID int,
    @IsActive bit
	
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
		INSERT INTO [dbo].[Identity_UserRoleMapping]
           (
		    [UserID]
           ,[RoleID]
           ,[ReportingUserID]
           ,[CategoryID]
           ,[BUID]
           ,[RoleTypeID]
           ,[IsActive])
     VALUES(
           @UserID,
           @RoleID, 
           @ReportingUserID, 
           @CategoryID,
           @BUID, 
           @RoleTypeID,
           @IsActive)
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
