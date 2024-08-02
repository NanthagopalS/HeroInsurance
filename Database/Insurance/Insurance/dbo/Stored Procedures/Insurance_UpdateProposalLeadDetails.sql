    
    
CREATE PROCEDURE [dbo].[Insurance_UpdateProposalLeadDetails]      
@QuoteTransactionID VARCHAR(50) = NULL,    
@InsurerId VARCHAR(50) = NULL,    
@FirstName VARCHAR(50) = NULL,    
@MiddleName VARCHAR(50) = NULL,    
@LastName VARCHAR(50) = NULL,    
@PhoneNumber VARCHAR(50) = NULL,    
@Email VARCHAR(50) = NULL,    
@CompanyName VARCHAR(50) = NULL,    
@GSTNumber VARCHAR(50) = NULL,    
@DOB VARCHAR(50) = NULL,    
@Gender VARCHAR(50) = NULL,    
@NomineeFirstName VARCHAR(50) = NULL,    
@NomineeLastName VARCHAR(50) = NULL,    
@NomineeDOB VARCHAR(50) = NULL,    
@NomineeAge int = NULL,    
@NomineeRelation VARCHAR(50) = NULL,    
@AddressType VARCHAR(50) = NULL,    
@Perm_Address1 VARCHAR(200) = NULL,    
@Perm_Address2 VARCHAR(200) = NULL,    
@Perm_Address3 VARCHAR(200) = NULL,    
@Perm_Pincode VARCHAR(8) = NULL,    
@Perm_City VARCHAR(100) = NULL,    
@Perm_State VARCHAR(100) = NULL,    
@IsFinancier VARCHAR(10) = NULL,    
@FinancierName VARCHAR(50) = NULL,    
@FinancierBranch VARCHAR(50) = NULL,    
@EngineNumber VARCHAR(50) = NULL,    
@ChassisNumber VARCHAR(50) = NULL,    
@UserId VARCHAR(50) = NULL,    
@IsProposal BIT,  
@PanNumber VARCHAR(20) = NULL  
AS      
BEGIN      
      
 DECLARE @LeadID Varchar(50), @CustomerType Varchar(20), @StageId Varchar(50)    
 BEGIN TRY    
  IF(@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621')    
  BEGIN    
   SELECT @Perm_State= HSM.StateName FROM MOTOR.GoDigit_StateMaster GSM WITH(NOLOCK) INNER JOIN Insurance_State HSM WITH(NOLOCK) ON HSM.StateId = GSM.StateId WHERE GSM.StateCode=@Perm_State    
  END    
  ELSE IF(@InsurerId ='FD3677E5-7938-46C8-9CD2-FAE188A1782C')    
  BEGIN    
   SELECT @Perm_City = TXT_CITYDISTRICT  FROM MOTOR.ICICI_City_Master WITH(NOLOCK) WHERE IL_CITYDISTRICT_CD=@Perm_City    
   SELECT @Perm_State = ILStateName  FROM MOTOR.ICICI_State WITH(NOLOCK) WHERE ILState = @Perm_State    
  END    
  ELSE IF(@InsurerId ='0A326B77-AFD5-44DA-9871-1742624CFF16')    
  BEGIN    
   SELECT @Perm_City = TXT_CITYDISTRICT  FROM MOTOR.HDFC_CityDistrict WITH(NOLOCK) WHERE NUM_CITYDISTRICT_CD = @Perm_City    
   SELECT @Perm_State = TXT_STATE FROM MOTOR.HDFC_State WITH(NOLOCK) WHERE NUM_STATE_CD = @Perm_State    
  END    
    
  SELECT @LeadID = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionID = @QuoteTransactionID      
  SELECT @CustomerType =CarOwnedBy FROM Insurance_LeadDetails WHERE LeadId=@LeadID    
      
  IF(@IsProposal = 1)    
  BEGIN    
   SET @StageId = 'CD0D06FB-3AFC-4AF2-9FE6-3DBF98F0A105'    
  END    
  ELSE    
     BEGIN    
   SET @StageId = '4F0E30EE-1310-4A89-B2FD-C4314AB6CEBA'    
  END    
    
    
  IF(@CustomerType='INDIVIDUAL')    
  BEGIN    
  
   IF(@InsurerId ='85F8472D-8255-4E80-B34A-61DB8678135C')  
   BEGIN  
  -- Need to store PAN as well since kyc will come after proposal  
  UPDATE Insurance_LeadDetails SET PANNumber = @PanNumber WHERE LeadId = @LeadID   
   END  
  
   UPDATE Insurance_LeadDetails SET LeadName = @FirstName,    
   MiddleName = @MiddleName,    
   LastName = @LastName,    
   PhoneNumber = @PhoneNumber,     
   Email = @Email,    
   DOB = @DOB,    
   Gender = @Gender,    
   QuoteTransactionID = @QuoteTransactionID,    
   EngineNumber = @EngineNumber,    
   ChassisNumber = @ChassisNumber,    
   IsCarOnLoan = CASE WHEN @IsFinancier='Yes' THEN 1 WHEN @IsFinancier='No' THEN 0 END,    
   LoadProvidedCompany = @FinancierName,    
   LoadProvidedCity = @FinancierBranch,    
   StageId = @StageId,    
   UpdatedBy = @UserId,    
   UpdatedOn = GETDATE()    
   WHERE LeadId = @LeadID    
    
  END    
  ELSE IF(@CustomerType='COMPANY')    
  BEGIN    
	IF(@InsurerId ='85F8472D-8255-4E80-B34A-61DB8678135C')  
	BEGIN  
	  -- Need to store PAN as well since kyc will come after proposal  
	  UPDATE Insurance_LeadDetails SET PANNumber = @PanNumber WHERE LeadId = @LeadID   
	END

   UPDATE Insurance_LeadDetails SET CompanyName=@CompanyName,    
   GSTNo=@GSTNumber,    
   PhoneNumber = @PhoneNumber,     
   Email = @Email,    
   QuoteTransactionID = @QuoteTransactionID,    
   EngineNumber = @EngineNumber,    
   ChassisNumber = @ChassisNumber,    
   StageId = @StageId,    
   UpdatedBy = @UserId,    
   UpdatedOn = GETDATE()    
   WHERE LeadId = @LeadID    
  END    
      
  MERGE INTO Insurance_LeadAddressDetails AS LeadAdd      
   USING(      
   SELECT @LeadID  LeadId,            
   @AddressType AddressType,    
   @Perm_Address1 Perm_Address1,    
   @Perm_Address2 Perm_Address2,    
   @Perm_Address3 Perm_Address3,    
   @Perm_Pincode Perm_Pincode,    
   @Perm_City Perm_City,    
   @Perm_State Perm_State    
   ) AS Src      
   ON  LeadAdd.LeadID=Src.LeadId    
   WHEN MATCHED THEN UPDATE SET    
   LeadAdd.AddressType = @AddressType,    
   LeadAdd.Address1 = @Perm_Address1,    
   LeadAdd.Address2 = @Perm_Address2,    
   LeadAdd.Address3 = @Perm_Address3,    
   LeadAdd.Pincode = @Perm_Pincode,    
   LeadAdd.City = @Perm_City,    
   LeadAdd.State = @Perm_State,    
   LeadAdd.Country = 'INDIA',    
   LeadAdd.UpdatedOn = getdate(),      
   LeadAdd.UpdatedBy = @UserId    
   WHEN NOT MATCHED THEN           
   INSERT (LeadID,AddressType,Address1,Address2,Address3,Pincode,City,State,Country,CreatedBy,CreatedOn)      
   VALUES(@LeadID,@AddressType,@Perm_Address1,@Perm_Address2,@Perm_Address3,@Perm_Pincode,@Perm_City,@Perm_State,'INDIA',@UserId,GETDATE());    
    
  MERGE INTO Insurance_LeadNomineeDetails AS LeadNom    
   USING(      
   SELECT @LeadID  LeadId,            
   @NomineeFirstName NomineeFirstName,    
   @NomineeLastName NomineeLastName,    
   @NomineeDOB NomineeDOB,    
   @NomineeAge NomineeAge,    
   @NomineeRelation NomineeRelation    
   ) AS Src      
   ON  LeadNom.LeadID=Src.LeadId    
   WHEN MATCHED THEN UPDATE SET    
   LeadNom.FirstName = @NomineeFirstName,    
   LeadNom.LastName = @NomineeLastName,    
   LeadNom.DOB = @NomineeDOB,    
   LeadNom.Age = @NomineeAge,    
   LeadNom.Relationship = @NomineeRelation,    
   LeadNom.UpdatedOn = getdate(),      
   LeadNom.UpdatedBy = @UserId    
   WHEN NOT MATCHED THEN           
   INSERT (LeadID,FirstName,LastName,DOB,Age,Relationship,CreatedBy,CreatedOn)      
   VALUES(@LeadID,@NomineeFirstName,@NomineeLastName,@NomineeDOB,@NomineeAge,@NomineeRelation,@UserId,GETDATE());    
    
  SELECT @LeadID LeadID    
    
 END TRY    
BEGIN CATCH    
 SELECT '' LeadID      
    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                    
    
end catch    
END