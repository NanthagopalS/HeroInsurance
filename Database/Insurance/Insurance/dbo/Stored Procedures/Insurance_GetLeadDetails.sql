--[dbo].[Insurance_GetLeadDetails] 'HERO/ENQ/114736','A69719FC-AB1B-4BA5-87D0-EDB9024A93E7'          
--------------------------------------------------------------------------------------------------------------------------------------------          
--    Modified By      Modified Date       Purpose      
--------------------------------------------------------------------------------------------------------------------------------------------      
--  Parth Gandhi     07,Sept-2023       User Story 4126 (Generic IC Wise Key Highlights) Added Column in Insurer Table      
--  Parth Gandhi     19,Sept-2023       Create table Insurance_KeyHighlightsDescription & Use Key Highlights desc       
--  Firoz S    02,Nov-2023        Need to return MinIDV, MaxIDV, RecommendedIDV and SelectedIDV as per Insurer    
--  Madhu G    15,Dec-2023        Need to return Lead Previous Cover Details  
--  Nanthagopal 19-DEC-2023       Commercial Vehicle Details Need to fetch incase of commercial
--------------------------------------------------------------------------------------------------------------------------------------------      
CREATE   PROCEDURE [dbo].[Insurance_GetLeadDetails]                       
@LeadId VARCHAR(50) = NULL,                  
@StageId VARCHAR(50) = NULL                  
AS                        
BEGIN                        
 BEGIN TRY                        
                    
  DECLARE @COMMONRESPONSE NVARCHAR(MAX), @INSURERID VARCHAR(50), @CashlessGarageCount VARCHAR(10), @Logo VARCHAR(100), @PrevPolicyNCBPercentage VARCHAR(50), @STAGE VARCHAR(50),@InsurerName VARCHAR(100),@PolicyNumber VARCHAR(50),                   
  @CurrentNCBId VARCHAR(100), @PrevPolicyNCBId VARCHAR(100), @Year VARCHAR(50), @YearId Varchar(50), @IsSAODDateMandatory BIT, @IsSATPDateMandatory BIT, @TransactionId VARCHAR(50), @PolicyTypeId VARCHAR(50), @IsBrandNew BIT, @SelfVideoClaims NVARCHAR(MAX)
  
    
