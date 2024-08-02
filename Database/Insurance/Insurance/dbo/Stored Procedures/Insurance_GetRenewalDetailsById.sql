       
CREATE     PROCEDURE [dbo].[Insurance_GetRenewalDetailsById]
(
	@LeadId VARCHAR(100)
)
AS        
BEGIN        
 BEGIN TRY

	Select LD.QuoteTransactionID,LD.InsurerId,LD.PolicyNumber,LD.PhoneNumber,LD.Email,LD.LeadName, IT.InsuranceName, IT.InsuranceType, DATEDIFF(DAY, GETDATE(), PolicyEndDate) as Days, 
	I.InsurerName, M.ModelName, IM.MakeName, V.VariantName
	from [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH(NOLOCK)
	LEFT JOIN [HeroInsurance].[dbo].[Insurance_InsuranceType] IT WITH(NOLOCK) on IT.InsuranceTypeId = LD.VehicleTypeId
	LEFT JOIN [HeroInsurance].[dbo].[Insurance_VehicleType] VT WITH(NOLOCK)  on VT.InsuranceTypeId = LD.VehicleTypeId
	LEFT JOIN [HeroInsurance].[dbo].[Insurance_Insurer] I WITH(NOLOCK) on I.InsurerId = LD.InsurerId
	LEFT JOIN [HeroInsurance].[dbo].[Insurance_Variant] V WITH(NOLOCK) on V.VariantId = LD.VariantId
	LEFT JOIN [HeroInsurance].[dbo].[Insurance_Model] M WITH(NOLOCK) on M.ModelId = V.ModelId
	LEFT JOIN [HeroInsurance].[dbo].[Insurance_Make] IM WITH(NOLOCK) on IM.MakeId = M.MakeId
	where LD.LeadId = @LeadId order by LD.CreatedOn desc

 END TRY                        
 BEGIN CATCH        
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END