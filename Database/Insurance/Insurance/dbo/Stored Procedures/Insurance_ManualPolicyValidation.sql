/*            
 [dbo].[Insurance_ManualPolicyValidation] '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0'            
*/  
CREATE  
   
   
 PROCEDURE [dbo].[Insurance_ManualPolicyValidation] (@UserId VARCHAR(100))  
AS  
BEGIN  
 DECLARE @TempErrorLog TABLE (  
  DumpId VARCHAR(100)  
  ,ErrorLog VARCHAR(200)  
  ,PolicyId VARCHAR(100)  
  )  
  
 BEGIN TRY  
  SELECT *  
  INTO #Insurance_PolicyDumpTable  
  FROM Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE UserId = @UserId  
   AND GeneratedLeadId IS NULL  
   AND ValidationCheckPassed = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Email ID does not exists'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  LEFT JOIN HeroIdentity.dbo.Identity_User IU WITH (NOLOCK) ON PDT.UserEmail = IU.EmailId  
  WHERE IU.EmailId IS NULL  AND IU.UserProfileStage = 5
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Prev Policy NCB.'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  LEFT JOIN insurance_ncb NCB WITH (NOLOCK) ON PDT.PreviousNCB = NCB.NCBValue  
  WHERE NCB.NCBId IS NULL  
   AND ISNULL(PreviousNCB, '') != ''  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Policy NCB.'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  LEFT JOIN insurance_ncb NCB WITH (NOLOCK) ON PDT.NCB = NCB.NCBValue  
  WHERE NCB.NCBId IS NULL  
   AND ISNULL(NCB, '') != ''  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT PDT.DumpId  
   ,'Duplicate Policy Number'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  INNER JOIN Insurance_LeadDetails ILD WITH (NOLOCK) ON ILD.PolicyNumber = PDT.PolicyNo  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Policy Number'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(PolicyNo, '') = ''  
   OR PolicyNo = ''  OR PolicyNo LIKE '[0-9].[0-9]%[0-9]E+[0-9]%'
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Insurer name'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  LEFT JOIN Insurance_PolicyMigrationExcelInsurerMapping INS WITH (NOLOCK) ON LOWER(PDT.Insurer) = LOWER(INS.excelInsurerName)  
  WHERE INS.DBInsurerId IS NULL  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid RTO Code'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  LEFT JOIN Insurance_RTO RTO WITH (NOLOCK) ON LOWER(REPLACE(PDT.RTOCode, '-', '')) = LOWER(RTO.RTOCode)  
  WHERE RTO.RTOId IS NULL  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Pincode'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  LEFT JOIN HeroIdentity.dbo.Identity_Pincode INS WITH (NOLOCK) ON PDT.PinCode = INS.Pincode  
  WHERE INS.PincodeId IS NULL  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Basic OD'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(BasicOD) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Basic TP'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(BasicTP) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid IDV'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(IDV) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Manufacturing Year'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(ManufacturingYear, '') = ''  
   OR ISNUMERIC(ManufacturingYear) = 0  
   OR TRY_CONVERT(INT, ManufacturingYear) < YEAR(DATEADD(YEAR, - 30, GETDATE()))  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Manufacturing Month'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(ManufacturingMonth, '') = ''  
   OR ISNUMERIC(ManufacturingMonth) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Customer Date of Birth'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(CustomerDOB, '') != ''  
   AND TRY_CONVERT(DATE, CustomerDOB) IS NULL  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid NCB'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(NCB) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid New Premium'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(NetPremium) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Service Tax'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(ServiceTax) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Total Premium'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(TotalPremium) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Add On Premium'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(AddOnPremium) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid CPA'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(CPA) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Gross Discount'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(GrossDiscount) = 0  
   AND ISNULL(GrossDiscount, '') != ''  
   AND GrossDiscount != ''  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Previous NCB name'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(PreviousNCB) = 0  
   AND ISNULL(PreviousNCB, '') != ''  
   AND PreviousNCB != ''  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Period'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(Period) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Total TP name'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNUMERIC(TotalTP) = 0  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Policy Start Date'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(PolicyStartDate, '') = ''  

    INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Policy Start Date should be in valid/format'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(PolicyStartDate, '') != '' 
  AND TRY_CONVERT(DATE, PolicyStartDate) IS NULL 
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid  Policy End Date'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(PolicyEndDate, '') = ''  


      INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Policy End Date should be in valid/format'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(PolicyEndDate, '') != '' 
  AND TRY_CONVERT(DATE, PolicyEndDate) IS NULL 
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid  Policy Issue Date'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(PolicyIssueDate, '') = ''


   INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Policy Issue Date should be in valid/format'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(PolicyIssueDate, '') != '' 
  AND TRY_CONVERT(DATE, PolicyIssueDate) IS NULL 
  
   INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid  Cheque Date'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(ChequeDate, '') != ''
    AND TRY_CONVERT(DATE, ChequeDate) IS NULL 
  
  --INSERT INTO @TempErrorLog (  
  -- DumpId  
  -- ,ErrorLog  
  -- ,PolicyId  
  -- )  
  --SELECT DumpId  
  -- ,'Invalid  State name'  
  -- ,PolicyNo  
  --FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  --LEFT JOIN Insurance_State INS WITH (NOLOCK) ON LOWER(PDT.[State]) = LOWER(INS.StateName)  
  --WHERE INS.StateId IS NULL  
  
  --INSERT INTO @TempErrorLog (  
  -- DumpId  
  -- ,ErrorLog  
  -- ,PolicyId  
  -- )  
  --SELECT DumpId  
  -- ,'Invalid  City name'  
  -- ,PolicyNo  
  --FROM #Insurance_PolicyDumpTable PDT WITH (NOLOCK)  
  --LEFT JOIN Insurance_City INS WITH (NOLOCK) ON LOWER(PDT.[City]) = LOWER(INS.CityName)  
  --WHERE INS.CityId IS NULL  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid  Period/Tenure'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL([Period], '') = ''  
   OR [Period] LIKE '%.%'  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Customer Name'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(CustomerName, '') = ''  
  
  INSERT INTO @TempErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,'Invalid Customer Mobile'  
   ,PolicyNo  
  FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
  WHERE ISNULL(PhoneNo, '') = ''  
  
  INSERT INTO Insurance_ManualPolicyErrorLog (  
   DumpId  
   ,ErrorLog  
   ,PolicyId  
   )  
  SELECT DumpId  
   ,ErrorLog  
   ,PolicyId  
  FROM @TempErrorLog  
  
  UPDATE Insurance_PolicyDumpTable  
  SET ValidationCheckPassed = 1  
   ,GeneratedLeadId = 'HERO/ENQ/' + CAST(NEXT VALUE FOR [dbo].[SEQ_LeadTransactionId] AS VARCHAR(10))  
   ,QuoteTxnId = newid()  
  WHERE DumpId IN (  
    SELECT DumpId  
    FROM #Insurance_PolicyDumpTable WITH (NOLOCK)  
    )  
   AND DumpId NOT IN (  
    SELECT DumpId  
    FROM @TempErrorLog  
    )  
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
GO

