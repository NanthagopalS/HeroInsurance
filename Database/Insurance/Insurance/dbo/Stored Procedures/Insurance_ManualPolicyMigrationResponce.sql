/*          
  [dbo].[Insurance_ManualPolicyMigrationResponce] '45EDBC53-507B-4B28-90BD-F0972027CECA'          
*/    
CREATE     PROCEDURE [dbo].[Insurance_ManualPolicyMigrationResponce] (@UserId VARCHAR(100))    
AS    
BEGIN    
 BEGIN TRY    
  SELECT UserEmail    
   ,MotorType    
   ,PolicyType    
   ,PolicyCategory    
   ,BasicOD    
   ,BasicTP    
   ,TotalPremium    
   ,NetPremium    
   ,ServiceTax    
   ,PolicyNo    
   ,EngineNo    
   ,ChasisNo    
   ,VehicleNo    
   ,IDV    
   ,Insurer    
   ,Make    
   ,Fuel    
   ,Variant    
   ,ManufacturingMonth    
   ,CustomerName    
   ,PolicyIssueDate    
   ,PolicyStartDate    
   ,PolicyEndDate    
   ,BusinessType    
   ,NCB    
   ,ChequeNo    
   ,ChequeDate    
   ,ChequeBank    
   ,CustomerEmail    
   ,CustomerMobile    
   ,ManufacturingYear    
   ,PreviousNCB    
   ,CubicCapacity    
   ,RTOCode    
   ,PreviousPolicyNo    
   ,CPA    
   ,[Period]    
   ,InsuranceType    
   ,AddOnPremium    
   ,NilDep    
   ,IsPOSPProduct    
   ,CustomerAddress    
   ,[State]    
   ,City    
   ,PhoneNo    
   ,PinCode    
   ,CustomerDOB    
   ,PANNo    
   ,GrossDiscount    
   ,TotalTP    
   ,GVW    
   ,SeatingCapacity    
   ,UserId    
   ,DumpId    
   ,ValidationCheckPassed    
   ,GeneratedLeadId    
   INTO #Insurance_PolicyDumpTable    
  FROM Insurance_PolicyDumpTable WITH (NOLOCK)    
  WHERE UserId = @UserId    
    
    
  SELECT DumpId    
   ,PolicyId    
   ,ErrorLog    
   ,CreatedOn    
   ,Id    
  INTO #Insurance_ManualPolicyErrorLog    
  FROM Insurance_ManualPolicyErrorLog    
  WHERE DumpId IN (    
    SELECT DumpId    
    FROM #Insurance_PolicyDumpTable )    
  ORDER BY PolicyId    
  
     DELETE FROM  Insurance_ManualPolicyErrorLog WHERE DumpId IN (    
    SELECT DumpId    
    FROM #Insurance_PolicyDumpTable)    
    
   DELETE FROM  Insurance_PolicyDumpTable     
    WHERE DumpId IN (SELECT DumpId FROM #Insurance_PolicyDumpTable)  
    
  DECLARE @TotalRecords int = (select COUNT(*) FROM #Insurance_PolicyDumpTable)    
  DECLARE @FailedRecords int = (select COUNT(*) FROM #Insurance_PolicyDumpTable WHERE GeneratedLeadId IS NULL and ValidationCheckPassed  =0)    
  DECLARE @SuccessRecords int = (select COUNT(*) FROM #Insurance_PolicyDumpTable  WHERE GeneratedLeadId IS NOT NULL)    
  DECLARE @EmailID VARCHAR(max) = (select TOP 1 EmailId FROM HeroIdentity.dbo.Identity_User WHERE UserId=@UserId)    
  DECLARE @UserName VARCHAR(max) = (select TOP 1 UserName FROM HeroIdentity.dbo.Identity_User WHERE UserId=@UserId)    
    
  SELECT * FROM     
  #Insurance_PolicyDumpTable 
    
  SELECT * FROM     
  #Insurance_ManualPolicyErrorLog  ORDER BY PolicyId  
    
  SELECT @TotalRecords as totalpolicy,@FailedRecords as policyfailed,@SuccessRecords as policyuploadedsuccessful,@EmailId AS EmailId,@UserName AS UserName  
  
   INSERT INTO dbo.Insurance_PolicyDumpTableHistory (    
   UserEmail    
   ,MotorType    
   ,PolicyType    
   ,PolicyCategory    
   ,BasicOD    
   ,BasicTP    
   ,TotalPremium    
   ,NetPremium    
   ,ServiceTax    
   ,PolicyNo    
   ,EngineNo    
   ,ChasisNo    
   ,VehicleNo    
   ,IDV    
   ,Insurer    
   ,Make    
   ,Fuel    
   ,Variant    
   ,ManufacturingMonth    
   ,CustomerName    
   ,PolicyIssueDate    
   ,PolicyStartDate    
   ,PolicyEndDate    
   ,BusinessType    
   ,NCB    
   ,ChequeNo    
   ,ChequeDate    
   ,ChequeBank    
   ,CustomerEmail    
   ,CustomerMobile    
   ,ManufacturingYear    
   ,PreviousNCB    
   ,CubicCapacity    
   ,RTOCode    
   ,PreviousPolicyNo    
   ,CPA    
   ,[Period]    
   ,InsuranceType    
   ,AddOnPremium    
   ,NilDep    
   ,IsPOSPProduct    
   ,CustomerAddress    
   ,[STATE]    
   ,City    
   ,PhoneNo    
   ,PinCode    
   ,CustomerDOB    
   ,PANNo    
   ,GrossDiscount    
   ,TotalTP    
   ,GVW    
   ,SeatingCapacity    
   ,UserId    
   ,DumpId    
   ,ValidationCheckPassed    
   ,GeneratedLeadId    
   ) (    
   SELECT UserEmail    
   ,MotorType    
   ,PolicyType    
   ,PolicyCategory    
   ,BasicOD    
   ,BasicTP    
   ,TotalPremium    
   ,NetPremium    
   ,ServiceTax    
   ,PolicyNo    
   ,EngineNo    
   ,ChasisNo       ,VehicleNo    
   ,IDV    
   ,Insurer    
   ,Make    
   ,Fuel    
   ,Variant    
   ,ManufacturingMonth    
   ,CustomerName    
   ,PolicyIssueDate    
   ,PolicyStartDate    
   ,PolicyEndDate    
   ,BusinessType    
   ,NCB    
   ,ChequeNo    
   ,ChequeDate    
   ,ChequeBank    
   ,CustomerEmail    
   ,CustomerMobile    
   ,ManufacturingYear    
   ,PreviousNCB    
   ,CubicCapacity    
   ,RTOCode    
   ,PreviousPolicyNo    
   ,CPA    
   ,[Period]    
   ,InsuranceType    
   ,AddOnPremium    
   ,NilDep    
   ,IsPOSPProduct    
   ,CustomerAddress    
   ,[STATE]    
   ,City    
   ,PhoneNo    
   ,PinCode    
   ,CustomerDOB    
   ,PANNo    
   ,GrossDiscount    
   ,TotalTP    
   ,GVW    
   ,SeatingCapacity       
   ,UserId    
   ,DumpId    
   ,ValidationCheckPassed    
   ,GeneratedLeadId FROM #Insurance_PolicyDumpTable)    
    
  INSERT INTO dbo.Insurance_ManualPolicyErrorLogHistory (    
   DumpId    
   ,ErrorLog    
   ,CreatedOn        
   ,PolicyId    
   ) (    
   SELECT DumpId    
   ,ErrorLog    
   ,CreatedOn       
   ,PolicyId FROM #Insurance_ManualPolicyErrorLog)    
    
   
    DROP TABLE IF EXISTS #Insurance_PolicyDumpTable    
    DROP TABLE IF EXISTS #Insurance_ManualPolicyErrorLog    
    
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