
--[Insurance_SaveUpdateProposal] 'test1','test'  
CREATE PROCEDURE [dbo].[Insurance_SaveCKYCDetails]  
@QuoteTransactionID VARCHAR(50) = NULL,  
@RequestBody VARCHAR(max) = NULL,
@ResponseBody VARCHAR(max) = NULL,
@InsurerId VARCHAR(50) = NULL,
@LeadName VARCHAR(50) = NULL,
@PhoneNumber VARCHAR(50) = NULL,
@Email VARCHAR(50) = NULL,
@CompanyName VARCHAR(50) = NULL,
@DOI VARCHAR(50) = NULL,
@GSTNo VARCHAR(50) = NULL,
@MiddleName VARCHAR(50) = NULL,
@LastName VARCHAR(50) = NULL,
@DOB VARCHAR(50) = NULL,
@Gender VARCHAR(50) = NULL,
@PANNumber VARCHAR(50) = NULL,
@AadhaarNumber VARCHAR(50) = NULL,
@PhotoId VARCHAR(max) = NULL,
@AddressType VARCHAR(50) = NULL,
@Address1 VARCHAR(50) = NULL,
@Address2 VARCHAR(50) = NULL,
@Address3 VARCHAR(50) = NULL,
@Pincode VARCHAR(50) = NULL,
@Stage VARCHAR(3) = NULL,
@CKYCNumber VARCHAR(50) = NULL,
@KYCId VARCHAR(50) = NULL,
@CKYCStatus VARCHAR(50) = NULL,
@UserId VARCHAR(50) = NULL,
@City VARCHAR(100) = NULL,
@State VARCHAR(100) = NULL,
@Salutation VARCHAR(100) = NULL
AS  
BEGIN  
  
 DECLARE @LeadID varchar(50)

 begin try
	
	 SELECT @LeadID = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionID = @QuoteTransactionID  
	 SELECT @CKYCStatus = CASE WHEN @CKYCStatus='approved' THEN 'Success' ELSE @CKYCStatus END
	 

	MERGE INTO Insurance_CKYCTransaction AS tg  
		USING(  
			SELECT @LeadID  LeadId,        
			@QuoteTransactionID QuoteTransactionId,
			@InsurerId InsurerId,
			@KYCId KYCId,
			@CKYCNumber CKYCNumber,
			@CKYCStatus CKYCStatus,
			@Stage Stage
		) AS Src  
		ON  tg.LeadID=Src.LeadId AND tg.QuoteTransactionId=Src.QuoteTransactionId AND tg.InsurerId=Src.InsurerId AND tg.Stage = Src.Stage
		WHEN MATCHED THEN UPDATE SET        
		  tg.CKYCStatus = Src.CKYCStatus,
		  tg.KYCId = Src.KYCId,
		  tg.CKYCNumber = Src.CKYCNumber,
		  tg.UpdatedOn = getdate(),  
		  tg.UpdatedBy = @UserId
		WHEN NOT MATCHED THEN       
		INSERT (InsurerId,QuoteTransactionId,CKYCRequestBody,CKYCResponseBody,CreatedBy,CreatedOn,LeadId,PhotoId,Stage,KYCId,CKYCNumber,CKYCStatus)  
		VALUES(@InsurerId,@QuoteTransactionID,@RequestBody,@ResponseBody,@UserId,GETDATE(),@LeadID,@PhotoId,@Stage,@KYCId,@CKYCNumber,@CKYCStatus);
	
	IF(@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621') -- For Godigit only DOB and PAN has to updated(KYC after payment)
	BEGIN
		UPDATE Insurance_LeadDetails SET 
		DOB = @DOB,
		PANNumber = @PANNumber,
		QuoteTransactionID = @QuoteTransactionID,
		StageId = '4F0E30EE-1310-4A89-B2FD-C4314AB6CEBA',
		UpdatedBy = @UserId,
		UpdatedOn = GETDATE(),
		InsurerId = @InsurerId
		WHERE LeadId = @LeadID
	END
	ELSE IF(@InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C') -- For TATA only PAN,AADHAR,DOB and Gender has to updated(KYC after payment)
	BEGIN
		UPDATE Insurance_LeadDetails SET 
		DOB = @DOB,
		PANNumber = @PANNumber,
		AadharNumber = @AadhaarNumber,
		Gender = CASE WHEN @Gender ='M' THEN 'Male' WHEN @Gender='F' THEN 'Female' ELSE @Gender END,
		QuoteTransactionID = @QuoteTransactionID,
		StageId = '4F0E30EE-1310-4A89-B2FD-C4314AB6CEBA',
		UpdatedBy = @UserId,
		UpdatedOn = GETDATE(),
		InsurerId = @InsurerId
		WHERE LeadId = @LeadID
	END
	ELSE IF(@InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9') -- For Oriental 
	BEGIN
		UPDATE Insurance_LeadDetails SET 
		PANNumber = @PANNumber,
		QuoteTransactionID = @QuoteTransactionID,
		UpdatedBy = @UserId,
		UpdatedOn = GETDATE(),
		InsurerId = @InsurerId
		WHERE LeadId = @LeadID
	END
	ELSE IF(@InsurerId = 'DC874A12-6667-41AB-A7A1-3BB832B59CEB') -- For UIIC
	BEGIN
		UPDATE Insurance_LeadDetails SET LeadName = @LeadName,
		--PhoneNumber = @PhoneNumber, 
		--Email = @Email,
		MiddleName = @MiddleName,
		LastName = @LastName,
		DOB = @DOB,
		Gender = CASE WHEN @Gender ='M' THEN 'MALE' WHEN @Gender='F' THEN 'FEMALE' ELSE @Gender END,
		PANNumber = @PANNumber,
		AadharNumber = @AadhaarNumber,
		--StageId = '4F0E30EE-1310-4A89-B2FD-C4314AB6CEBA',
		UpdatedBy = @UserId,
		UpdatedOn = GETDATE(),
		InsurerId = @InsurerId,
		Salutation = @Salutation
		WHERE LeadId = @LeadID
	END
	ELSE
	BEGIN
		UPDATE Insurance_LeadDetails SET LeadName = @LeadName,
		PhoneNumber = @PhoneNumber, 
		Email = @Email,
		MiddleName = @MiddleName,
		LastName = @LastName,
		DOB = @DOB,
		Gender = CASE WHEN @Gender ='M' THEN 'MALE' WHEN @Gender='F' THEN 'FEMALE' ELSE @Gender END,
		PANNumber = @PANNumber,
		AadharNumber = @AadhaarNumber,
		QuoteTransactionID = @QuoteTransactionID,
		StageId = '4F0E30EE-1310-4A89-B2FD-C4314AB6CEBA',
		UpdatedBy = @UserId,
		UpdatedOn = GETDATE(),
		InsurerId = @InsurerId,
		Salutation = @Salutation
		WHERE LeadId = @LeadID
	END
	
	MERGE INTO Insurance_LeadAddressDetails AS tg  
		USING(  
			SELECT @LeadID  LeadId,        
			@AddressType AddressType,
			@Address1 Address1,
			@Address2 Address2,
			@Address3 Address3,
			@Pincode Pincode,
			@City City,
			@State State
		) AS Src  
		ON  tg.LeadID=Src.LeadId
		WHEN MATCHED THEN UPDATE SET        
		  tg.AddressType = Src.AddressType,
		  tg.Address1= Src.Address1,  
		  tg.Address2 = Src.Address2,
		  tg.Address3 = Src.Address3,
		  tg.Pincode = Src.Pincode,
		  tg.UpdatedOn = getdate(),  
		  tg.UpdatedBy = @UserId,
		  tg.City = Src.City,
		  tg.State = Src.State
		WHEN NOT MATCHED THEN       
		INSERT (LeadID,AddressType,Address1,Address2,Address3,Pincode,CreatedBy,CreatedOn)
		VALUES(@LeadID,@AddressType,@Address1,@Address2,@Address3,@Pincode,@UserId,getdate());
		

	SELECT @LeadID LeadID,
			@KYCId KYCId,
			@CKYCNumber CKYCNumber

end try
begin catch
	SELECT '' LeadID  

	 DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
	 SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
	 SET @ErrorDetail=ERROR_MESSAGE()                                    
	 EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                

end catch
END  
