-- =============================================
-- Author:		<Author,,NANTHA S>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--[dbo].[Insurance_Job_GetBajajBreakinStatus]
-- =============================================

CREATE PROCEDURE [dbo].[Insurance_Job_GetBajajBreakinStatus]
AS
BEGIN 
	BEGIN TRY
		SELECT TOP 10 VehicleNumber,QuoteTransactionID,LeadId
		FROM Insurance_LeadDetails WITH(NOLOCK) 
		WHERE InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37' AND IsBreakin = 1
		AND IsBreakinApproved is NULL 
		AND BreakinId IS NOT NULL order by CreatedOn desc
	END TRY   
	
BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END