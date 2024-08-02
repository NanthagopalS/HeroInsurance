-- =============================================                    
-- Author:  <Author,,Ankit Ghosh>                               
--[Identity_GetUserDetail]                    
-- =============================================                    
CREATE     PROCEDURE [dbo].[Identity_GetUserDetail] (@UserId VARCHAR(100) = NULL)  
AS  
BEGIN  
 BEGIN TRY  
  -- SET NOCOUNT ON added to prevent extra result sets from                    
  -- interfering with SELECT statements.                    
  SET NOCOUNT ON;  
  
  DECLARE @ExamCertificate BIT = 0  
   ,@POSPAgreement BIT = 0  
  
  SELECT IU.UserId  
   ,IU.UserName  
   ,IU.EmailId  
   ,IU.MobileNo AS MobileNo  
   ,IU.RoleId  
   ,IR.RoleName  
   ,RM.RoleName AS UserRoleMappingName  
   ,IU.POSPLeadId  
   ,IU.POSPId  
   ,IU.UserProfileStage  
   ,IU.CreatedBy AS CreatedById  
   ,pa.CreatedOn AS OnboardingDate  
   ,CASE   
    WHEN IUE.IsVerify IS NULL  
     THEN 0  
    ELSE IUE.IsVerify  
    END AS IsEmailVerified  
  FROM Identity_User AS IU WITH (NOLOCK)  
  LEFT JOIN Identity_RoleMaster AS IR WITH (NOLOCK) ON IR.RoleId = IU.RoleId  
  LEFT JOIN Identity_EmailVerification AS IUE WITH (NOLOCK) ON IUE.UserId = IU.UserId  
  LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] pa WITH (NOLOCK) ON pa.UserId = IU.UserId  AND pa.AgreementId is not null
  LEFT JOIN [HeroAdmin].[dbo].[Admin_UserRoleMapping] URM WITH (NOLOCK) ON IU.UserId = URM.UserId  
  LEFT JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] RM WITH (NOLOCK) ON URM.RoleId = RM.RoleId  
  WHERE IU.UserId = @UserId  
  
  IF EXISTS (  
    SELECT DISTINCT Id  
    FROM [HeroPOSP].[dbo].[POSP_Exam] WITH (NOLOCK)  
    WHERE IsActive = 1  
     AND IsCleared = 1  
     AND DocumentId IS NOT NULL  
     AND UserId = @UserId  
    )  
  BEGIN  
   SET @ExamCertificate = 1  
  END  
  
  IF EXISTS (  
    SELECT DISTINCT Id  
    FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)  
    WHERE IsActive = 1  
     AND AgreementId IS NOT NULL  
     AND UserId = @UserId  
    )  
  BEGIN  
   SET @POSPAgreement = 1  
  END  
  
  --IUD.POSPSourceMode, IUD.POSPSourceTypeId, IUD.SourcedByUserId, IUD.ServicedByUserId, IUD.ProfilePictureID, IUD.ProfilePictureFileName, IUD.ProfilePictureStoragePath, IUD.NameDifferentDocument, IUD.NameDifferentDocumentFilePath                     
  SELECT IUD.UserId  
   ,IUD.DateofBirth  
   ,IUD.Gender  
   ,IUD.AlternateContactNo  
   ,IUD.AadhaarNumber AS AadhaarNumber  
   ,IPan.PanNumber AS PAN  
   ,IPan.FatherName  
   ,IUD.IsNameDifferentOnDocument  
   ,IUD.AliasName  
   ,IUD.NOCAvailable  
   ,IUD.IsSelling  
   ,IUD.EducationQualificationTypeId  
   ,EDU.EducationQualificationType  
   ,CREATEDBY.UserName AS CreatedByName  
   ,SERUSER.UserName AS ServicedByUserName  
   ,SOURCEUSER.UserName AS SourcedByName  
   ,IUD.InsuranceSellingExperienceRangeId  
   ,PRM.PremiumRangeType  
   ,PRM.Id AS PremiumRangeId  
   ,@ExamCertificate AS ExamCertificate  
   ,@POSPAgreement AS POSPAgreement  
   ,serUsr.UserName AS RelationshipManagername  
  FROM Identity_UserDetail AS IUD WITH (NOLOCK)  
  LEFT JOIN Identity_PanVerification AS IPan WITH (NOLOCK) ON IPan.UserId = IUD.UserId  
  LEFT JOIN Identity_EducationQualificationTypeMaster AS EDU WITH (NOLOCK) ON EDU.Id = IUD.EducationQualificationTypeId  
  LEFT JOIN Identity_PremiumRangeTypeMaster AS PRM WITH (NOLOCK) ON PRM.Id = IUD.InsuranceSellingExperienceRangeId  
  LEFT JOIN Identity_User SERUSER WITH (NOLOCK) ON IUD.ServicedByUserId = SERUSER.UserId  
  LEFT JOIN Identity_User SOURCEUSER WITH (NOLOCK) ON IUD.SourcedByUserId = SOURCEUSER.UserId  
  LEFT JOIN Identity_User CREATEDBY WITH (NOLOCK) ON IUD.CreatedBy = CREATEDBY.UserId  
  LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH (NOLOCK) ON IUD.ServicedByUserId = serUsr.UserId  
  WHERE IUD.UserId = @UserId  
   AND IPan.IsActive = 1  
  
  SELECT InsuranceProductsOfInterestId  
   ,PRD.ProductName  
  FROM HeroIdentity.dbo.Identity_UserInsuranceProductsOfInterest AS IPI WITH (NOLOCK)  
  LEFT JOIN HeroIdentity.dbo.Identity_InsuranceProductsOfInterest AS PRD WITH (NOLOCK) ON PRD.Id = IPI.InsuranceProductsOfInterestId  
  WHERE IPI.UserId = @UserId  
   AND IPI.IsActive = 1  
  
  SELECT InsurerCompanyOfInterestId  
   ,ICM.InsurerCompanyName  
  FROM HeroIdentity.dbo.Identity_UserInsurerCompanyOfInterest AS ICI WITH (NOLOCK)  
  LEFT JOIN HeroIdentity.dbo.Identity_InsurerCompanyMaster AS ICM WITH (NOLOCK) ON ICM.Id = ICI.InsurerCompanyOfInterestId  
  WHERE ICI.UserId = @UserId  
   AND ICI.IsActive = 1  
  
  SELECT IU.UserId  
   ,UB.Id  
   ,UB.BankId  
   ,UB.IFSC  
   ,UB.AccountHolderName  
   ,UB.AccountNumber  
   ,ib.BankName  
  FROM Identity_User AS IU WITH (NOLOCK)  
  LEFT JOIN Identity_UserBankDetail AS UB WITH (NOLOCK) ON IU.UserId = UB.UserId  
  LEFT JOIN Identity_BankNameMaster AS IB WITH (NOLOCK) ON IB.Id = UB.BankId  
  WHERE IU.UserId = @UserId  
  
  /*SELECT UserId, AddressLine1, AddressLine2, Pincode, CityId, StateId, IsActive FROM Identity_UserAddressDetail WITH(NOLOCK) WHERE UserId = @UserId */  
  SELECT TOP (1) IU.UserId  
   ,UAD.AddressLine1  
   ,UAD.AddressLine2  
   ,UAD.Pincode  
   ,UAD.CityId  
   ,ICity.CityName  
   ,UAD.StateId  
   ,IState.StateName  
  FROM Identity_User AS IU WITH (NOLOCK)  
  LEFT JOIN Identity_UserAddressDetail AS UAD WITH (NOLOCK) ON IU.UserId = UAD.UserId  
  LEFT JOIN Identity_City ICity WITH (NOLOCK) ON ICity.CityId = UAD.CityId  
  LEFT JOIN Identity_State IState WITH (NOLOCK) ON IState.StateId = UAD.StateId  
  WHERE IU.UserId = @UserId  
  ORDER BY UAD.CreatedOn DESC  
  
  SELECT IU.UserId  
   ,IDoc.DocumentTypeId  
   ,DocType.DocumentType  
   ,DocType.IsMandatory  
   ,IDoc.DocumentFileName  
   ,IDoc.IsVerify  
   ,IDoc.IsApprove  
  FROM Identity_User AS IU WITH (NOLOCK)  
  LEFT JOIN Identity_DocumentDetail AS IDoc WITH (NOLOCK) ON IU.UserId = IDoc.UserId  
  LEFT JOIN Identity_DocumentTypeMaster DocType WITH (NOLOCK) ON DocType.Id = IDoc.DocumentTypeId  
  WHERE IU.UserId = @UserId  
   AND IDoc.IsActive = 1  
  
  SELECT ipi.ProductName AS InterestInProduct  
   ,ud.IsSelling AS SellingExperience  
   ,ins.InsurerCompanyName AS ICName  
   ,ipt.PremiumRangeType AS PremiumSold  
   ,pa.CreatedOn AS OnboardingDate  
   ,sd.StampData AS StampNumber  
   ,srUsr.UserName AS SourcedBy  
   ,ud.CreatedBy AS CreatedBy  
   ,serUsr.UserName AS ServicedBy  
   ,PrSale.UserId AS PreSaleUserId  
   ,PrSale.UserName AS PreSale  
   ,PoSale.UserId AS PostSaleUserId  
   ,PoSale.UserName AS PostSale  
   ,Mark.UserId AS MarketingUserId  
   ,Mark.UserName AS Marketing  
   ,Clm.UserId AS ClaimUserId  
   ,Clm.UserName AS Claim  
  FROM Identity_UserDetail AS ud WITH (NOLOCK)  
  LEFT JOIN [Identity_InsuranceProductsOfInterest] AS ipi WITH (NOLOCK) ON ipi.Id = ud.InsuranceProductsofInterestTypeId  
  LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] pa WITH (NOLOCK) ON pa.UserId = ud.UserId  
  LEFT JOIN [HeroAdmin].[dbo].[Admin_StampData] sd WITH (NOLOCK) ON pa.StampId = sd.Id  
  LEFT JOIN [Identity_User] srUsr WITH (NOLOCK) ON srUsr.UserId = ud.SourcedByUserId  
  LEFT JOIN [Identity_User] serUsr WITH (NOLOCK) ON serUsr.UserId = ud.ServicedByUserId  
  LEFT JOIN [Identity_InsurerCompanyMaster] ins WITH (NOLOCK) ON ins.Id = ud.ServicedByUserId  
  LEFT JOIN [Identity_PremiumRangeTypeMaster] AS ipt WITH (NOLOCK) ON ipt.Id = ud.InsuranceSellingExperienceRangeId  
  LEFT JOIN [Identity_User] PrSale WITH (NOLOCK) ON PrSale.UserId = ud.PreSale  
  LEFT JOIN [Identity_User] PoSale WITH (NOLOCK) ON PoSale.UserId = ud.PostSale  
  LEFT JOIN [Identity_User] Mark WITH (NOLOCK) ON Mark.UserId = ud.Marketing  
  LEFT JOIN [Identity_User] Clm WITH (NOLOCK) ON Clm.UserId = ud.Claim  
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
