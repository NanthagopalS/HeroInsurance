-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--[Insurance_GetProposalFields]'78190CB2-B325-4764-9BD9-5B9806E99621','49A95CB8-6AD3-441D-AD74-7762C5FD45F6'
-- =============================================

-- exec  [dbo].[Insurance_GetProposalFields_mapping] '16413879-6316-4C1E-93A4-FF8318B14D37', '3136CE5C-E055-4E6D-89ED-8B7E61118486'
CREATE procedure [dbo].[Insurance_GetProposalFields]  
@InsurerID VARCHAR(50),
@QuotetransactionId VARCHAR(50)

AS
BEGIN 
BEGIN TRY
	DECLARE 
	@LeadID VARCHAR(50), @CustomerType VARCHAR(20)
	
	SELECT @LeadID = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionID = @QuoteTransactionID 
	SELECT @CustomerType = CarOwnedBy FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId=@LeadID

	IF(@CustomerType='INDIVIDUAL')
	BEGIN
		SELECT Section,
		FieldKey,
		FieldText,
		FieldType,
		IsMandatory,
		Validation,
		IsMaster,
		MasterData,
		MasterRef,
		ColumnRef,
		[dbo].[Insurance_ProposalFieldMapper] (DBKey,@LeadID) AS DefaultValue
		FROM Insurance_ProposalField with(nolock) 
		WHERE InsurerId=@InsurerID  and IsActive=1 and FieldKey not in('companyName','gstno','dateOfIncorporation')
		ORDER BY FieldOrder 
	END
	ELSE IF(@CustomerType='COMPANY' and @InsurerID='FD3677E5-7938-46C8-9CD2-FAE188A1782C')
	BEGIN
		SELECT Section,
		FieldKey,
		FieldText,
		FieldType,
		IsMandatory,
		Validation,
		IsMaster,
		MasterData,
		MasterRef,
		ColumnRef,
		[dbo].[Insurance_ProposalFieldMapper] (DBKey,@LeadID) AS DefaultValue
		FROM Insurance_ProposalField with(nolock) 
		WHERE InsurerId=@InsurerID  and IsActive=1 and FieldKey not in('firstName','lastName','dateOfBirth','gender','occupation','maritalStatus','Salutation') 
		ORDER BY FieldOrder 
	END
	ELSE IF(@CustomerType='COMPANY')
	BEGIN
		SELECT Section,
		FieldKey,
		FieldText,
		FieldType,
		IsMandatory,
		Validation,
		IsMaster,
		MasterData,
		MasterRef,
		ColumnRef,
		[dbo].[Insurance_ProposalFieldMapper] (DBKey,@LeadID) AS DefaultValue
		FROM Insurance_ProposalField with(nolock) 
		WHERE InsurerId=@InsurerID  and IsActive=1 and FieldKey not in('firstName','lastName','dateOfBirth','customerName','gender','occupation','maritalStatus','Salutation') 
		ORDER BY FieldOrder 
	END
	 END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END