      
/*          
 [dbo].[Insurance_ManualPolicyCreation] '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0'          
        
 [dbo].sp_helptext [Insurance_ManualPolicyValidation] '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0'          
        
*/      
CREATE      PROCEDURE [dbo].[Insurance_ManualPolicyCreation] (@UserId VARCHAR(100))      
AS      
BEGIN      
 BEGIN TRY      
      
  DECLARE @TempDumpTable TABLE(      
   UserEmail VARCHAR(100)      
   ,MotorType VARCHAR(100)      
   ,PolicyType VARCHAR(100)      
   ,PolicyCategory VARCHAR(100)      
   ,BasicOD FLOAT      
   ,BasicTP FLOAT      
   ,TotalPremium FLOAT      
   ,NetPremium FLOAT      
   ,ServiceTax FLOAT      
   ,PolicyNo VARCHAR(100)      
   ,EngineNo VARCHAR(100)      
   ,ChasisNo VARCHAR(100)      
   ,VehicleNo VARCHAR(100)      
   ,IDV INT      
   ,Insurer VARCHAR(100)      
   ,Make VARCHAR(100)      
   ,Fuel VARCHAR(100)      
   ,Variant VARCHAR(100)      
   ,ManufacturingMonth VARCHAR(100)      
   ,CustomerName VARCHAR(100)      
   ,PolicyIssueDate VARCHAR(100)      
   ,PolicyStartDate VARCHAR(100)      
   ,PolicyEndDate VARCHAR(100)      
   ,BusinessType VARCHAR(100)      
   ,NCB FLOAT      
   ,ChequeNo VARCHAR(100)      
   ,ChequeDate VARCHAR(100)      
   ,ChequeBank VARCHAR(100)      
   ,CustomerEmail VARCHAR(100)      
   ,CustomerMobile VARCHAR(100)      
   ,ManufacturingYear VARCHAR(100)      
   ,PreviousNCB FLOAT      
   ,CubicCapacity FLOAT      
   ,RTOCode VARCHAR(10)      
   ,PreviousPolicyNo VARCHAR(100)      
   ,CPA FLOAT      
   ,[Period] FLOAT      
   ,InsuranceType VARCHAR(100)      
   ,AddOnPremium INT      
   ,NilDep VARCHAR(100)      
   ,IsPOSPProduct VARCHAR(100)      
   ,CustomerAddress VARCHAR(max)      
   ,[STATE] VARCHAR(100)      
   ,City VARCHAR(100)      
   ,PhoneNo VARCHAR(100)      
   ,Pincode INT      
   ,CustomerDOB VARCHAR(100)      
   ,PANNo VARCHAR(100)      
   ,GrossDiscount FLOAT      
   ,TotalTP FLOAT      
   ,GVW FLOAT      
   ,SeatingCapacity INT      
   ,DumpId VARCHAR(50)      
   ,GeneratedLeadId VARCHAR(50)      
   ,QuoteTxnId VARCHAR(50)      
  )      
      
 INSERT @TempDumpTable      
 SELECT LOWER(UserEmail) AS UserEmail      
   ,LOWER(MotorType)      
   ,LOWER(PolicyType) AS PolicyType      
   ,PolicyCategory      
   ,CONVERT(FLOAT, BasicOD) AS BasicOD      
   ,CONVERT(FLOAT, BasicTP) AS BasicTP      
   ,CONVERT(FLOAT, TotalPremium) AS TotalPremium      
   ,CONVERT(FLOAT, NetPremium) AS NetPremium      
   ,CONVERT(FLOAT, ServiceTax) AS ServiceTax      
   ,PolicyNo      
   ,EngineNo      
   ,ChasisNo      
   ,VehicleNo      
   ,CONVERT(INT, IDV) AS IDV      
   ,LOWER(Insurer) AS Insurer      
   ,Make      
   ,Fuel      
   ,Variant      
   ,ManufacturingMonth      
   ,CustomerName      
   ,CAST(CONVERT(DATETIME, PolicyIssueDate, 101) AS DATE) AS PolicyIssueDate      
   ,CAST(CONVERT(DATETIME, PolicyStartDate, 101) AS DATE) AS PolicyStartDate      
   ,CAST(CONVERT(DATETIME, PolicyEndDate, 101) AS DATE) AS PolicyEndDate      
   ,BusinessType      
   ,CONVERT(FLOAT, NCB) AS NCB      
   ,ChequeNo      
   ,CAST(CONVERT(DATETIME, ChequeDate, 101) AS DATE) AS ChequeDate      
   ,ChequeBank      
   ,CustomerEmail      
   ,CustomerMobile      
   ,ManufacturingYear      
   ,CONVERT(FLOAT, PreviousNCB) AS PreviousNCB      
   ,CONVERT(FLOAT, CubicCapacity) AS CubicCapacity      
   ,LOWER(REPLACE(RTOCode, '-', ''))      
   ,PreviousPolicyNo      
   ,CONVERT(FLOAT, CPA) AS CPA      
   ,CONVERT(FLOAT, [Period]) AS [Period]      
   ,InsuranceType      
   ,CONVERT(INT, AddOnPremium) AS AddOnPremium      
   ,NilDep      
   ,IsPOSPProduct      
   ,CustomerAddress      
   ,[STATE]      
   ,City      
   ,PhoneNo      
   ,CONVERT(INT, PinCode) AS Pincode      
   ,CAST(CONVERT(DATETIME, CustomerDOB, 101) AS DATE) AS CustomerDOB      
   ,PANNo      
   ,CONVERT(FLOAT, GrossDiscount) AS GrossDiscount      
   ,CONVERT(FLOAT, TotalTP) AS TotalTP      
   ,CONVERT(FLOAT, GVW) AS GVW      
   ,CONVERT(INT, SeatingCapacity) AS SeatingCapacity      
   ,DumpId      
   ,GeneratedLeadId      
   ,QuoteTxnId      
  FROM HeroInsurance.dbo.Insurance_PolicyDumpTable WITH (NOLOCK)      
  WHERE ValidationCheckPassed = 1      
   AND UserId = @UserId      
   AND GeneratedLeadId IS NOT NULL      
      
      
   PRINT('insert into leads')      
       MERGE Insurance_LeadDetails AS LD    USING @TempDumpTable AS TEMP     ON LD.PolicyNumber = TEMP.PolicyNo    WHEN NOT MATCHED BY Target     THEN      INSERT (      [LeadId]      
     ,[VehicleTypeId]      
    ,[VehicleNumber]      
    ,[YearId]      
    ,[LeadName]      
    ,[PhoneNumber]      
    ,[Email]      
    ,[CreatedBy]      
    ,[CreatedOn]  
	,[UpdatedOn]
    ,[MakeMonthYear]      
    ,[IsPrevPolicy]      
    ,[PrevPolicyNumber]      
    ,[PrevPolicyNCB]      
    ,[Tenure]      
    ,[EngineNumber]      
    ,[ChassisNumber]      
    ,[DOB]      
    ,[PANNumber]      
    ,[StageId]      
    ,[IsActive]      
    ,[PolicyStartDate]      
    ,[PolicyEndDate]      
    ,[IsBrandNew]      
    ,[PolicyNumber]      
    ,[TotalPremium]      
    ,[GrossPremium]      
    ,[Tax]      
    ,[NCBPercentage]      
    ,[InsurerId]      
    ,[IDV]      
    ,[isPolicyExpired]      
    ,[RTOId]      
    ,[IsManualPolicy]      
    ,[QuoteTransactionID]      
    )      
   VALUES (      
    GeneratedLeadId      
    ,(      
     CASE       
      WHEN MotorType = 'privatecar'      
       THEN '2d566966-5525-4ed7-bd90-bb39e8418f39'      
      WHEN MotorType = 'twowheeler'      
       THEN '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'      
      ELSE '88a807b3-90e4-484b-b5d2-65059a8e1a91'      
      END      
     )      
    ,TEMP.VehicleNo      
    ,(      
     SELECT TOP 1 YearId      
     FROM HeroInsurance.dbo.Insurance_Year WITH (NOLOCK)      
     WHERE [Year] = TEMP.ManufacturingYear      
     )      
    ,TEMP.CustomerName      
    ,TEMP.CustomerMobile      
    ,TEMP.CustomerEmail      
    ,(      
     SELECT TOP 1 UserId      
     FROM HeroIdentity.dbo.Identity_User WITH (NOLOCK)      
     WHERE LOWER(EmailId) = TEMP.UserEmail      
     )      
    ,CAST(TEMP.PolicyIssueDate AS DATE)
	,CAST(TEMP.PolicyIssueDate AS DATE)
    ,(TEMP.ManufacturingYear + '-' + TEMP.ManufacturingMonth + '-01')      
    ,(      
     CASE       
      WHEN TEMP.PreviousPolicyNo IS NOT NULL      
       THEN 1      
      ELSE 0      
      END      
     )      
    ,TEMP.PreviousPolicyNo      
    ,TEMP.PreviousNCB      
    ,TEMP.[Period]      
    ,TEMP.EngineNo      
    ,TEMP.ChasisNo      
    ,TEMP.CustomerDOB      
    ,TEMP.PANNo      
    ,'D07216AF-ACAD-4EEA-8CFF-2910BA77E5EE'      
    ,1      
    ,CAST(TEMP.PolicyStartDate AS DATE)      
    ,CAST(TEMP.PolicyEndDate AS DATE)
    ,(      
     CASE       
      WHEN TEMP.PolicyType = 'new'      
       THEN 1      
      ELSE 0      
      END      
     )      
    ,TEMP.PolicyNo      
    ,CONVERT(VARCHAR, TEMP.TotalPremium)      
    ,CONVERT(VARCHAR, TEMP.TotalPremium + TEMP.ServiceTax)      
    ,'{"cgst":null,"sgst":null,"igst":null,"utgst":null,"totalTax":"' + CONVERT(VARCHAR, TEMP.ServiceTax) + '","taxType":null}'      
    ,TEMP.NCB      
    ,(    
  SELECT TOP 1 DBInsurerId      
     FROM Insurance_PolicyMigrationExcelInsurerMapping      
     WHERE LOWER(excelInsurerName) = LOWER(TEMP.Insurer)      
     )      
    ,TEMP.IDV      
    ,0      
    ,(      
     SELECT TOP 1 RTOId      
     FROM Insurance_RTO      
     WHERE LOWER(RTOCode) = TEMP.RTOCode      
     )      
    ,1      
    ,QuoteTxnId      
    );      
      
   PRINT ('lead payment')      
   INSERT INTO [dbo].[Insurance_PaymentTransaction] (      
    [QuoteTransactionId]      
    ,[InsurerId]      
    ,[Status]      
    ,[CreatedBy]      
    ,[LeadId]      
    ,[Amount]      
    ,[PolicyNumber]      
    ,[PaymentDate]   
	,[UpdatedOn]
    )      
   SELECT      
    QuoteTxnId      
    ,(    
  SELECT TOP 1 DBInsurerId      
     FROM Insurance_PolicyMigrationExcelInsurerMapping      
     WHERE LOWER(excelInsurerName) = LOWER(Insurer)      
     )      
    ,'A25D747B-167E-4C1B-AE13-E6CC49A195F8'      
    ,(      
     SELECT TOP 1 UserId      
     FROM HeroIdentity.dbo.Identity_User WITH (NOLOCK)      
     WHERE LOWER(EmailId) = UserEmail      
     )      
    ,GeneratedLeadId      
    ,CONVERT(DECIMAL, TotalPremium + ServiceTax)      
    ,PolicyNo      
    ,CAST(ChequeDate AS DATE)  
	,CAST(PolicyIssueDate AS DATE)
    FROM @TempDumpTable      
      
   PRINT ('manual lead details')      
     
   INSERT INTO [dbo].[Insurance_ManualLeadDetails] (      
    LeadId      
    ,MotorType      
    ,PolicyType      
    ,ServiceTax      
    ,Make      
    ,Fuel      
    ,Variant      
    ,BusinessType      
    ,CubicCapacity      
    ,InsuranceType      
    ,IsPOSPProduct      
    ,GVW      
    ,SeatingCapacity      
    ,PolicyCategory      
    ,CreatedOn      
    ,IsActive      
    ,PaymentMethod      
    ,PaymentTxnNumber      
    ,paymentMode      
    )      
   SELECT      
    GeneratedLeadId      
    ,MotorType      
    ,(SELECT TOP 1 PolicyNatureTypeId FROM [dbo].[Insurance_ManualPolicyNatureTypeMaster]      
    WHERE REPLACE(LOWER(PolicyNatureTypeName),' ','') = PolicyType)      
    ,ServiceTax      
    ,Make      
    ,Fuel      
    ,Variant      
    ,BusinessType      
    ,CubicCapacity      
    ,InsuranceType      
    ,IsPOSPProduct      
    ,GVW      
    ,SeatingCapacity      
    ,PolicyCategory      
    ,GETDATE()      
    ,1      
    ,'Cheque'      
    ,ChequeNo      
    ,ChequeBank      
    FROM @TempDumpTable      
      
   PRINT('address')      
      INSERT INTO [dbo].[Insurance_LeadAddressDetails] (      
    [LeadID]      
    ,[Address1]      
    ,[Pincode]      
    ,[CreatedBy]      
    ,[City]      
    ,[State]      
    ,[Country]      
    )      
   SELECT GeneratedLeadId,      
   CustomerAddress      
    ,PinCode      
    ,(      
     SELECT TOP 1 UserId      
     FROM HeroIdentity.dbo.Identity_User WITH (NOLOCK)      
     WHERE LOWER(EmailId) = UserEmail      
     )      
    ,City      
    ,(      
     SELECT TOP 1 StateName      
     FROM HeroIdentity.dbo.Identity_Pincode      
     WHERE Pincode = PinCode      
     )      
    ,'INDIA'      
    FROM @TempDumpTable      
      
   PRINT('premium details')      
   INSERT INTO [dbo].[Insurance_PremiumDetails] (      
    [LeadId]      
    ,[InsurerId]      
    ,[AddOns]      
    ,[NilDep]      
    ,[CPA]      
    ,[BasicOD]      
    ,[BasicTP]      
    ,[TotalTP]      
    ,[NetPremium]      
    ,[GST]      
    ,[GrossPremium]      
    ,[GrossDiscount]      
    ,[CreatedBy]      
    )      
  SELECT      
    GeneratedLeadId      
    ,(    
  SELECT TOP 1 DBInsurerId      
     FROM Insurance_PolicyMigrationExcelInsurerMapping      
     WHERE LOWER(excelInsurerName) = LOWER(Insurer)      
     )     
    ,AddOnPremium      
    ,NilDep      
    ,CPA      
    ,BasicOD      
    ,BasicTP      
    ,TotalTP      
    ,TotalPremium      
    ,ServiceTax      
    ,(TotalPremium + ServiceTax)      
    ,GrossDiscount      
    ,(      
     SELECT TOP 1 UserId      
     FROM HeroIdentity.dbo.Identity_User WITH (NOLOCK)      
     WHERE LOWER(EmailId) = UserEmail      
     )      
    FROM @TempDumpTable      
      
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