CREATE      PROCEDURE [dbo].[Insurance_GetParticularLeadDetailById]             
(        
 @LeadId VARCHAR(100)  = NULL    
 )       
AS            
       
BEGIN            
 BEGIN TRY      
      
 --Personal Detail      
 SELECT ' ' as AlternateMobileNumber,' ' as Income,NOMINEE.FirstName as NFirstName,NOMINEE.LastName as NLastName,LD.MaritalStatus,      
 LD.LeadId, LD.LeadName, LD.PhoneNumber, LD.Email,       
 CONCAT(UPPER(LEFT(LD.Gender,1)),LOWER(RIGHT(LD.Gender,LEN(LD.Gender)-1)))AS Gender,       
 LD.DOB, LD.PANNumber AS PANNumber, LD.AadharNumber AS AadharNumber,      
  CONCAT(LAD.Address1, ' ', LAD.Address2) as Address, '' as Education,      
  LD.Profession,LAD.AddressType    
  ,LD.PolicyNumber    
   ,LD.PolicyStartDate    
   ,LD.PolicyEndDate    
   ,LD.TotalPremium    
   ,INSURER.InsurerName    
   ,CASE     
    WHEN LD.PolicyEndDate < GETDATE()    
     THEN 'Expired'    
    WHEN LD.IsPrevPolicy = 1    
     THEN 'Renewal'    
    WHEN LD.IsBrandNew = 1    
     THEN 'NEW'    
    END AS PolicyType    
 FROM [Insurance_LeadDetails] LD WITH(NOLOCK)       
 LEFT JOIN [Insurance_LeadAddressDetails] LAD WITH(NOLOCK) on LAD.LeadID = LD.LeadId      
 LEFT JOIN [Insurance_LeadNomineeDetails] NOMINEE WITH(NOLOCK) on NOMINEE.LeadID=LD.LeadId     
 LEFT JOIN [Insurance_Insurer] INSURER WITH (NOLOCK) ON INSURER.InsurerId = LD.InsurerId    
 WHERE LD.LeadId = @LeadId      
       
      
 --Vehicle detail      
      
 SELECT LEADDETAILS.VehicleNumber    
   ,VEHREG.regDate    
   ,MAKE.MakeName AS Maker    
   ,MODEL.ModelName AS Model    
   ,LEADDETAILS.ChassisNumber    
   ,LEADDETAILS.EngineNumber    
   ,II.InsurerName    
   ,LEADDETAILS.PolicyNumber    
   ,LEADDETAILS.PolicyStartDate    
   ,LEADDETAILS.PolicyEndDate    
   ,LEADDETAILS.TotalPremium    
   ,VEHTYPE.VehicleType    
   ,VARIANT.VariantName    
   ,LEADDETAILS.VehicleNumber AS regNo    
   ,VEHREG.[owner] AS AccountHolder    
   ,'' AS PolicySource    
   ,CASE     
    WHEN LEADDETAILS.PolicyEndDate < GETDATE()    
     THEN 'Expired'    
    WHEN LEADDETAILS.IsPrevPolicy = 1    
     THEN 'Renewal'    
    WHEN LEADDETAILS.IsBrandNew = 1    
     THEN 'NEW'    
    END AS PolicyType    
  FROM [Insurance_LeadDetails] LEADDETAILS WITH (NOLOCK)    
  LEFT JOIN [Insurance_Insurer] II WITH (NOLOCK) ON II.InsurerId = LEADDETAILS.InsurerId    
  LEFT JOIN [Insurance_Variant] VARIANT WITH (NOLOCK) ON LEADDETAILS.VariantId = VARIANT.VariantId    
  LEFT JOIN [Insurance_Model] MODEL WITH (NOLOCK) ON MODEL.ModelId = VARIANT.ModelId    
  LEFT JOIN [Insurance_Make] MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId    
  LEFT JOIN [Insurance_VehicleRegistration] VEHREG WITH (NOLOCK) ON LEADDETAILS.VehicleNumber = VEHREG.regNo    
  LEFT JOIN [Insurance_VehicleType] VEHTYPE WITH (NOLOCK) ON VEHTYPE.InsuranceTypeId = LEADDETAILS.VehicleTypeId    
  WHERE LEADDETAILS.LeadId = @LeadId    
  ORDER BY LEADDETAILS.CreatedOn DESC     
      
 END TRY                            
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
         
END