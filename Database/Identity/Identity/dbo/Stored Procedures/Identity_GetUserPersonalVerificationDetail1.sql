  
      
      
CREATE     PROCEDURE [dbo].[Identity_GetUserPersonalVerificationDetail1]      
@UserId VARCHAR(50) = NULL      
AS      
BEGIN      
 BEGIN TRY      
    
    
    SELECT IU.UserId, IU.UserName, IU.EmailId, IU.MobileNo, IU.RoleId, IU.POSPLeadId, IU.POSPId,       
      CASE WHEN IUE.IsVerify IS NULL THEN 0      
      ELSE IUE.IsVerify       
     END AS IsEmailVerified       
      FROM Identity_User IU WITH(NOLOCK)      
      LEFT JOIN Identity_EmailVerification IUE WITH(NOLOCK) ON IUE.UserId = IU.UserId AND IUE.IsActive = 1      
      WHERE IU.UserId = @UserId       
      
    SELECT       
    IUD.UserId, IUD.DateofBirth, IUD.Gender, IUD.AlternateContactNo, IUD.AadhaarNumber, IPan.PanNumber as PAN, IUD.IsNameDifferentOnDocument,      
    IUD.AliasName, IUD.NOCAvailable, IUD.IsSelling,      
    IUD.EducationQualificationTypeId, EDU.EducationQualificationType,      
    IUD.InsuranceSellingExperienceRangeId, PRM.PremiumRangeType                  
    FROM Identity_UserDetail IUD  WITH(NOLOCK)    
    LEFT JOIN Identity_PanVerification IPan WITH(NOLOCK) ON IPan.UserId = IUD.UserId      
    LEFT JOIN Identity_EducationQualificationTypeMaster EDU WITH(NOLOCK) ON EDU.Id = IUD.EducationQualificationTypeId      
    LEFT JOIN Identity_PremiumRangeTypeMaster PRM WITH(NOLOCK) ON PRM.Id = IUD.InsuranceSellingExperienceRangeId      
    LEFT JOIN Identity_InsurerCompanyMaster ICM WITH(NOLOCK) ON ICM.Id = IUD.ServicedByUserId
    WHERE IUD.UserId = @UserId 
	
	Select InsuranceProductsOfInterestId, PRD.ProductName from HeroIdentity.dbo.Identity_UserInsuranceProductsOfInterest IPI WITH(NOLOCK)
	Left Join HeroIdentity.dbo.Identity_InsuranceProductsOfInterest PRD WITH(NOLOCK) on PRD.Id = IPI.InsuranceProductsOfInterestId
	where IPI.UserId = @UserId and IPI.IsActive = 1

	Select InsurerCompanyOfInterestId, ICM.InsurerCompanyName from HeroIdentity.dbo.Identity_UserInsurerCompanyOfInterest ICI WITH(NOLOCK) 
	Left Join HeroIdentity.dbo.Identity_InsurerCompanyMaster ICM WITH(NOLOCK) on ICM.Id = ICI.InsurerCompanyOfInterestId
	where ICI.UserId = @UserId and ICI.IsActive = 1
      
    if exists (Select 1 from HeroIdentity.dbo.Identity_PanVerification PV inner Join TestPAN Tp WITH(NOLOCK) on Tp.PANNumber = PV.PanNumber
	where Pv.UserId = @UserId)      
     Begin      
    
   Select  PV.[PanVerificationId], TP.[Name] as [Name], TP.[DOB], TP.[PanNumber], IU.EmailId, TP.FatherName, Iu.UserId from TestPAN Tp WITH(NOLOCK)    
   Left join HeroIdentity.dbo.Identity_PanVerification PV WITH(NOLOCK) on PV.PanNumber = Tp.PANNumber    
   Left Join HeroIdentity.dbo.Identity_User IU WITH(NOLOCK) on IU.UserId = Pv.UserId    
   where IU.UserId = @UserId    
      
     End      
    else       
    Begin      
     SELECT IUP.[PanVerificationId], IUP.[Name] as [Name], IUP.[DOB], IUP.[PanNumber], IU.EmailId, IUP.FatherName      
     FROM Identity_User IU WITH(NOLOCK)      
     LEFT JOIN Identity_PanVerification IUP WITH(NOLOCK) ON IU.UserId = IUP.UserId      
     WHERE IU.UserId =@UserId       
     AND DATEDIFF(MONTH, CAST(IUP.PanAddUpdateOn AS DATE), CAST(GETDATE() AS DATE)) <= 6      
    End      
      
    SELECT       
    TOP(1) IU.UserId, UAD.AddressLine1, UAD.AddressLine2, UAD.Pincode,       
    UAD.CityId, ICity.CityName, UAD.StateId, IState.StateName      
    FROM Identity_User IU WITH(NOLOCK)     
    LEFT JOIN Identity_UserAddressDetail UAD WITH(NOLOCK) ON IU.UserId = UAD.UserId      
    LEFT JOIN Identity_City ICity WITH(NOLOCK) ON ICity.CityId = UAD.CityId      
    LEFT JOIN Identity_State IState WITH(NOLOCK) ON IState.StateId = UAD.StateId      
    WHERE IU.UserId = @UserId       
    ORDER BY UAD.CreatedOn DESC      
      
      
         
 END TRY      
      
 BEGIN CATCH                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC dbo.Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH      
END