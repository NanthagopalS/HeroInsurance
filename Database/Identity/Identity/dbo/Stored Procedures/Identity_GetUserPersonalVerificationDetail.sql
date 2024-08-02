       
CREATE         PROCEDURE [dbo].[Identity_GetUserPersonalVerificationDetail]                  
@UserId VARCHAR(50) = NULL                  
AS                  
BEGIN                  
 BEGIN TRY                  
                
    SELECT IU.UserId, IU.UserName, IU.EmailId, IU.MobileNo, IU.RoleId,        
 IU.POSPLeadId, IU.POSPId, IU.UserProfileStage,IR.RoleName,  IU.CreatedBy as CreatedById,         
      CASE WHEN IUE.IsVerify IS NULL THEN 0               
      ELSE IUE.IsVerify                   
     END AS IsEmailVerified , IU.IsActive ,   
  CASE WHEN PA.AgreementId IS NULL THEN NULL ELSE CAST(PA.CreatedOn AS DATE) END AS OnboardingDate           
      FROM Identity_User IU WITH(NOLOCK)                 
      LEFT JOIN Identity_EmailVerification IUE WITH(NOLOCK) ON IUE.UserId = IU.UserId AND IUE.IsActive = 1         
 LEFT JOIN Identity_RoleMaster IR WITH(NOLOCK) ON IR.RoleId = IU.RoleId     
  LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] PA WITH(NOLOCK) ON PA.UserId = IU.UserId AND PA.IsActive = 1   
    
      WHERE IU.UserId = @UserId  and IU.IsActive = 1                
                  
    SELECT                   
    IUD.UserId, IUD.DateofBirth, IUD.Gender, IUD.AlternateContactNo, IUD.AadhaarNumber, IPan.PanNumber as PAN, IUD.IsNameDifferentOnDocument,                  
    IUD.AliasName, IUD.NOCAvailable, IUD.IsSelling, IUD.AssistedBUId, IUD.SourcedByUserId, IUD.ServicedByUserId,  
 CREATEDBY.UserName as CreatedByName, SERUSER.UserName AS ServicedByUserName, SOURCEUSER.UserName as SourcedByName,        
    IUD.EducationQualificationTypeId, EDU.EducationQualificationType, IUD.POSPSourceTypeId, SERUSER.UserName AS RelationshipMangername,          
    IUD.InsuranceSellingExperienceRangeId, PRM.PremiumRangeType  , IUD.CreatedBy as CreatedById                           
    FROM Identity_UserDetail IUD  WITH(NOLOCK)                
    LEFT JOIN Identity_PanVerification IPan WITH(NOLOCK) ON IPan.UserId = IUD.UserId                  
    LEFT JOIN Identity_EducationQualificationTypeMaster EDU WITH(NOLOCK) ON EDU.Id = IUD.EducationQualificationTypeId                  
    LEFT JOIN Identity_PremiumRangeTypeMaster PRM WITH(NOLOCK) ON PRM.Id = IUD.InsuranceSellingExperienceRangeId  
 LEFT JOIN Identity_User SERUSER WITH(NOLOCK) ON IUD.ServicedByUserId = SERUSER.UserId  
 LEFT JOIN Identity_User SOURCEUSER  WITH(NOLOCK) ON IUD.SourcedByUserId = SOURCEUSER.UserId  
 LEFT JOIN Identity_User CREATEDBY WITH(NOLOCK) ON IUD.CreatedBy = CREATEDBY.UserId  
    --LEFT JOIN Identity_InsurerCompanyMaster ICM ON ICM.Id = IUD.ServicedByUserId            
    WHERE IUD.UserId = @UserId  and IUD.IsActive = 1  and IPan.IsActive = 1
             
 Select InsuranceProductsOfInterestId, PRD.ProductName from HeroIdentity.dbo.Identity_UserInsuranceProductsOfInterest IPI WITH(NOLOCK)           
 Left Join HeroIdentity.dbo.Identity_InsuranceProductsOfInterest PRD WITH(NOLOCK) on PRD.Id = IPI.InsuranceProductsOfInterestId            
 where IPI.UserId = @UserId and IPI.IsActive = 1            
            
 Select InsurerCompanyOfInterestId, ICM.InsurerCompanyName from HeroIdentity.dbo.Identity_UserInsurerCompanyOfInterest ICI WITH(NOLOCK)           
 Left Join HeroIdentity.dbo.Identity_InsurerCompanyMaster ICM WITH(NOLOCK) on ICM.Id = ICI.InsurerCompanyOfInterestId            
 where ICI.UserId = @UserId and ICI.IsActive = 1            
                  
    if exists (Select 1 from HeroIdentity.dbo.Identity_PanVerification PV WITH(NOLOCK) inner Join TestPAN Tp WITH(NOLOCK) on Tp.PANNumber = PV.PanNumber            
 where Pv.UserId = @UserId and PV.IsActive = 1)                  
     Begin                  
                
   Select  PV.[PanVerificationId], TP.[Name] as [Name], TP.[DOB], TP.[PanNumber], IU.EmailId, TP.FatherName, Iu.UserId from TestPAN Tp WITH(NOLOCK)               
   Left join HeroIdentity.dbo.Identity_PanVerification PV WITH(NOLOCK) on PV.PanNumber = Tp.PANNumber                
   Left Join HeroIdentity.dbo.Identity_User IU WITH(NOLOCK) on IU.UserId = Pv.UserId                
   where IU.UserId = @UserId  and PV.IsActive = 1           
                  
     End                  
    else                   
    Begin                  
     SELECT IUP.[PanVerificationId], IUP.[Name] as [Name], IUP.[DOB], IUP.[PanNumber], IU.EmailId, IUP.FatherName                  
     FROM Identity_User IU WITH(NOLOCK)                  
     LEFT JOIN Identity_PanVerification IUP WITH(NOLOCK) ON IU.UserId = IUP.UserId                  
     WHERE IU.UserId =@UserId   and IU.IsActive = 1   and IUP.IsActive = 1         
     AND DATEDIFF(MONTH, CAST(IUP.PanAddUpdateOn AS DATE), CAST(GETDATE() AS DATE)) <= 6                  
    End                  
                  
    SELECT                   
    TOP(1) IU.UserId, UAD.AddressLine1, UAD.AddressLine2,      
 case when UAD.Pincode = 0 then  NULL   
 --when UAD.Pincode is null then ''   
 else UAD.Pincode end as Pincode,                   
    UAD.CityId, ICity.CityName, UAD.StateId, IState.StateName                  
    FROM Identity_User IU WITH(NOLOCK)                 
    LEFT JOIN Identity_UserAddressDetail UAD WITH(NOLOCK) ON IU.UserId = UAD.UserId                  
    LEFT JOIN Identity_City ICity WITH(NOLOCK) ON ICity.CityId = UAD.CityId                  
    LEFT JOIN Identity_State IState WITH(NOLOCK) ON IState.StateId = UAD.StateId                  
    WHERE IU.UserId = @UserId  and IU.IsActive = 1                 
    ORDER BY UAD.CreatedOn DESC              
             
 --SELECT U.EmpID, U.UserName AS EmployeeName             
 --FROM [HeroInsurance].[dbo].[Admin_UserRoleMapping] (NOLOCK) UM                   
 --INNER JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) U ON UM.UserId = U.UserId                   
 --INNER JOIN [HeroInsurance].[dbo].[Admin_RoleType] (NOLOCK) RT ON UM.RoleTypeID = RT.RoleTypeID                   
 --INNER JOIN [HeroInsurance].[dbo].[Admin_RoleMaster] (NOLOCK) RM on UM.RoleId = RM.RoleId              
 ----INNER JOIN [HeroInsurance].[dbo].[Admin_UserRoleMapping] (NOLOCK) URM on URM.CategoryId = UM.CategoryId            
 --where RT.RoleTypeID='D1E03B48-C313-4BB6-928C-E62C44E1DBDE' and UM.CategoryId = 'Sales'            
                       
                     
 END TRY                  
                  
 BEGIN CATCH                             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                              
  SET @ErrorDetail=ERROR_MESSAGE()                                              
  EXEC dbo.Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                          
 END CATCH                  
END 

