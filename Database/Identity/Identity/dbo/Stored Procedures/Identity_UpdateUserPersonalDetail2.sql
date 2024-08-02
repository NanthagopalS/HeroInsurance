        
-- =============================================        
-- Author:  <Author,ANKIT GHOSH>               
-- Description: <Description,,UPDATE USER PERSONAL DETAIL>        
-- =============================================        
CREATE   PROCEDURE [dbo].[Identity_UpdateUserPersonalDetail2]         
(        
 @UserId VARCHAR(500) = NULL,        
 @Gender VARCHAR(20) = NULL,  
 @FatherName VARCHAR(50) = NULL,  
 @DOB VARCHAR(20) = NULL,       
 @AlternateContactNo VARCHAR(10) = NULL,        
 @AadhaarNumber VARCHAR(12) = NULL,        
 @IsNameDifferentOnDocument bit = 0,        
 @NameDifferentOnDocument VARCHAR(1000) = NULL,        
 @NameDifferentOnDocumentFilePath VARCHAR(1000) = NULL,        
 @AliasName VARCHAR(500) = NULL,        
 @AddressLine1 VARCHAR(200) = NULL,        
 @AddressLine2 VARCHAR(200) = NULL,        
 @Pincode int = 0,        
 @CityId VARCHAR(500) = NULL,        
 @StateId VARCHAR(500) = NULL,        
 @EducationQualificationTypeId VARCHAR(500) = NULL,        
 @InsuranceSellingExperienceRangeId VARCHAR(500) = NULL,        
 @InsuranceProductsofInterestTypeId VARCHAR(MAX) = NULL,    
 @POSPSourceMode bit = 0,        
 @POSPSourceTypeId VARCHAR(500) = NULL,        
 @SourcedByUserId VARCHAR(500) = NULL,        
 @ServicedByUserId VARCHAR(MAX) = NULL,      
 @DocumentId VARCHAR(500)  =NULL,    
 @NOCAvailable VARCHAR(100),    
 @IsSelling VARCHAR(10) = 'NO'
 --@RelationshipManagerId VARCHAR(100) = NULL, -- Newly Added 
 --@PreSaleUserId VARCHAR(100) = NULL,
 --@PostSaleUserId VARCHAR(100) = NULL,
 --@MarketingUserId VARCHAR(100) = NULL,
 --@ClaimUserId VARCHAR(100) = NULL
 --@ICName VARCHAR(100) = NULL,    -- Newly Added From IC Name -- StoreICId
 --@PremiumSold VARCHAR(100) = NULL,
 --@PolicyTagged VARCHAR(100) = NULL,
 --@StampNumber VARCHAR(100) = NULL, 
 --@SourcedBy VARCHAR(100) = NULL,
 --@CreatedBy VARCHAR(100) = NULL,
 --@ServicedBy VARCHAR(100) = NULL,
 --@ProductTeam VARCHAR(100) = NULL,
 --@Tagging VARCHAR(100) = NULL,
 --@OnboardingDate VARCHAR(100) = NULL 
)        
AS        
BEGIN        
 BEGIN TRY        
	 BEGIN
	  --Drop table testInsuranceId
	  IF OBJECT_ID('#testInsuranceId') IS NOT NULL
    DROP TABLE #testInsuranceId
	Create table #testInsuranceId(
		InsuranceProductsofInterestTypeId VARCHAR(100) NOT NULL
	)

	Insert into #testInsuranceId (InsuranceProductsofInterestTypeId) Select value from string_split(@InsuranceProductsofInterestTypeId,',')
	--select * from testInsuranceId
	if exists (Select 1 from [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest] where UserId = @UserId)
	BEGIN
		Delete from [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest] where UserId = @UserId and IsActive = 1
	END
	Insert into [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest] (UserId, InsuranceProductsOfInterestId)
	Select @UserId, InsuranceProductsofInterestTypeId from #testInsuranceId 
	--select * from [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest]
	Drop table #testInsuranceId
	END

	BEGIN
	IF OBJECT_ID('#testICId') IS NOT NULL
    DROP TABLE #testICId
	Create table #testICId(
		InsurerCompanyOfInterestId VARCHAR(100) NOT NULL
	)

	Insert into #testICId (InsurerCompanyOfInterestId) Select value from string_split(@ServicedByUserId,',')
	--select * from testICId
	if exists (Select 1 from [HeroIdentity].[dbo].[Identity_UserInsurerCompanyOfInterest] where UserId = @UserId)
	BEGIN
		Delete from [HeroIdentity].[dbo].[Identity_UserInsurerCompanyOfInterest] where UserId = @UserId and IsActive = 1
	END
	Insert into [HeroIdentity].[dbo].[Identity_UserInsurerCompanyOfInterest] (UserId, InsurerCompanyOfInterestId)
	Select @UserId, InsurerCompanyOfInterestId from #testICId 
	--select * from [HeroIdentity].[dbo].[Identity_UserInsurerCompanyOfInterest]
	Drop table #testICId
	END
     
  
 UPDATE Identity_PanVerification SET FatherName = @FatherName WHERE UserId = @UserId  
  
  IF EXISTS(SELECT DISTINCT Id FROM [dbo].[Identity_UserDetail] WHERE UserId = @UserId)        
  BEGIN   
     
   UPDATE [dbo].[Identity_UserDetail] SET         
    Gender = @Gender,      
	DateofBirth = @DOB,      
    AlternateContactNo = @AlternateContactNo,         
    AadhaarNumber = @AadhaarNumber,         
    IsNameDifferentOnDocument = @IsNameDifferentOnDocument,         
    NameDifferentDocument = @NameDifferentOnDocument,        
    NameDifferentDocumentFilePath =  @NameDifferentOnDocumentFilePath,        
    AliasName = @AliasName,            
    EducationQualificationTypeId = @EducationQualificationTypeId,         
    InsuranceSellingExperienceRangeId = @InsuranceSellingExperienceRangeId,         
    --InsuranceProductsofInterestTypeId = @InsuranceProductsofInterestTypeId,         
    POSPSourceMode = @POSPSourceMode,         
    POSPSourceTypeId = @POSPSourceTypeId,         
    SourcedByUserId = @SourcedByUserId,         
    --ServicedByUserId = @ServicedByUserId,         
    NOCAvailable = @NOCAvailable,    
    DocumentId = @DocumentId,  
    IsSelling = @IsSelling
   WHERE        
    UserId = @UserId        
      
	  /*
	RelationshipManager = @RelationshipManagerId,
	PreSale = @PreSaleUserId,
	PostSale = @PostSaleUserId,
	Marketing = @MarketingUserId,
	Claim = @ClaimUserId
	  */

  END        
  ELSE        
  BEGIN        
   INSERT INTO [dbo].[Identity_UserDetail] (UserId, DateofBirth, Gender, AlternateContactNo, AadhaarNumber, 
   IsNameDifferentOnDocument, NameDifferentDocument, NameDifferentDocumentFilePath, AliasName, EducationQualificationTypeId,
   InsuranceSellingExperienceRangeId, InsuranceProductsofInterestTypeId, POSPSourceMode, POSPSourceTypeId, SourcedByUserId,
   ServicedByUserId, DocumentId, NOCAvailable, IsSelling)         
   VALUES         
   (@UserId, @DOB, @Gender, @AlternateContactNo, @AadhaarNumber, @IsNameDifferentOnDocument, @NameDifferentOnDocument,  
   @NameDifferentOnDocumentFilePath, @AliasName, @EducationQualificationTypeId, @InsuranceSellingExperienceRangeId, 
   @InsuranceProductsofInterestTypeId, @POSPSourceMode, @POSPSourceTypeId, @SourcedByUserId, @ServicedByUserId, @DocumentId,
   @NOCAvailable, @IsSelling)      
         
  END        
          --, RelationshipManager, PreSale, PostSale, Marketing, Claim
		  --, @RelationshipManagerId, @PreSaleUserId, @PostSaleUserId, @MarketingUserId, @ClaimUserId
          
  IF EXISTS(SELECT DISTINCT Id FROM [dbo].[Identity_UserAddressDetail] WHERE UserId = @UserId)        
  BEGIN        
   UPDATE [dbo].[Identity_UserAddressDetail] SET         
    AddressLine1 = @AddressLine1,         
    AddressLine2 = @AddressLine2,         
    Pincode = @Pincode,         
    CityId = @CityId,         
    StateId = @StateId        
   WHERE        
    UserId = @UserId          
	END        
  ELSE        
  BEGIN        
   INSERT INTO [dbo].[Identity_UserAddressDetail] (UserId, AddressLine1, AddressLine2, Pincode, CityId, StateId)         
   VALUES         
   (@UserId, @AddressLine1, @AddressLine2, @Pincode, @CityId, @StateId)        
  END        
        
		/*
		(IsDraft = 0)
		{
			UPDATE Identity_User SET UserProfileStage = 2 WHERE UserId=@UserId 
		}



		*/


  --UPDATE PROFILE STAGE FOR USERS      
  UPDATE Identity_User SET UserProfileStage = 2 WHERE UserId=@UserId      
      
 END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList        
          
 END CATCH        
        
END