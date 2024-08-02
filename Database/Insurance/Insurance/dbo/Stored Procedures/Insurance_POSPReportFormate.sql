/*      
EXEC [dbo].[Insurance_POSPReportFormate] 'LEAD','','','','48FD2E33-E853-4CF5-B164-902B4AA357D7','','','','2023-05-01','2023-06-05',1,5,0

*/      
      
CREATE    PROCEDURE [dbo].[Insurance_POSPReportFormate]       
(                        
  @ViewLeadsType VARCHAR(100) = 'LEAD',                     
  @UserId VARCHAR(100) = NULL,      -- no idea    
  @SearchText VARCHAR(100) = NULL,                    
  @LeadType VARCHAR(100) = NULL,         -- no idea             
  @POSPId  VARCHAR(100) = NULL,        -- POSP ID (userId of POSP)      
  @PolicyType VARCHAR(100) = NULL,              
  @PreQuote VARCHAR(100) = NULL,             
  @AllStatus VARCHAR(100) = NULL,                  
  @StartDate VARCHAR(100) = NULL,                     
  @EndDate VARCHAR(100) = NULL,                      
  @CurrentPageIndex INT = 1,                      
  @CurrentPageSize INT = 500,    
  @IsFromDashboard BIT = 0    
 )                       
AS                            
                  
