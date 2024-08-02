  
/*       
EXEC [Insurance_GetManualPolicyList] '','','88a807b3-90e4-484b-b5d2-65059a8e1a91','','2023-07-01','2023-09-01',1,10,'Tata AIG General Insurance ','45BBA540-07C5-4D4C-BEFF-771AE2FC32B0'                 
*/  
CREATE   PROCEDURE [dbo].[Insurance_GetManualPolicyList] (  
 @Product VARCHAR(100) = NULL  
 ,@PolicyType VARCHAR(100) = NULL  
 ,@Moter VARCHAR(100) = NULL  
 ,@PolicySource VARCHAR(100) = NULL  
 ,@StartDate VARCHAR(100) = NULL  
 ,@EndDate VARCHAR(100) = NULL  
 ,@CurrentPageIndex INT = 1  
 ,@CurrentPageSize INT = 10  
 ,@SearchText VARCHAR(100) = NULL  
 ,@CreatedBy VARCHAR(100) = NULL  
 )  
AS  
BEGIN  
 BEGIN TRY  
  DECLARE @IsSuperAdmin BIT = 0  
  DECLARE @UsersByHierarchy TABLE (UserId VARCHAR(100))  
  
  INSERT INTO @UsersByHierarchy (UserId)  
  EXEC [HeroIdentity].[dbo].[Identity_GetUserIdForDataVisibility] @CreatedBy  
  
  SET @IsSuperAdmin = (  
    SELECT CASE   
      WHEN UserId IS NULL  
       THEN 0  
      ELSE 1  
      END AS IsSuperAdmin  
    FROM @UsersByHierarchy  
    WHERE UserId = '0'  
    )  
  
  DECLARE @TempTable TABLE (  
   MotorType VARCHAR(100)  
   ,PolicyType VARCHAR(100)  
   ,PolicyCategory VARCHAR(100)  
   ,BasicOD VARCHAR(100)  
   ,BasicTP VARCHAR(100)  
   ,TotalPremium VARCHAR(100)  
   ,NetPremium VARCHAR(100)  
   ,GST VARCHAR(100)  
   ,PolicyNumber VARCHAR(100)  
   ,EngineNumber VARCHAR(100)  
   ,ChassisNumber VARCHAR(100)  
   ,RegistrationNo VARCHAR(100)  
   ,IDV VARCHAR(100)  
   ,Insurer VARCHAR(100)  
   ,Make VARCHAR(100)  
   ,Fuel VARCHAR(100)  
   ,Variant VARCHAR(100)  
   ,Month VARCHAR(100)  
   ,LeadName VARCHAR(100)  
   ,CreatedOn VARCHAR(100)  
   ,PolicyStartDate VARCHAR(100)  
   ,PolicyEndDate VARCHAR(100)  
   ,BusinessType VARCHAR(100)  
   ,NCBPercentage VARCHAR(100)  
   ,PaymentTxnNumber VARCHAR(100)  
   ,PaymentDate VARCHAR(100)  
   ,PaymentMethod VARCHAR(100)  
   ,Email VARCHAR(100)  
   ,CustomerPhoneNumber VARCHAR(100)  
   ,Year VARCHAR(100)  
   ,PrevPolicyNCB VARCHAR(100)  
   ,CubicCapacity VARCHAR(100)  
   ,RTOCode VARCHAR(100)  
   ,PrevPolicyNumber VARCHAR(100)  
   ,CPA VARCHAR(100)  
   ,Tenure VARCHAR(100)  
   ,InsuranceType VARCHAR(100)  
   ,AddOns VARCHAR(100)  
   ,NilDep VARCHAR(100)  
   ,IsPOSPProduct VARCHAR(100)  
   ,Address1 VARCHAR(1000)  
   ,Address2 VARCHAR(1000)  
   ,Address3 VARCHAR(1000)  
   ,STATE VARCHAR(100)  
   ,City VARCHAR(100)  
   ,PhoneNumber VARCHAR(100)  
   ,PinCode VARCHAR(100)  
   ,DOB VARCHAR(100)  
   ,PANNumber VARCHAR(100)  
   ,GrossDiscount VARCHAR(100)  
   ,GrossPremium VARCHAR(100)  
   ,TotalTP VARCHAR(100)  
   ,GVW VARCHAR(100)  
   ,SeatingCapacity VARCHAR(100)  
   ,CreatedBy VARCHAR(100)  
   )  
  
  INSERT INTO @TempTable  
  SELECT MLD.MotorType  
   ,MPNTM.PolicyNatureTypeName  
   ,MLD.PolicyCategory AS PolicyCategories  
   ,PMD.BasicOD  
   ,PMD.BasicTP  
   ,PMD.TotalTP AS TotalPremium  
   ,PMD.NetPremium  
   ,PMD.GST  
   ,-- Extract from JSON  
   LD.PolicyNumber  
   ,LD.EngineNumber  
   ,LD.ChassisNumber  
   ,LD.VehicleNumber AS RegistrationNo  
   ,LD.IDV  
   ,II.InsurerName AS Insurer  
   ,MLD.Make  
   ,MLD.Fuel  
   ,MLD.Variant  
   ,Month(CAST(LD.MakeMonthYear AS DATE)) AS Month  
   ,-- month  
   LD.LeadName  
   ,PT.CreatedOn  
   ,-- Payment Date Update with satus Policy issue date  
   LD.PolicyStartDate  
   ,LD.PolicyEndDate  
   ,MLD.BusinessType  
   ,LD.NCBPercentage  
   ,MLD.PaymentTxnNumber  
   ,PT.PaymentDate  
   ,MLD.PaymentMethod  
   ,LD.Email  
   ,LD.PhoneNumber AS CustomerPhoneNumber  
   ,YEAR(CAST(LD.MakeMonthYear AS DATE)) AS Year -- Year   
   ,LD.PrevPolicyNCB  
   ,MLD.CubicCapacity  
   ,RTO.RTOCode  
   ,LD.PrevPolicyNumber  
   ,PMD.CPA  
   ,LD.Tenure  
   ,MLD.InsuranceType  
   ,PMD.AddOns  
   ,PMD.NilDep  
   ,MLD.IsPOSPProduct  
   ,LAD.Address1  
   ,LAD.Address2  
   ,LAD.Address3  
   ,LAD.STATE  
   ,LAD.City  
   ,MLD.PhoneNumber  
   ,LAD.PinCode  
   ,LD.DOB  
   ,LD.PANNumber  
   ,PMD.GrossDiscount  
   ,PMD.GrossPremium  
   ,PMD.TotalTP  
   ,MLD.GVW  
   ,MLD.SeatingCapacity  
   ,LD.CreatedBy  
  FROM Insurance_LeadDetails LD WITH (NOLOCK)  
  LEFT JOIN Insurance_PaymentTransaction PT WITH (NOLOCK) ON PT.LeadId = LD.LeadId  
  LEFT JOIN Insurance_PremiumDetails PMD WITH (NOLOCK) ON PMD.LeadId = LD.LeadId  
  LEFT JOIN Insurance_ManualLeadDetails MLD WITH (NOLOCK) ON MLD.LeadId = LD.LeadId  
  LEFT JOIN Insurance_LeadAddressDetails LAD WITH (NOLOCK) ON LAD.LeadId = LD.LeadId  
  LEFT JOIN Insurance_Insurer II WITH (NOLOCK) ON II.InsurerId = LD.InsurerId  
  LEFT JOIN Insurance_RTO RTO WITH (NOLOCK) ON RTO.RTOId = LD.RTOId  
  LEFT JOIN Insurance_ManualPolicyNatureTypeMaster MPNTM WITH (NOLOCK) ON MPNTM.PolicyNatureTypeId = MLD.PolicyTYpe  
  WHERE LD.IsManualPolicy = 1  
   AND CAST(LD.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)  
   AND CAST(LD.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)  
   AND (  
    ISNULL(@SearchText, '') = ''  
    OR (  
     (  
      @Searchtext IS NULL  
      OR @Searchtext = ''  
      )  
     OR (II.InsurerName LIKE '%' + @Searchtext + '%')  
     )  
    OR (  
     (  
      @Searchtext IS NULL  
      OR @Searchtext = ''  
      )  
     OR (LD.PolicyNumber LIKE '%' + @Searchtext + '%')  
     )  
    OR (  
     (  
      @Searchtext IS NULL  
      OR @Searchtext = ''  
      )  
     OR (LD.LeadName LIKE '%' + @Searchtext + '%')  
     )  
    OR (  
     (  
      @Searchtext IS NULL  
      OR @Searchtext = ''  
      )  
     OR (LD.PhoneNumber LIKE '%' + @Searchtext + '%')  
     )  
    OR (  
     (  
      @Searchtext IS NULL  
      OR @Searchtext = ''  
      )  
     OR (LD.VehicleNumber LIKE '%' + @Searchtext + '%')  
     )  
    )  
   AND (  
    ISNULL(@Moter, '') = ''  
    OR LD.VehicleTypeId = @Moter  
    )  
   AND (  
    ISNULL(@PolicyType, '') = ''  
    OR MPNTM.PolicyNatureTypeId = @PolicyType  
    )  
    ORDER BY LD.CreatedOn DESC  
  
  
  DECLARE @TotalRecords INT = (  
    SELECT COUNT(*)  
    FROM @TempTable  
    )  
  
  SELECT @TotalRecords = COUNT(1)  
  FROM @TempTable  
  WHERE CreatedBy IN (  
    SELECT UserId  
    FROM @UsersByHierarchy  
    )  
   OR @IsSuperAdmin = 1  
  
  SELECT *  
   ,@TotalRecords AS TotalRecords  
  FROM @TempTable  
  WHERE CreatedBy IN (  
    SELECT UserId  
    FROM @UsersByHierarchy  
    )  
   OR @IsSuperAdmin = 1  
  ORDER BY CreatedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS  
  
  FETCH NEXT @CurrentPageSize ROWS ONLY  
 END TRY  
  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500)  
   ,@ErrorDetail VARCHAR(1000)  
   ,@ParameterList VARCHAR(2000)  
  
  SET @StrProcedure_Name = ERROR_PROCEDURE()  
  SET @ErrorDetail = ERROR_MESSAGE()  
  
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name  
   ,@ErrorDetail = @ErrorDetail  
   ,@ParameterList = @ParameterList  
 END CATCH  
END