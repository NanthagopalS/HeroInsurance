-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,24-Apr-2023>      
-- Description: <Description, Admin_InsertDeactivatePospDetails>
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_InsertDeactivatePospDetails]       
 (      
	@POSPId VARCHAR(100) = Null,
	@Remark VARCHAR(100) = Null,
	@DocumentId1 VARCHAR(100) = Null,
	@DocumentId2 VARCHAR(100) = Null,
	@Status VARCHAR(100) = 'In Process'
) 
 As
 Begin 

	BEGIN Try
			DECLARE @CodePatternPOSPId varchar(50) = null,
					@FinalCodePOSPId VARCHAR(50) = NULL,
					@DeactivateId VARCHAR(50) = NULL,
					@IndexDeactivateId int =0
		if exists (select 1 from [HeroAdmin].[dbo].[Admin_DeActivatePOSP] WITH(NOLOCK) where DeActivatePospId = @POSPId)
		Update [HeroAdmin].[dbo].[Admin_DeActivatePOSP] 
		Set EmailAttachmentDocumentId = @DocumentId1, BusinessTeamApprovalAttachmentDocumentId = @DocumentId2, Remark = @Remark, Status = @Status, UpdatedOn = GETDATE()
		where DeActivatePospId = @PospId
		else
			BEGIN
				SET @CodePatternPOSPId = (SELECT CodePattern FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH(NOLOCK) WHERE [Code] = 'DEACPOSP' AND IsActive = 1)  
  
				SET @IndexDeactivateId = (SELECT NextValue FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH(NOLOCK) WHERE [Code] = 'DEACPOSP' AND IsActive = 1)  
  
				SET @FinalCodePOSPId = CONCAT(@CodePatternPOSPId, CAST(@IndexDeactivateId AS VARCHAR)) 

				SET @IndexDeactivateId = @IndexDeactivateId + 1  
				
				UPDATE [HeroIdentity].[dbo].[Identity_AutoGenerateId] SET NextValue = @IndexDeactivateId WHERE [Code] = 'DEACPOSP' AND IsActive = 1  
				
				Declare
					@PolicyType VARCHAR(100)
				SET @PolicyType = (Select TOP(1) PRD.ProductName from [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest] IPI          
									 Left Join [HeroIdentity].[dbo].[Identity_InsuranceProductsOfInterest] PRD on PRD.Id = IPI.InsuranceProductsOfInterestId 
									 LEFT JOIn [HeroIdentity].[dbo].[Identity_User] IU on IU.UserId = IPI.UserId
									where IU.POSPId = @POSPId and IU.IsActive = 1 and PRD.ProductName is not null)
				print @PolicyType
				Insert into [HeroAdmin].[dbo].[Admin_DeActivatePOSP] (Id,DeActivatePospId, Remark, EmailAttachmentDocumentId, BusinessTeamApprovalAttachmentDocumentId, Status, PolicyType)
				values (@FinalCodePOSPId,@POSPId, @Remark, @DocumentId1, @DocumentId2, @Status, @PolicyType)
				-- Update [HeroIdentity].[dbo].[Identity_User] Set IsActive = 0 where POSPId = @POSPId  -- commented as we need to deactivate only when we send NOC email (By:-Suraj)
			END

			
	END try
	
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END 