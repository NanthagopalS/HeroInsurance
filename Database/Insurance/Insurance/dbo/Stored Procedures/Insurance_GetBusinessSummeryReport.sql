  
--select * from Identity_ErrorDetail order by ErrorDate desc     
/*              
EXEC [Insurance_GetBusinessSummeryReport] 'D636317E-8C77-4D4A-ADCF-7B5624FE96DC','HERO/ENQ/112485','','','2023-10-09','2023-10-10',1,10      
*/  
CREATE  
   
   
 PROCEDURE [dbo].[Insurance_GetBusinessSummeryReport] (  
 @UserId VARCHAR(100) = NULL  
 ,@SearchText VARCHAR(100) = NULL  
 ,@Insurers VARCHAR(MAX) = NULL  
 ,@InsuranceType VARCHAR(MAX) = NULL  
 ,@StartDate VARCHAR(100) = NULL  
 ,@EndDate VARCHAR(100) = NULL  
 ,@CurrentPageIndex INT = 1  
 ,@CurrentPageSize INT = 10  
 )  
AS  
BEGIN  
 BEGIN TRY  
  DROP TABLE  
  
  IF EXISTS #TEMP_BUSINESS_SUMMERY  
   DECLARE @BUSINESS_SUMMERY_REPORT_TEMP AS TABLE (  
    PolicyStatus VARCHAR(50)  
    ,OD_Coverage VARCHAR(50)  
    ,TP_Coverage VARCHAR(50)  
    ,VehicleRegnStatus VARCHAR(50)  
    ,ChassisNo VARCHAR(50)  
    ,EngineNo VARCHAR(50)  
    ,CubicCapacity VARCHAR(50)  
    ,Fuel VARCHAR(50)  
    ,RTOCode VARCHAR(50)  
    ,RTOName VARCHAR(50)  
    ,CurrentNCB VARCHAR(50)  
    ,PreviousNCB VARCHAR(50)  
    ,ODD VARCHAR(50)  
    ,PCV VARCHAR(50)  
    ,OD_PolicyStartDate VARCHAR(50)  
    ,OD_PolicyEndDate VARCHAR(50)  
    ,TP_PolicyStartDate VARCHAR(50)  
    ,TP_PolicyEndDate VARCHAR(50)  
    ,CoverNoteNo VARCHAR(50)  
    ,PolicyNo VARCHAR(50)  
    ,PolicyIssueDate VARCHAR(50)  
    ,PolicyReceiveDate VARCHAR(50)  
    ,BizType VARCHAR(50)  
    ,SumInsured VARCHAR(50)  
    ,ODNetPremium VARCHAR(50)  
    ,TpPrem VARCHAR(50)  
    ,TotalTp VARCHAR(50)  
    ,NetPremium VARCHAR(50)  
    ,GST VARCHAR(MAX)  
    ,GrossPremium VARCHAR(50)  
    ,PrevPolicyNO VARCHAR(50)  
    ,UserName VARCHAR(50)  
    ,Uploadtime VARCHAR(50)  
    ,Product VARCHAR(50)  
    ,POSP_Product VARCHAR(50)  
    ,SeatingCapacity VARCHAR(50)  
    ,CPA VARCHAR(50)  
    ,Discount_Applied VARCHAR(50)  
    ,NillDep VARCHAR(50)  
    ,MotorType VARCHAR(50)  
    ,BusinessFrom VARCHAR(50)  
    ,YearofManf VARCHAR(50)  
    ,Variant VARCHAR(50)  
    ,InsurerName VARCHAR(50)  
    ,POSPName VARCHAR(50)  
    ,POSPCode VARCHAR(50)  
    ,ReferalCode VARCHAR(50)  
    ,ReportingEmail VARCHAR(50)  
    ,RMNAme VARCHAR(50)  
    ,RMCode VARCHAR(50)  
    ,Fax VARCHAR(50)  
    ,PANNumber VARCHAR(50)  
    ,RegNo VARCHAR(50)  
    ,VehicleManufacturerName VARCHAR(100)  
    ,ModelName VARCHAR(50)  
    ,VechicleType VARCHAR(50)  
    ,RegDate VARCHAR(50)  
    ,AddressType VARCHAR(50)  
    ,Address1 VARCHAR(max)  
    ,Address2 VARCHAR(200)  
    ,Address3 VARCHAR(200)  
    ,Pincode VARCHAR(50)  
    ,City VARCHAR(50)  
    ,AState VARCHAR(50)  
    ,Country VARCHAR(50)  
    ,DOB VARCHAR(50)  
    ,Gender VARCHAR(50)  
    ,UserId VARCHAR(50)  
    ,POSPId VARCHAR(50)  
    ,OrderNumber VARCHAR(50)  
    ,CustomerName VARCHAR(50)  
    ,StageValue VARCHAR(50)  
    ,MobileNo VARCHAR(50)  
    ,EmailId VARCHAR(50)  
    ,PolicyType VARCHAR(50)  
    ,PolicyTypeId VARCHAR(50)  
    ,GeneratedOn DATETIME  
    ,ExpiringOn VARCHAR(50)  
    ,InsuranceProduct VARCHAR(50)  
    ,Amount VARCHAR(50)  
    ,PaymentStatus VARCHAR(50)  
    ,CreatedBy VARCHAR(50)  
    ,StageId VARCHAR(50)  
    ,VehicleTypeId VARCHAR(50)  
    ,IsActive VARCHAR(50)  
    ,VehicleType VARCHAR(50)  
    ,InsurerId VARCHAR(max)  
    )  
  DECLARE @IsSuperAdmin BIT = 0  
  DECLARE @UsersByHierarchy TABLE (UserId VARCHAR(100))  
  
  INSERT INTO @UsersByHierarchy (UserId)  
  EXEC [HeroIdentity].[dbo].[Identity_GetUserIdForDataVisibility] @UserId  
  
  SET @IsSuperAdmin = (  
    SELECT CASE   
      WHEN UserId IS NULL  
       THEN 0  
      ELSE 1  
      END AS IsSuperAdmin  
    FROM @UsersByHierarchy  
    WHERE UserId = '0'  
    )  
  SELECT PAYSTATUS.PaymentStatus AS PolicyStatus  
   ,CASE   
    -- 4 wheeler     
    WHEN IL.VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'  
     AND IL.IsBrandNew = 1  
     THEN '1'  
    WHEN IL.VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'  
     AND Il.PolicyTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3'  
     AND IL.IsBrandNew = 0  
     THEN '1'  
    WHEN IL.VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'  
     AND Il.PolicyTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896'  
     AND IL.IsBrandNew = 0  
     THEN '1'  
    WHEN IL.VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'  
     AND Il.PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B'  
     AND IL.IsBrandNew = 0  
     THEN '0'  
      -- 2w    
    WHEN IL.VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'  
     AND IL.IsBrandNew = 1  
     THEN '1'  
    WHEN IL.VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'  
     AND Il.PolicyTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3'  
     AND IL.IsBrandNew = 0  
     THEN '1'  
    WHEN IL.VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'  
     AND Il.PolicyTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896'  
     AND IL.IsBrandNew = 0  
     THEN '1'  
    WHEN IL.VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'  
     AND Il.PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B'  
     AND IL.IsBrandNew = 0  
     THEN '0'  
    END AS OD_Coverage  
   ,CASE   
    -- 4w    
    WHEN IL.VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'  
     AND IL.IsBrandNew = 1  
     THEN '3'  
    WHEN IL.VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'  
     AND Il.PolicyTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3'  
     AND IL.IsBrandNew = 0  
     THEN '1'  
    WHEN IL.VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'  
     AND Il.PolicyTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896'  
     AND IL.IsBrandNew = 0  
     THEN '0'  
    WHEN IL.VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39'  
     AND Il.PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B'  
     AND IL.IsBrandNew = 0  
     THEN '1'  
      -- 2w    
    WHEN IL.VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'  
     AND IL.IsBrandNew = 1  
     THEN '5'  
    WHEN IL.VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'  
     AND Il.PolicyTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3'  
     AND IL.IsBrandNew = 0  
     THEN '1'  
    WHEN IL.VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'  
     AND Il.PolicyTypeId = '48B01586-C66A-4A4A-AAFB-3F07F8A31896'  
     AND IL.IsBrandNew = 0  
     THEN '0'  
    WHEN IL.VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'  
     AND Il.PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B'  
     AND IL.IsBrandNew = 0  
     THEN '1'  
    END AS TP_Coverage  
   ,CASE   
    WHEN IL.IsBrandNew = 1  
     THEN 'N'  
    ELSE 'Y'  
    END AS VehicleRegnStatus  
   ,IL.ChassisNumber AS ChassisNo  
   ,IL.EngineNumber AS EngineNo  
   ,CASE   
    WHEN MLEAD.ManualLeadId IS NULL  
     THEN VEHREG.vehicleCubicCapacity  
    ELSE MLEAD.CubicCapacity  
    END AS CubicCapacity  
   ,CASE   
    WHEN MLEAD.ManualLeadId IS NULL  
     THEN FULE.FuelName  
    ELSE MLEAD.Fuel  
    END AS Fuel  
   ,RTO.RTOCode AS RTOCode  
   ,RTO.RTOName AS RTOName  
   ,convert(VARCHAR(max), IL.NCBPercentage) AS CurrentNCB  
   ,-- (%)       
   convert(VARCHAR(max), NCB.NCBValue) AS PreviousNCB  
   ,-- (%)      
   '--' AS ODD  
   ,--(OD Discount %)      
   '--' AS PCV  
   ,IL.PolicyStartDate AS OD_PolicyStartDate  
   ,CASE   
    WHEN Il.PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B'  
     THEN (GETDATE() - GETDATE())  
    ELSE DATEADD(DAY, - 1, DATEADD(YEAR, 1, IL.PolicyStartDate))  
    END AS OD_PolicyEndDate -- add one year in policy start date if the vechicle is new      
   ,IL.PolicyStartDate AS TP_PolicyStartDate  
   ,IL.PolicyEndDate AS TP_PolicyEndDate  
   ,'--' AS CoverNoteNo  
   ,IL.PolicyNumber AS PolicyNo  
   ,PM.UpdatedOn AS PolicyIssueDate  
   ,-- (Policy Created Date and Time)       
   PM.UpdatedOn AS PolicyReceiveDate  
   ,-- (Policy Created Date and Time)       
   CASE   
    WHEN IL.PolicyEndDate < GETDATE()  
     THEN 'Expired'  
    WHEN IL.IsPrevPolicy = 1  
     THEN 'Renewal'  
    WHEN IL.IsBrandNew = 1  
     THEN 'New'  
    END AS BizType  
   ,IL.IDV AS SumInsured  
   ,--(IDV)       
   '' AS ODNetPremium -- not storing seperately      
   ,'' AS TpPrem -- not storing seperately      
   ,'' AS TotalTp -- not storing seperately      
   ,IL.TotalPremium AS NetPremium  
   ,--(ODNetPremium + TotalTP)       
   CASE   
    WHEN ISJSON(IL.TAX) = 1  
     THEN CAST(JSON_VALUE(IL.Tax, '$.totalTax') AS float)  
    ELSE 0  
    END AS GST  
   ,IL.GrossPremium AS GrossPremium  
   ,--(Net Premium + GST)       
   IL.PrevPolicyNumber AS PrevPolicyNO  
   ,IL.LeadName AS UserName  
   ,MLEAD.CreatedOn AS Uploadtime  
   ,'B2B' AS Product  
   ,'' AS POSP_Product -- generating by POSP or other      
   ,CASE   
    WHEN MLEAD.ManualLeadId IS NULL  
     THEN VARIANT.SeatingCapacity  
    ELSE MLEAD.SeatingCapacity  
    END AS SeatingCapacity  
   ,'NO' AS CPA  
   ,CASE   
    WHEN (  
      ISNULL(PREMIUM.GrossDiscount, '') = ''  
      OR PREMIUM.GrossDiscount = ''  
      )  
     THEN 0  
    ELSE 1  
    END AS Discount_Applied  
   ,'NO' AS NillDep  
   ,INSURETYPE.InsuranceName AS MotorType  
   ,CASE   
    WHEN IL.IsManualPolicy = 1  
     THEN 'Offline'  
    ELSE 'Online'  
    END AS BusinessFrom  
   ,IL.MakeMonthYear AS YearofManf  
   ,CASE   
    WHEN MLEAD.ManualLeadId IS NULL  
     THEN VARIANT.VariantName  
    ELSE MLEAD.Variant  
    END AS Variant  
   ,INSURER.InsurerName AS InsurerName  
   ,IU.UserName AS POSPName  
   ,IU.POSPId AS POSPCode  
   ,'' AS ReferalCode  
   ,RM.EmailId AS ReportingEmail  
   ,RM.UserName AS RMNAme  
   ,RM.EmpID AS RMCode  
   ,'' AS Fax  
   ,IL.PANNumber  
   ,IL.VehicleNumber AS RegNo  
   ,CASE   
    WHEN MLEAD.ManualLeadId IS NULL  
     THEN CAST(MAKE.MakeName AS VARCHAR(100))  
    ELSE CAST(MLEAD.Make AS VARCHAR(100))  
    END AS VehicleManufacturerName  
   ,MODEL.ModelName  
   ,VEHTYPE.VehicleType AS VechicleType  
   ,IL.RegistrationDate AS RegDate  
   ,LEADADDRESSDET.AddressType  
   ,LEADADDRESSDET.Address1  
   ,LEADADDRESSDET.Address2  
   ,LEADADDRESSDET.Address3  
   ,LEADADDRESSDET.Pincode  
   ,LEADADDRESSDET.City AS City  
   ,LEADADDRESSDET.STATE AS AState  
   ,LEADADDRESSDET.Country AS Country  
   ,IL.DOB  
   ,CASE   
    WHEN IL.Gender IS NULL  
     OR IL.Gender = ''  
     THEN ''  
    ELSE CONCAT (  
      UPPER(LEFT(IL.Gender, 1))  
      ,LOWER(RIGHT(IL.Gender, LEN(IL.Gender) - 1))  
      )  
    END AS Gender  
   ,IU.POSPId AS UserId  
   ,IU.POSPId  
   ,IL.LeadId AS OrderNumber  
   ,IL.LeadName AS CustomerName  
   ,SM.Stage AS StageValue  
   ,CASE   
    WHEN @CurrentPageSize != - 1  
     THEN IL.PhoneNumber  
    ELSE LEFT(IL.PhoneNumber, 2) + REPLICATE('*', (LEN(IL.PhoneNumber) - 4)) + RIGHT(IL.PhoneNumber, 2)  
    END AS MobileNo  
   ,CASE   
    WHEN @CurrentPageSize != - 1  
     THEN IL.PhoneNumber  
    ELSE LEFT(IL.Email, 2) + REPLICATE('*', (LEN(IL.Email) - 4)) + RIGHT(IL.Email, 2)  
    END AS EmailId  
   ,INSURETYPE.InsuranceName AS PolicyType  
   ,IL.VehicleTypeId AS PolicyTypeId  
   ,IL.CreatedOn AS GeneratedOn  
   ,CAST(IL.PolicyEndDate AS DATE) AS ExpiringOn  
   ,INSURETYPE.InsuranceType AS InsuranceProduct  
   ,IL.GrossPremium AS Amount  
   ,PAYSTATUS.PaymentStatus AS PaymentStatus  
   ,CASE   
    WHEN IU.POSPId IS NOT NULL  
     THEN IU.POSPId + '-' + IU.UserName  
    WHEN IU.EmpID IS NOT NULL  
     THEN IU.EmpID + '-' + IU.UserName  
    END AS CreatedBy  
   ,IL.StageId  
   ,IL.VehicleTypeId  
   ,IL.IsActive  
   ,VEHTYPE.VehicleType  
   ,convert(VARCHAR(max), IL.InsurerId) AS InsurerId  
  INTO #TEMP_BUSINESS_SUMMERY  
  FROM [Insurance_LeadDetails] IL WITH (NOLOCK)  
  LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK) ON IU.UserId = IL.CreatedBy  
  LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH (NOLOCK) ON UD.UserId = IU.UserId  
  LEFT JOIN [Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageId = IL.StageId  
  LEFT JOIN [Insurance_PaymentTransaction] PM WITH (NOLOCK) ON PM.LeadId = IL.LeadId  
   AND IL.QuoteTransactionID = PM.QuoteTransactionId  
  LEFT JOIN [Insurance_InsuranceType] INSURETYPE WITH (NOLOCK) ON INSURETYPE.InsuranceTypeId = IL.VehicleTypeId  
  LEFT JOIN [Insurance_Insurer] INSURER WITH (NOLOCK) ON INSURER.InsurerId = IL.InsurerId  
  LEFT JOIN [Insurance_LeadAddressDetails] LEADADDRESSDET WITH (NOLOCK) ON IL.LeadId = LEADADDRESSDET.LeadID  
  LEFT JOIN [Insurance_PaymentStatus] PAYSTATUS WITH (NOLOCK) ON PAYSTATUS.PaymentId = PM.[Status]  
  LEFT JOIN [Insurance_Variant] VARIANT WITH (NOLOCK) ON VARIANT.VariantId = IL.VariantId  
  LEFT JOIN [Insurance_VehicleType] VEHTYPE WITH (NOLOCK) ON VEHTYPE.VehicleTypeId = VARIANT.VehicleTypeId  
  LEFT JOIN [Insurance_Model] MODEL WITH (NOLOCK) ON MODEL.ModelId = VARIANT.ModelId  
  LEFT JOIN [Insurance_Make] MAKE WITH (NOLOCK) ON MAKE.MakeId = MODEL.MakeId  
  LEFT JOIN Insurance_Fuel FULE WITH (NOLOCK) ON VARIANT.FuelId = FULE.FuelId  
  LEFT JOIN [HeroIdentity].[dbo].[Identity_User] RM WITH (NOLOCK) ON RM.UserId = UD.ServicedByUserId  
  LEFT JOIN [Insurance_RTO] RTO WITH (NOLOCK) ON RTO.RTOId = IL.RTOId  
  LEFT JOIN Insurance_PremiumDetails PREMIUM WITH (NOLOCK) ON PREMIUM.LeadId = IL.LeadId  
  LEFT JOIN [Insurance_VehicleRegistration] VEHREG WITH (NOLOCK) ON VEHREG.vehicleNumber = IL.VehicleNumber  
  LEFT JOIN [Insurance_NCB] NCB WITH (NOLOCK) ON NCB.NCBId = (  
    CASE   
     WHEN (  
       IL.PrevPolicyNCB = '0'  
       OR IL.PrevPolicyNCB = '50'  
       OR IL.PrevPolicyNCB = '35'  
       OR IL.PrevPolicyNCB = '20'  
       OR IL.PrevPolicyNCB = '45'  
       OR IL.PrevPolicyNCB = ''  
       )  
      THEN NULL  
     ELSE IL.PrevPolicyNCB  
     END  
    )  
  LEFT JOIN [Insurance_ManualLeadDetails] MLEAD WITH (NOLOCK) ON IL.LeadId = MLEAD.LeadId  
  WHERE (  
    CAST(IL.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)  
    OR ISNULL(@StartDate, '') = ''  
    )  
   AND (  
    CAST(IL.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)  
    OR ISNULL(@EndDate, '') = ''  
    )  
   AND PM.[Status] = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'  
   AND IL.IsActive = 1  
   AND (  
    (  
     IL.InsurerId IN (  
      SELECT value  
      FROM STRING_SPLIT(convert(VARCHAR(max), @Insurers), ',')  
      )  
     )  
    OR ISNULL(convert(VARCHAR(max), @Insurers), '') = ''  
    )  
   AND (  
    IL.VehicleTypeId IN (  
     SELECT value  
     FROM STRING_SPLIT(@InsuranceType, ',')  
     )  
    OR ISNULL(@InsuranceType, '') = ''  
    )  
   AND (  
    IL.CreatedBy IN (  
     SELECT UserId  
     FROM @UsersByHierarchy  
     )  
    OR @IsSuperAdmin = 1  
    )   
  INSERT @BUSINESS_SUMMERY_REPORT_TEMP  
  SELECT *  
  FROM #TEMP_BUSINESS_SUMMERY  
  WHERE (  
    (CustomerName LIKE '%' + ISNULL(@SearchText, '') + '%')  
    OR (RegNo LIKE '%' + ISNULL(@SearchText, '') + '%')  
    OR (MobileNo LIKE '%' + ISNULL(@SearchText, '') + '%')  
    OR (EmailId LIKE '%' + ISNULL(@SearchText, '') + '%')  
    OR (OrderNumber LIKE '%' + ISNULL(@SearchText, '') + '%')  
    OR (PolicyNo LIKE '%' + ISNULL(@SearchText, '') + '%')  
    OR ISNULL(@SearchText, '') = ''  
    )  
  ORDER BY GeneratedOn DESC  
  
  DECLARE @TotalRecords INT = (  
    SELECT COUNT(*)  
    FROM @BUSINESS_SUMMERY_REPORT_TEMP  
    )  
  
  IF (@CurrentPageSize = - 1)  
  BEGIN  
   SELECT *  
    ,@TotalRecords AS TotalRecord  
   FROM @BUSINESS_SUMMERY_REPORT_TEMP  
  END  
  ELSE  
  BEGIN  
   SELECT *  
    ,@TotalRecords AS TotalRecord  
   FROM @BUSINESS_SUMMERY_REPORT_TEMP  
   ORDER BY GeneratedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS  
  
   FETCH NEXT @CurrentPageSize ROWS ONLY  
  END  
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