-- =============================================        
-- Author:  <Author, Parth>        
-- Create date: <Create Date,9-Mar-2022>        
-- Description: <Description,[Admin_GetStampCountForAgreementManagement]>        
--[Admin_RoleType]        
-- =============================================        
CREATE PROCEDURE [dbo].[Admin_GetStampCountForAgreementManagement]        
AS        
BEGIN        
 BEGIN TRY

	DECLARE 
		@TotalStampCount int = 0,
		@TotalUsedStamp int = 0,
		@TotalBlockedStamp int = 0,
		@TotalAvailableStamp int = 0
	
	SET @TotalStampCount = (SELECT COUNT(Id) as CountValue FROM [dbo].[Admin_StampData] WITH(NOLOCK) WHERE IsActive = 1)

	SET @TotalUsedStamp = (
		SELECT COUNT(Id) as CountValue FROM [dbo].[Admin_StampData] WITH(NOLOCK) WHERE IsActive = 1 AND StampStatus = 'Used')

	SET @TotalBlockedStamp = (
		SELECT COUNT(Id) as CountValue FROM [dbo].[Admin_StampData] WITH(NOLOCK) WHERE IsActive = 1 AND StampStatus = 'Blocked')

	SET @TotalAvailableStamp = (
		SELECT COUNT(Id) as CountValue FROM [dbo].[Admin_StampData] WITH(NOLOCK) WHERE IsActive = 1 AND StampStatus = 'Available')


	SELECT @TotalStampCount as TotalStampCount, @TotalUsedStamp as TotalUsedStamp,
		   @TotalBlockedStamp as TotalBlockedStamp, @TotalAvailableStamp as TotalAvailableStamp
         
 END TRY                        
 BEGIN CATCH        
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END       
