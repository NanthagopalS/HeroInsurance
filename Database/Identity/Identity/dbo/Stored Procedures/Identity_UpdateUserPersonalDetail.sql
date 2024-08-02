      
-- =============================================                          
-- Author:  <Author,ANKIT GHOSH>                                 
-- Description: <Description,,UPDATE USER PERSONAL DETAIL>                          
-- =============================================                          
CREATE      PROCEDURE [dbo].[Identity_UpdateUserPersonalDetail] (      
 @UserId VARCHAR(500) = NULL      
 ,@Gender VARCHAR(20) = NULL      
 ,@FatherName VARCHAR(50) = NULL      
 ,@DOB VARCHAR(20) = NULL      
 ,@Email VARCHAR(100) = NULL      
 ,@AlternateContactNo VARCHAR(10) = NULL      
 ,@AadhaarNumber VARCHAR(12) = NULL      
 ,@IsNameDifferentOnDocument BIT = 0      
 ,@NameDifferentOnDocument VARCHAR(1000) = NULL      
 ,@NameDifferentOnDocumentFilePath VARCHAR(1000) = NULL      
 ,@AliasName VARCHAR(500) = NULL      
 ,@AddressLine1 VARCHAR(200) = NULL      
 ,@AddressLine2 VARCHAR(200) = NULL      
 ,@Pincode INT = 0      
 ,@CityId VARCHAR(500) = NULL      
 ,@StateId VARCHAR(500) = NULL      
 ,@EducationQualificationTypeId VARCHAR(500) = NULL      
 ,@InsuranceSellingExperienceRangeId VARCHAR(500) = NULL      
 ,@InsuranceProductsofInterestTypeId VARCHAR(MAX) = NULL      
 ,@POSPSourceMode BIT = 0      
 ,@POSPSourceTypeId VARCHAR(500) = NULL      
 ,@SourcedByUserId VARCHAR(500) = NULL      
 ,@ServicedByUserId VARCHAR(MAX) = NULL      
 ,@DocumentId VARCHAR(500) = NULL      
 ,@NOCAvailable VARCHAR(100) = NULL      
 ,@IsSelling VARCHAR(10) = 'NO'      
 ,@IsDraft BIT      
 ,@InsuranceCompanyofInterestTypeId VARCHAR(MAX) = NULL      
 ,@AssistedBUId VARCHAR(200) = NULL      
 ,@CreatedBy VARCHAR(100) = NULL      
 ,@IsAdminUpdating BIT = 0      
 )      
