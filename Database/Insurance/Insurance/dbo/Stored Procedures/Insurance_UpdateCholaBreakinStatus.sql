CREATE PROCEDURE [dbo].[Insurance_UpdateCholaBreakinStatus]     
@IsBreakinApproved BIT = NULL,
@BreakinId VARCHAR(50) = NULL,
@Stage VARCHAR(50) = NULL,
@InspectionDate VARCHAR(50) = NULL,
@InspectionAgency VARCHAR(50) = NULL
AS      
BEGIN      
	BEGIN TRY      
		BEGIN
				UPDATE Insurance_LeadDetails 
				SET IsBreakinApproved = @IsBreakinApproved, 
				StageId = (SELECT StageId FROM Insurance_StageMaster WHERE Stage = @Stage),
				BreakinInspectionDate = @InspectionDate,
				InspectionAgency = @InspectionAgency, UpdatedOn = GETDATE()
				WHERE BreakinId = @BreakinId
		END
	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END