, @RequestBody NVARCHAR(MAX) ,@SelfDescription NVARCHAR(MAX), @GarageDescription NVARCHAR(MAX), @MaxIDV VARCHAR(50),     
@MinIDV VARCHAR(50), @RecommendedIDV VARCHAR(50), @SelectedIDV VARCHAR(10), @VehicleTypeId VARCHAR(50),
@PCVVehicleCatagoryId VARCHAR(50) = NULL,
@UsageTypeId VARCHAR(50) = NULL,
@UsageNatureId VARCHAR(50) = NULL,
@IsTrailer VARCHAR(10) = NULL,
@IsHazardous VARCHAR(10) = NULL,
@BodyTypeId VARCHAR(50) = NULL,
@CarrierTypeId VARCHAR(50) = NULL,
@SubCatagoryId VARCHAR(50) = NULL,
@CatagoryId VARCHAR(50) = NULL
                  
  SELECT @YearId = YearId, @PolicyTypeId = PolicyTypeId FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId=@LeadId                  
               
  IF(@YearId='0' OR @YearId IS NULL OR @YearId = '')                  
  BEGIN                  
   SELECT @Year= YEAR(GETDATE())                  
  END                  
  ELSE                  
  BEGIN                  
   SELECT @Year = Year FROM Insurance_Year WITH(NOLOCK) WHERE YearId = @YearId                  
  END                         
  SELECT @INSURERID=InsurerId, @IsBrandNew = IsBrandNew, @VehicleTypeId = VehicleTypeId                    
  FROM Insurance_LeadDetails WITH(NOLOCK)                  
  WHERE LeadId=@LeadId                   
                 
  IF(@IsBrandNew = 1)                
  BEGIN                
  SELECT @IsSATPDateMandatory = 0, @IsSAODDateMandatory = 0                
  END                
  ELSE                
  BEGIN                
    IF(@PolicyTypeId='517D8F9C-F532-4D45-8034-ABECE46693E3')                  
 BEGIN                  
    SELECT @IsSAODDateMandatory = 1 , @IsSATPDateMandatory = 0                  
 END                  
 ELSE IF(@PolicyTypeId='2AA7FDCA-9E36-4A8D-9583-15ADA737574B')                  
 BEGIN                  
    SELECT @IsSATPDateMandatory = 1, @IsSAODDateMandatory = 0                  
 END                  
 ELSE                  
 BEGIN                  
    SELECT @IsSAODDateMandatory = 1, @IsSATPDateMandatory = 1                  
 END                 
  END                
                 
  IF @INSURERID = '78190CB2-B325-4764-9BD9-5B9806E99621'                  
  BEGIN                  
   SELECT @PolicyNumber=PolicyNumber                  
   FROM Insurance_LeadDetails WITH(NOLOCK)                  
   WHERE LeadId=@LeadId                   
  END                  
  ELSE                  
  BEGIN                  
   SELECT @PolicyNumber=BreakinId                  
   FROM Insurance_LeadDetails WITH(NOLOCK)                  
   WHERE LeadId=@LeadId                   
  END         
           
  SELECT TOP 1 @COMMONRESPONSE=CommonResponse, @TransactionId = QuoteTransactionId,@MaxIDV = MaxIDV, @MinIDV = MinIDV,    
  @RecommendedIDV = RecommendedIDV, @SelectedIDV = SelectedIDV    
  FROM Insurance_QuoteTransaction WITH(NOLOCK)                  
  WHERE LeadId=@LeadId                   
  AND StageID='A69719FC-AB1B-4BA5-87D0-EDB9024A93E7'                  
  AND INSURERID=@INSURERID order by CreatedOn desc-- THIS WILL TAKE COMMON RESPONSE AND QUOTETANSACTIONID WHICH WE ARE SENDING IN QUOTE LISTING                  
      
  IF(@StageId = 'A69719FC-AB1B-4BA5-87D0-EDB9024A93E7')-- Need to take selectedIDV from leaddetails when stage is quote other wise need to take from IC    
  BEGIN    
 SELECT @SelectedIDV=SelectedIDV                  
 FROM Insurance_LeadDetails WITH(NOLOCK)                  
 WHERE LeadId=@LeadId       
  END    
      
  SELECT @CashlessGarageCount = COUNT(*) FROM Insurance_CashlessGarage WITH(NOLOCK) WHERE InsurerId=@INSURERID            
  IF(@IsBrandNew = 1)                
  BEGIN        
  SELECT @Logo = Logo,@InsurerName=InsurerName, @SelfVideoClaims = SelfVideoClaims,@SelfDescription = SelfDescription,@GarageDescription = GarageDescription       
  FROM Insurance_Insurer WITH(NOLOCK) WHERE InsurerId=@INSURERID       
  END      
  ELSE      
  BEGIN      
  SELECT @Logo = INS.Logo,@InsurerName=INS.InsurerName,@SelfVideoClaims = KHD.SelfVideoClaims,@SelfDescription = KHD.SelfDescription,      
  @GarageDescription = KHD.GarageDescription     
  FROM Insurance_Insurer INS WITH(NOLOCK)      
  JOIN Insurance_KeyHighlightsDescription KHD WITH(NOLOCK) ON KHD.InsurerId = INS.InsurerId      
  LEFT JOIN Insurance_LeadDetails IL WITH(NOLOCK) ON IL.LeadId = @LeadId      
  WHERE INS.InsurerId = @INSURERID AND KHD.PolicyTypeId = @PolicyTypeId AND KHD.VehicleTypeId = IL.VehicleTypeId      
 END      
                     
  SELECT @CurrentNCBId = NCBId FROM Insurance_NCB WITH(NOLOCK) WHERE NCBValue = (SELECT NCBPercentage FROM Insurance_LeadDetails                   
  WITH(NOLOCK) WHERE LeadId = @LeadId AND StageId = @StageId)                  
                 
  SELECT @PrevPolicyNCBPercentage = NCBValue FROM Insurance_NCB WITH(NOLOCK) WHERE NCBId = (SELECT PrevPolicyNCB FROM Insurance_LeadDetails                   
  WITH(NOLOCK) WHERE LeadId = @LeadId AND StageId = @StageId)                  
          
  SELECT @RequestBody = RequestBody FROM Tbl_QuotationRequest WHERE LeadId = @LeadId   
  
  IF(@VehicleTypeId = '88a807b3-90e4-484b-b5d2-65059a8e1a91')
  BEGIN
	SELECT
	@PCVVehicleCatagoryId = PCVVehicleCategoryId,
	@UsageTypeId = UsageTypeId,
	@UsageNatureId = UsageNatureId,
	@IsTrailer = IsTrailer,
	@IsHazardous = IsHazardousVehicleUse,
	@BodyTypeId = VehicleBodyId,
	@CarrierTypeId = CarrierTypeId,
	@SubCatagoryId = VehicleSubCategoryId,
	@CatagoryId = VehicleCategoryId
	FROM Insurance_CommercialLeadDetail 
	WHERE LeadId = @LeadId
  END
        
  SELECT VehicleNumber,InsurerId,                  
  MakeMonthYear,                  
  LD.VehicleTypeId,                  
  RegistrationDate,                  
  LeadName,                  
  IsPrevPolicy,                  
  PrevPolicyNumber,                  
  PrevPolicyExpiryDate,                  
  PrevPolicyClaims,                  
  PrevPolicyNCB,                  
  PreviousPolicyNumberSAOD,                  
  PreviousPolicyExpirySAOD,                  
  IsPACover,                  
  Tenure,                  
  PolicyTypeId,                  
  QuoteTransactionID,                  
  CompanyName,                  
  TotalPremium,                  
  GrossPremium,                  
  Tax,                  
  NCBPercentage,                  
  GSTNo,                  
  DOB,                  
  DateOfIncorporation,                  
  InsurerId,                  
  LD.VariantId,                  
  @RecommendedIDV IDV,                  
  @MinIDV MinIDV,                  
  @MaxIDV MaxIDV,                  
  VARIANT.VariantName,                   
  MODEL.ModelName,                  
  MAKE.MakeName,                  
  FUEL.FuelName,                  
  @Year RegistrationYear,                  
  @COMMONRESPONSE CommonResponse,             
  @RequestBody QuoteRequest,        
  @TransactionId TransactionId,                  
  @IsSAODDateMandatory IsSAODDateMandatory,                  
  @IsSATPDateMandatory IsSATPDateMandatory,                  
  @CashlessGarageCount CashlessGarageCount,                  
  @Logo Logo,   
  @PrevPolicyNCBPercentage PrevPolicyNCBPercentage,                  
  PrevPolicyNCB,                  
  IsBreakinApproved,                  
  @InsurerName InsurerName,                  
  @CurrentNCBId CurrentNCBId,                  
  isPolicyExpired,                  
  IsBrandNew,                  
  @SelectedIDV SelectedIDV,                  
  RTOId,                  
  PreviousSAODInsurer,                  
  PreviousSATPInsurer,                  
  VARIANT.CubicCapacity,                  
  LD.PaymentLink AS PaymentURL,                
  LD.PANNumber PanNumber,              
  LD.PhoneNumber,              
  LD.Email,              
  LD.LeadName FirstName,              
  LD.LastName LastName,              
  LD.CarOwnedBy CustomerType,
  CASE WHEN (IsBreakin=1 AND IsBreakinApproved=0) THEN 'Failed'                  
    WHEN (IsBreakin=1 AND IsBreakinApproved=1) THEN 'Completed'                  
    WHEN (IsBreakin=1 AND IsBreakinApproved IS NULL) THEN 'InProgress' END AS BreakinStatus,                  
  CASE WHEN (IsBreakin=1 AND IsBreakinApproved=0) THEN 'Vehicle Inspection is Not Successful with ,' + @InsurerName + '. Inspection ID: ' + @PolicyNumber + ' Please retry with other insurer or reach out to us for assistance.'                  
    WHEN (IsBreakin=1 AND IsBreakinApproved=1) THEN 'Vehicle Inspection is Completed Successfully with ' + @InsurerName + '. Inspection ID: ' + @PolicyNumber + '.'                  
    WHEN (IsBreakin=1 AND IsBreakinApproved IS NULL) THEN 'Vehicle Inspection is In Progress,' + @InsurerName + ' team will reach out for conducting inspection. Inspection ID: ' + @PolicyNumber + ' Please save it for future reference.'              
 END AS BreakInMessage,                
 LD.CarOwnedBy AS CustomerType,              
 MODEL.MakeId AS BrandId,              
 VARIANT.ModelId AS ModelId,              
 VARIANT.FuelId AS FuelId,            
 @SelfVideoClaims as SelfVideoClaims,            
 @SelfDescription as SelfDescription,            
 @GarageDescription as GarageDescription,
 @PCVVehicleCatagoryId AS PCVVehicleCatagoryId,
  @UsageTypeId AS UsageTypeId,
  @UsageNatureId AS UsageNatureId,
  @IsTrailer AS IsTrailer,
  @IsHazardous AS IsHazardous,
  @BodyTypeId AS BodyTypeId,
  @CarrierTypeId AS CarrierTypeId,
  @SubCatagoryId AS SubCatagoryId,
  @CatagoryId AS CatagoryId
  FROM Insurance_LeadDetails LD WITH(NOLOCK)                   
  LEFT JOIN Insurance_Variant VARIANT WITH(NOLOCK) ON LD.VariantId=VARIANT.VariantId                  
  LEFT JOIN Insurance_Model MODEL WITH(NOLOCK) ON VARIANT.ModelId=MODEL.ModelId                  
  LEFT JOIN Insurance_Make MAKE WITH(NOLOCK) ON MODEL.MakeId=MAKE.MakeId                  
  LEFT JOIN Insurance_Fuel FUEL WITH(NOLOCK) ON VARIANT.FuelId=FUEL.FuelId                  
  LEFT JOIN Insurance_NCB NCB WITH(NOLOCK) ON LD.PrevPolicyNCB = NCB.NCBId                  
  WHERE LeadId = @LeadId  AND StageId = @StageId             
  SELECT   
    l.CoverId,   
    g.CoverFlag,  
    l.IsChecked FROM dbo.Insurance_LeadPreviousCoverDetails l  
INNER JOIN [dbo].[Insurance_GetPreviousCoverMaster] g ON l.CoverId = g.CoverId  
WHERE l.LeadId = @LeadId  
GROUP BY l.LeadId, l.CoverId,g.CoverFlag,l.IsChecked;  
 END TRY                        
 BEGIN CATCH                                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                                    
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                   
           
 END CATCH                        
END 
GO