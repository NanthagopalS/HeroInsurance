            
-- =============================================            
-- Author:  <Author,,Ankit Ghosh>                       
--[Identity_GetUserDetail1]            
-- =============================================            
CREATE   PROCEDURE[dbo].[Identity_GetUserDetail1]            
(            
 @UserId VARCHAR(100) = NULL            
)            
AS            
BEGIN             
 BEGIN TRY            
  -- SET NOCOUNT ON added to prevent extra result sets from            
  -- interfering with SELECT statements.            
  SET NOCOUNT ON;            
            
  DECLARE @ExamCertificate bit = 0, @POSPAgreement bit = 0            
            
  SELECT IU.UserId, IU.UserName, IU.EmailId, IU.MobileNo, IU.RoleId, IR.RoleName,        
         IU.POSPLeadId, IU.POSPId, IU.UserProfileStage,         
      CASE WHEN IUE.IsVerify IS NULL THEN 0            
      ELSE IUE.IsVerify             
     END AS IsEmailVerified             
  FROM Identity_User IU WITH(NOLOCK)           
  LEFT JOIN Identity_RoleMaster IR WITH(NOLOCK) ON IR.RoleId = IU.RoleId            
  LEFT JOIN Identity_EmailVerification IUE WITH(NOLOCK) ON IUE.UserId = IU.UserId AND IUE.IsActive = 1            
  WHERE IU.UserId = @UserId             
              
  IF EXISTS(SELECT DISTINCT Id FROM [HeroPOSP].[dbo].[POSP_Exam] WITH(NOLOCK) WHERE IsActive = 1 AND IsCleared = 1 AND DocumentId IS NOT NULL AND UserId = @UserId)                  
  BEGIN            
   SET @ExamCertificate = 1            
  END            
            
  IF EXISTS(SELECT DISTINCT Id FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH(NOLOCK) WHERE IsActive = 1 AND AgreementId IS NOT NULL AND UserId = @UserId)                  
  BEGIN            
   SET @POSPAgreement = 1            
  END            
            
  --IUD.POSPSourceMode, IUD.POSPSourceTypeId, IUD.SourcedByUserId, IUD.ServicedByUserId, IUD.ProfilePictureID, IUD.ProfilePictureFileName, IUD.ProfilePictureStoragePath, IUD.NameDifferentDocument, IUD.NameDifferentDocumentFilePath             
              
  SELECT             
   IUD.UserId, IUD.DateofBirth, IUD.Gender, IUD.AlternateContactNo,             
   CONCAT('XXXX XXXX ', right(IUD.AadhaarNumber, len(IUD.AadhaarNumber)-8)) as AadhaarNumber,             
   CONCAT('XXX XXX ', right(IPan.PanNumber, len(IPan.PanNumber)-6)) as PAN, IPan.FatherName, IUD.IsNameDifferentOnDocument,            
   IUD.AliasName, IUD.NOCAvailable, IUD.IsSelling,            
   IUD.EducationQualificationTypeId, EDU.EducationQualificationType,            
   IUD.InsuranceSellingExperienceRangeId, PRM.PremiumRangeType, PRM.Id as PremiumRangeId,            
   --IUD.InsuranceProductsofInterestTypeId, PRD.ProductName, PRD.Id as ProductId,       
   --IUD.ServicedByUserId, ICM.InsurerCompanyName,ICM.Id as InsurerCompanyId,            
   @ExamCertificate as ExamCertificate,            
   @POSPAgreement as POSPAgreement            
  FROM Identity_UserDetail IUD  WITH(NOLOCK)             
   LEFT JOIN Identity_PanVerification IPan WITH(NOLOCK) ON IPan.UserId = IUD.UserId            
   LEFT JOIN Identity_EducationQualificationTypeMaster EDU WITH(NOLOCK) ON EDU.Id = IUD.EducationQualificationTypeId            
   LEFT JOIN Identity_PremiumRangeTypeMaster PRM WITH(NOLOCK) ON PRM.Id = IUD.InsuranceSellingExperienceRangeId            
   --LEFT JOIN Identity_InsuranceProductsOfInterest PRD ON PRD.Id = IUD.InsuranceProductsofInterestTypeId            
   --LEFT JOIN Identity_InsurerCompanyMaster ICM ON ICM.Id = IUD.ServicedByUserId            
  WHERE IUD.UserId = @UserId      
  
  Select InsuranceProductsOfInterestId, PRD.ProductName from HeroIdentity.dbo.Identity_UserInsuranceProductsOfInterest IPI WITH(NOLOCK)
	Left Join HeroIdentity.dbo.Identity_InsuranceProductsOfInterest PRD WITH(NOLOCK) on PRD.Id = IPI.InsuranceProductsOfInterestId
	where IPI.UserId = @UserId and IPI.IsActive = 1

	Select InsurerCompanyOfInterestId, ICM.InsurerCompanyName from HeroIdentity.dbo.Identity_UserInsurerCompanyOfInterest ICI WITH(NOLOCK)
	Left Join HeroIdentity.dbo.Identity_InsurerCompanyMaster ICM WITH(NOLOCK) on ICM.Id = ICI.InsurerCompanyOfInterestId
	where ICI.UserId = @UserId and ICI.IsActive = 1
            
            
  SELECT IU.UserId, UB.Id, UB.BankId, UB.IFSC, UB.AccountHolderName, UB.AccountNumber, ib.BankName             
  FROM Identity_User IU WITH(NOLOCK)            
   LEFT JOIN Identity_UserBankDetail UB WITH(NOLOCK) ON IU.UserId = UB.UserId             
   LEFT JOIN Identity_BankNameMaster IB WITH(NOLOCK)  ON IB.Id = UB.BankId            
  WHERE IU.UserId = @UserId            
              
  /*SELECT UserId, AddressLine1, AddressLine2, Pincode, CityId, StateId, IsActive FROM Identity_UserAddressDetail WITH(NOLOCK) WHERE UserId = @UserId */             
            
  SELECT             
   TOP(1) IU.UserId, UAD.AddressLine1, UAD.AddressLine2, UAD.Pincode,             
 UAD.CityId, ICity.CityName, UAD.StateId, IState.StateName            
  FROM Identity_User IU  WITH(NOLOCK)           
   LEFT JOIN Identity_UserAddressDetail UAD WITH(NOLOCK) ON IU.UserId = UAD.UserId            
   LEFT JOIN Identity_City ICity WITH(NOLOCK) ON ICity.CityId = UAD.CityId            
   LEFT JOIN Identity_State IState WITH(NOLOCK) ON IState.StateId = UAD.StateId            
  WHERE IU.UserId = @UserId             
  ORDER BY UAD.CreatedOn DESC            
            
  SELECT             
   IU.UserId, IDoc.DocumentTypeId, DocType.DocumentType, DocType.IsMandatory,             
   IDoc.DocumentFileName, IDoc.IsVerify, IDoc.IsApprove            
  FROM Identity_User IU WITH(NOLOCK)            
  LEFT JOIN Identity_DocumentDetail IDoc WITH(NOLOCK) ON IU.UserId = IDoc.UserId            
  LEFT JOIN Identity_DocumentTypeMaster DocType WITH(NOLOCK) on DocType.Id = IDoc.DocumentTypeId            
  WHERE IU.UserId = @UserId AND IDoc.IsActive = 1            
       
  SELECT             
    ipi.ProductName as InterestInProduct,ud.IsSelling as SellingExperience,ins.InsurerCompanyName as ICName,  
    ipt.PremiumRangeType as PremiumSold,pa.CreatedOn as OnboardingDate,    
    sd.StampData as StampNumber,   
    srUsr.UserName as SourcedBy,ud.CreatedBy as CreatedBy,  
    serUsr.UserName as ServicedBy,    
    PrSale.UserId as PreSaleUserId,  
    PrSale.UserName as PreSale,      
    PoSale.UserId as PostSaleUserId,  
    PoSale.UserName as PostSale,      
    Mark.UserId as MarketingUserId,    
    Mark.UserName as Marketing,      
    Clm.UserId as ClaimUserId,       
    Clm.UserName as Claim       
   FROM Identity_UserDetail ud WITH(NOLOCK)   
    LEFT JOIN [Identity_InsuranceProductsOfInterest] ipi WITH(NOLOCK) on ipi.Id = ud.InsuranceProductsofInterestTypeId  
    LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] pa WITH(NOLOCK) on pa.UserId = ud.UserId  
    LEFT JOIN [HeroAdmin].[dbo].[Admin_StampData] sd WITH(NOLOCK) on pa.StampId = sd.Id  
    LEFT JOIN [Identity_User] srUsr WITH(NOLOCK) on srUsr.UserId = ud.SourcedByUserId  
    LEFT JOIN [Identity_User] serUsr WITH(NOLOCK) on serUsr.UserId = ud.ServicedByUserId  
    LEFT JOIN [Identity_InsurerCompanyMaster] ins WITH(NOLOCK) on ins.Id = ud.ServicedByUserId  
    LEFT JOIN [Identity_PremiumRangeTypeMaster] ipt WITH(NOLOCK) on ipt.Id = ud.InsuranceSellingExperienceRangeId  
    LEFT JOIN [Identity_User] PrSale WITH(NOLOCK) ON PrSale.UserId = ud.PreSale  
    LEFT JOIN [Identity_User] PoSale WITH(NOLOCK) ON PoSale.UserId = ud.PostSale  
    LEFT JOIN [Identity_User] Mark WITH(NOLOCK) ON Mark.UserId = ud.Marketing  
    LEFT JOIN [Identity_User] Clm WITH(NOLOCK) ON Clm.UserId = ud.Claim  
  
 END TRY                            
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
END