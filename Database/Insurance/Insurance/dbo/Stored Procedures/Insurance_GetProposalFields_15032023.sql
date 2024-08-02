-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--[Insurance_GetProposalFields]'78190CB2-B325-4764-9BD9-5B9806E99621'
-- =============================================

--exec  [dbo].[Insurance_GetProposalFields] '16413879-6316-4C1E-93A4-FF8318B14D37', '9D9F5D7A-601E-489B-B60D-F1719DD94053'
CREATE procedure [dbo].[Insurance_GetProposalFields_15032023]  
@InsurerID VARCHAR(50),
@QuotetransactionId VARCHAR(50)

AS
BEGIN 
BEGIN TRY
	DECLARE 
	@LeadName VARCHAR(50), 
	@MiddleName VARCHAR(50),
	@LastName VARCHAR(50),
	@PhoneNumber VARCHAR(50),
	@Email VARCHAR(50), 
	@Gender VARCHAR(50),
	@DOB VARCHAR(50),
	@PANNumber VARCHAR(50),
	@AadharNumber VARCHAR(50),
	@LeadID VARCHAR(50),
	@AddressType VARCHAR(100),
	@Address1 NVARCHAR(MAX),
	@Address2 NVARCHAR(MAX),
	@Address3 NVARCHAR(MAX),
	@Pincode VARCHAR(50)

	SELECT Section
	,FieldKey
	,FieldText
	,FieldType
	,IsMandatory
	,Validation
	,IsMaster
	,MasterData,MasterRef,ColumnRef
	FROM Insurance_ProposalField with(nolock) 
	WHERE InsurerId=@InsurerID  and IsActive=1

	SELECT @LeadID = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionID = @QuoteTransactionID 
	
	SELECT @LeadName = LeadName,
	@MiddleName = MiddleName,
	@LastName = LastName,
	@PhoneNumber = PhoneNumber,
	@Email = Email,
	@Gender = Gender,
	@DOB = DOB,
	@PANNumber = PANNumber,
	@AadharNumber = AadharNumber 
	FROM Insurance_LeadDetails  WITH(NOLOCK) 
	WHERE LeadId = @LeadID

	SELECT @AddressType = AddressType, @Address1 = Address1, @Address1 = Address2, @Address1 = Address3, @Pincode = Pincode
	FROM Insurance_LeadAddressDetails WITH(NOLOCK) 
	WHERE LeadId = @LeadID

	SELECT @LeadName AS LeadName, 
	@MiddleName AS MiddleName,
	@LastName AS LastName,
	@PhoneNumber AS PhoneNumber,
	@Email AS Email, 
	@Gender AS Gender,
	@DOB AS DOB,
	@PANNumber AS PANNumber,
	@AadharNumber AS AadharNumber

	SELECT @AddressType AS AddressType,
	@Address1 AS Address1,
	@Address2 AS Address2,
	@Address3 AS Address3,
	@Pincode AS Pincode 

	 END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END