AS      
BEGIN      
 BEGIN TRY      
  BEGIN      
   --Drop table testInsuranceId                  
   IF OBJECT_ID('#testInsuranceId') IS NOT NULL      
    DROP TABLE #testInsuranceId      
      
   CREATE TABLE #testInsuranceId (InsuranceProductsofInterestTypeId VARCHAR(100) NOT NULL)      
      
   INSERT INTO #testInsuranceId (InsuranceProductsofInterestTypeId)      
   SELECT value      
   FROM string_split(@InsuranceProductsofInterestTypeId, ',')      
      
   --select * from testInsuranceId                  
   IF EXISTS (      
     SELECT 1      
     FROM [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest] WITH (NOLOCK)      
     WHERE UserId = @UserId      
     )      
   BEGIN      
    DELETE      
    FROM [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest]      
    WHERE UserId = @UserId      
     AND IsActive = 1      
   END      
      
   INSERT INTO [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest] (      
    UserId      
    ,InsuranceProductsOfInterestId      
    )      
   SELECT @UserId      
    ,InsuranceProductsofInterestTypeId      
   FROM #testInsuranceId      
      
   --select * from [HeroIdentity].[dbo].[Identity_UserInsuranceProductsOfInterest]                  
   DROP TABLE #testInsuranceId      
  END      
      
  BEGIN      
   IF OBJECT_ID('#testICId') IS NOT NULL      
    DROP TABLE #testICId      
      
   CREATE TABLE #testICId (InsurerCompanyOfInterestId VARCHAR(100) NOT NULL)      
      
   INSERT INTO #testICId (InsurerCompanyOfInterestId)      
   SELECT value      
   FROM string_split(@InsuranceCompanyofInterestTypeId, ',')      
      
   --select * from testICId                 
   IF EXISTS (      
     SELECT 1      
     FROM [HeroIdentity].[dbo].[Identity_UserInsurerCompanyOfInterest] WITH (NOLOCK)      
     WHERE UserId = @UserId      
     )      
   BEGIN      
    DELETE      
    FROM [HeroIdentity].[dbo].[Identity_UserInsurerCompanyOfInterest]      
    WHERE UserId = @UserId      
     AND IsActive = 1      
   END      
      
   INSERT INTO [HeroIdentity].[dbo].[Identity_UserInsurerCompanyOfInterest] (      
    UserId      
    ,InsurerCompanyOfInterestId      
    )      
   SELECT @UserId      
    ,InsurerCompanyOfInterestId      
   FROM #testICId      
      
   --select * from [HeroIdentity].[dbo].[Identity_UserInsurerCompanyOfInterest]                  
   DROP TABLE #testICId      
  END      
      
  BEGIN      
   IF EXISTS (      
     SELECT EmailId      
     FROM [dbo].[Identity_User] WITH (NOLOCK)      
     WHERE UserId = @UserId      
      AND IsActive = 1      
  )      
    UPDATE Identity_User      
    SET EmailId = @Email, UpdatedOn = GETDATE()      
    WHERE UserId = @UserId      
  END      
      
  UPDATE Identity_PanVerification      
  SET FatherName = @FatherName, UpdatedOn = GETDATE()      
  WHERE UserId = @UserId      
      
  IF EXISTS (      
    SELECT DISTINCT Id      
    FROM [dbo].[Identity_UserDetail] WITH (NOLOCK)      
    WHERE UserId = @UserId      
    )      
  BEGIN      
   UPDATE [dbo].[Identity_UserDetail]      
   SET Gender = @Gender      
    ,DateofBirth = @DOB      
    ,AlternateContactNo = @AlternateContactNo      
    ,AadhaarNumber = @AadhaarNumber      
    ,IsNameDifferentOnDocument = @IsNameDifferentOnDocument      
    ,NameDifferentDocument = @NameDifferentOnDocument      
    ,NameDifferentDocumentFilePath = @NameDifferentOnDocumentFilePath      
    ,AliasName = @AliasName      
    ,EducationQualificationTypeId = @EducationQualificationTypeId      
    ,InsuranceSellingExperienceRangeId = @InsuranceSellingExperienceRangeId      
    ,      
    --InsuranceProductsofInterestTypeId = @InsuranceProductsofInterestTypeId,                           
    POSPSourceMode = @POSPSourceMode      
    ,POSPSourceTypeId = @POSPSourceTypeId      
    ,SourcedByUserId = @SourcedByUserId      
    ,ServicedByUserId = @ServicedByUserId      
    ,NOCAvailable = @NOCAvailable      
    ,DocumentId = @DocumentId      
    ,IsSelling = @IsSelling      
    ,AssistedBUId = @AssistedBUId      
    ,CreatedBy = @CreatedBy , UpdatedOn = GETDATE()     
   WHERE UserId = @UserId      
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
   INSERT INTO [dbo].[Identity_UserDetail] (      
    UserId      
    ,DateofBirth      
    ,Gender      
    ,AlternateContactNo      
    ,AadhaarNumber      
    ,IsNameDifferentOnDocument      
    ,NameDifferentDocument      
    ,NameDifferentDocumentFilePath      
    ,AliasName      
    ,EducationQualificationTypeId      
    ,InsuranceSellingExperienceRangeId      
    ,InsuranceProductsofInterestTypeId      
    ,POSPSourceMode      
    ,POSPSourceTypeId      
    ,SourcedByUserId      
    ,ServicedByUserId      
    ,DocumentId      
    ,NOCAvailable      
    ,IsSelling      
    ,CreatedBy      
    ,AssistedBUId      
    )      
   VALUES (      
    @UserId      
    ,@DOB      
    ,@Gender      
    ,@AlternateContactNo      
    ,@AadhaarNumber      
    ,@IsNameDifferentOnDocument      
    ,@NameDifferentOnDocument      
    ,@NameDifferentOnDocumentFilePath      
    ,@AliasName      
    ,@EducationQualificationTypeId      
    ,@InsuranceSellingExperienceRangeId      
    ,@InsuranceProductsofInterestTypeId      
    ,@POSPSourceMode      
    ,@POSPSourceTypeId      
    ,@SourcedByUserId      
    ,@ServicedByUserId      
    ,@DocumentId      
    ,@NOCAvailable      
    ,@IsSelling      
    ,@CreatedBy      
    ,@AssistedBUId      
    )      
  END      
      
  --, RelationshipManager, PreSale, PostSale, Marketing, Claim                  
  --, @RelationshipManagerId, @PreSaleUserId, @PostSaleUserId, @MarketingUserId, @ClaimUserId                  
  IF EXISTS (      
    SELECT DISTINCT Id      
    FROM [dbo].[Identity_UserAddressDetail] WITH (NOLOCK)      
    WHERE UserId = @UserId      
    )      
  BEGIN      
   UPDATE [dbo].[Identity_UserAddressDetail]      
   SET AddressLine1 = @AddressLine1      
    ,AddressLine2 = @AddressLine2      
    ,Pincode = @Pincode      
    ,CityId = @CityId      
    ,StateId = @StateId , UpdatedOn = GETDATE()     
   WHERE UserId = @UserId      
  END      
  ELSE      
  BEGIN      
   INSERT INTO [dbo].[Identity_UserAddressDetail] (      
    UserId   
    ,AddressLine1      
    ,AddressLine2      
    ,Pincode      
    ,CityId      
    ,StateId      
    )      
   VALUES (      
    @UserId      
    ,@AddressLine1      
    ,@AddressLine2      
    ,@Pincode      
    ,@CityId      
    ,@StateId      
    )      
  END      
      
  --UPDATE PROFILE STAGE FOR USERS      
      
  Declare @UserProfileStage int     
 Set @UserProfileStage = (Select UserProfileStage from Identity_User where UserId = @UserId)    
    
  IF (      
    @IsDraft = 1      
    AND @IsAdminUpdating = 0  and @UserProfileStage < 4    
    )      
  BEGIN      
   UPDATE Identity_User      
   SET UserProfileStage = 1 , UpdatedOn = GETDATE()     
   WHERE UserId = @UserId    
   EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId, '8EB4B84F-F12D-44E2-9A62-2C3A4848ABB4'
  END      
    
    
  IF(@IsDraft!=1  AND @IsAdminUpdating = 0 )      
   BEGIN      
    UPDATE Identity_User SET UserProfileStage = 2, UpdatedOn = GETDATE() WHERE UserId = @UserId      
	EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId, '8A2010DF-0137-4ED0-BDA6-6A8DF9128827'  
   END      
 END TRY      
      
 BEGIN CATCH      
  DECLARE @StrProcedure_Name VARCHAR(500)      
   ,@ErrorDetail VARCHAR(1000)      
   ,@ParameterList VARCHAR(2000)      
      
  SET @StrProcedure_Name = ERROR_PROCEDURE()      
  SET @ErrorDetail = ERROR_MESSAGE()      
      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name      
   ,@ErrorDetail = @ErrorDetail      
   ,@ParameterList = @ParameterList      
 END CATCH      
END 