BEGIN                            
 BEGIN TRY          
     
 DECLARE @TotalRecord INT = 5    
 DECLARE @LEADVIEWTABLETEMP as TABLE(    
   Education VARCHAR(100),    
   Profession VARCHAR(100),    
   RegNo VARCHAR(100),    
   VehicleManufacturerName VARCHAR(100),    
   Model VARCHAR(100),    
   VechicleType VARCHAR(100),    
   RegDate VARCHAR(20),    
   AddressType VARCHAR(10),    
   Address1 VARCHAR(100),    
   Address2 VARCHAR(100),    
   Address3 VARCHAR(100),    
   Pincode VARCHAR(10),    
   City VARCHAR(100),    
   AState VARCHAR(100),    
   Country VARCHAR(100),    
   DOB DATE,    
   Gender VARCHAR(50),    
   UserId VARCHAR(50),    
   POSPId VARCHAR(50),    
   LeadId VARCHAR(50),    
   CustomerName VARCHAR(50),    
   StageValue VARCHAR(50),    
   MobileNo VARCHAR(50),    
   EmailId VARCHAR(50),    
   PolicyType VARCHAR(50),    
   PolicyTypeId VARCHAR(50),    
   GeneratedOn DATETIME,    
   ExpiringOn DATETIME,    
   Product VARCHAR(50),    
   Amount FLOAT,    
   PolicyStatus VARCHAR(50),    
   PaymentStatus VARCHAR(50),    
   CreatedBy VARCHAR(50),    
   StageId VARCHAR(50),    
   VehicleTypeId VARCHAR(50),    
   IsActive bit,    
   VehicleType VARCHAR(50),
   InsurerId VARCHAR(50),
   QuoteTransactionId VARCHAR(50)
   )    
    
 SELECT     
   ' ' as Education,IBM.BankName,GD.Make,GD.Model,GD.Variant,GD.VehicleType,GD.FuelType,ID.ServicedByUserId,ID.PAN,IU.POSPId,IU.UserName,VEHREG.chassis,VEHREG.engine,VEHREG.vehicleNumber,IL.Profession,VEHREG.regNo as RegNo,VEHREG.vehicleManufacturerName as VehicleManufacturerName,   
   VEHREG.regDate as RegDate,    
   LEADADDRESSDET.AddressType,LEADADDRESSDET.Address1,LEADADDRESSDET.Address2,    
   LEADADDRESSDET.Address3,LEADADDRESSDET.Pincode,CityName, StateName,'' as Country,IL.DOB,    
 CASE WHEN IL.Gender IS NULL OR IL.Gender = '' THEN '' ELSE    
 CONCAT(UPPER(LEFT(IL.Gender,1)),LOWER(RIGHT(IL.Gender,LEN(IL.Gender)-1))) END AS Gender,    
 IU.UserId,IU.POSPId,IL.LeadId,IL.LeadName as CustomerName,SM.Stage as StageValue,                       
    IL.PhoneNumber AS MobileNo,                    
   IL.Email AS EmailId,                       
   INSURETYPE.InsuranceName as PolicyType,    
   IL.VehicleTypeId as PolicyTypeId,    
    IL.CreatedOn as GeneratedOn,      
   IL.PolicyEndDate AS ExpiringOn,                      
   INSURETYPE.InsuranceType as Product,      
   IL.GrossPremium as Amount,   
   CASE WHEN PM.[Status] = (select PaymentId from Insurance_PaymentStatus where PaymentStatus = 'Successfull') THEN 'Issued' ELSE 'Cancelled' END AS PolicyStatus,          
   PAYSTATUS.PaymentStatus as PaymentStatus,      
   CASE WHEN IU.POSPId IS NOT NULL       
     THEN IU.POSPId + '-' + IU.UserName       
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' +       
   IU.UserName END AS CreatedBy,       
   IL.StageId,IL.VehicleTypeId,IL.IsActive, VEHTYPE.VehicleType  ,
   PM.InsurerId as InsurerId,
   PM.QuoteTransactionId as QuoteTransactionId 
     
   FROM [Insurance_LeadDetails] IL WITH(NOLOCK)      
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK) ON IU.UserId = IL.CreatedBy  
   LEFT JOIN  [HeroIdentity].[dbo].[Identity_UserDetail] ID WITH(NOLOCK) ON ID.UserId = ID.PAN
   LEFT JOIN [Insurance_StageMaster] SM WITH(NOLOCK) on SM.StageId = IL.StageId           
   LEFT JOIN [Insurance_PaymentTransaction] PM WITH(NOLOCK) ON PM.LeadId = IL.LeadId      
   LEFT JOIN [Insurance_InsuranceType] INSURETYPE WITH(NOLOCK) ON INSURETYPE.InsuranceTypeId = IL.VehicleTypeId    
   LEFT JOIN [Insurance_LeadAddressDetails] LEADADDRESSDET WITH(NOLOCK) ON IL.LeadId=LEADADDRESSDET.LeadID    
   LEFT JOIN [Insurance_VehicleRegistration] VEHREG WITH(NOLOCK) ON IL.VehicleNumber = VEHREG.regNo    
   LEFT JOIN [Insurance_VehicleType] VEHTYPE WITH(NOLOCK) ON VEHTYPE.VehicleTypeId = IL.VehicleTypeId    
   LEFT JOIN [Insurance_PaymentStatus] PAYSTATUS WITH(NOLOCK) ON PAYSTATUS.PaymentId = PM.[Status] 
   LEFT JOIN [HeroIdentity].[dbo].[Identity_State] IST WITH(NOLOCK) on IST.StateId = IL.LeadId
   LEFT JOIN [HeroInsurance].[dbo].[Insurance_City] ICI WITH(NOLOCK) on ICI.CityId = IL.LeadId
   LEFT JOIN [MOTOR].[GoDigit_VehicleMaster] GD WITH(NOLOCK) on GD.Make = IL.LeadId
   LEFT JOIN  [HeroIdentity].[dbo].[Identity_BankNameMaster] IBM WITH(NOLOCK) on IBM.Id = IL.LeadId
   WHERE (CAST(IL.CreatedOn AS date) >=CAST(@StartDate AS date) OR ISNULL(@StartDate,'')='')    
   AND (CAST(IL.CreatedOn AS date) <=CAST(@EndDate AS date) OR ISNULL(@EndDate,'')='')    
   AND (LOWER(PM.[Status])  = LOWER(@AllStatus) OR ISNULL(@AllStatus,'')='')    
   AND (IL.CreatedBy =@POSPId OR ISNULL(@POSPId,'')='')    
   AND (IL.StageId =@PreQuote OR ISNULL(@PreQuote,'')='')    
   AND IL.IsActive = 1    
   ORDER BY IL.CreatedOn DESC     
     
   END TRY                                        
   BEGIN CATCH                              
 DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                                        
 SET @StrProcedure_Name=ERROR_PROCEDURE()                                                        
 SET @ErrorDetail=ERROR_MESSAGE()                                                        
 EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                                         
   END CATCH                            
          
END