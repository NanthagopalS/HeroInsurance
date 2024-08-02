
CREATE PROCEDURE [dbo].[Insurance_GodigitUpdateProposalLeadDetails]  
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
@UserId VARCHAR(50) = NULL
AS  
BEGIN  
  
	DECLARE @LeadID Varchar(50), @CustomerType Varchar(20)
	BEGIN TRY
	
		SELECT @LeadID = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionID = @QuoteTransactionID  
		SELECT @CustomerType =CarOwnedBy FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId=@LeadID
		
		IF(@CustomerType='INDIVIDUAL')
		BEGIN
			UPDATE Insurance_LeadDetails SET LeadName = @FirstName,
			MiddleName = @MiddleName,
			LastName = @LastName,
			PhoneNumber = @PhoneNumber, 
			Email = @Email,
			DOB = @DOB,
			Gender = @Gender,
			QuoteTransactionID = @QuoteTransactionID,
			UpdatedBy = @UserId,
			UpdatedOn = GETDATE()
			WHERE LeadId = @LeadID
		END
		ELSE IF(@CustomerType='COMPANY')
		BEGIN
			UPDATE Insurance_LeadDetails SET CompanyName=@CompanyName,
			GSTNo=@GSTNumber,
			PhoneNumber = @PhoneNumber, 
			Email = @Email,
			QuoteTransactionID = @QuoteTransactionID,
			UpdatedBy = @UserId,
			UpdatedOn = GETDATE()
			WHERE LeadId = @LeadID
		END
		


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
