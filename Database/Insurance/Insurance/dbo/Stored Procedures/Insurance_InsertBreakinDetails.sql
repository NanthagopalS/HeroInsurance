-- =============================================  
-- Author:  <Firoz S>  
-- Create date: <24-08-2023>  
-- Description: <Created to insert breakin details in leaddetails during proposal creation> 
-- =============================================  
CREATE PROCEDURE [dbo].[Insurance_InsertBreakinDetails]     
@LeadId VARCHAR(50) = NULL,
@IsBreakIn BIT = NULL,
@PolicyNumber VARCHAR(50) = NULL,
@BreakinId VARCHAR(50) = NULL,
@BreakinInspectionURL NVARCHAR(MAX) = NULL,
@BreakInInspectionAgency VARCHAR(50) = NULL,
@UserId VARCHAR(50) = NULL
AS      
BEGIN      
	BEGIN TRY      
		
		UPDATE Insurance_LeadDetails SET StageId='405F4696-CDFB-4065-B364-9410B56BC78D', IsBreakin = @IsBreakIn,
		BreakinId = @BreakinId, BreakinInspectionDate = CONVERT(VARCHAR(50),GETDATE(),121),  UpdatedBy=@UserId, 
		UpdatedOn = GETDATE(), BreakinInspectionURL = @BreakinInspectionURL, InspectionAgency = @BreakInInspectionAgency
		WHERE LeadId=@LeadId  

		SELECT * FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadId

	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END