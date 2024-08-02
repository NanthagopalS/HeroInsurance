    
CREATE  PROCEDURE [dbo].[Insurance_GetReliancePreviousInsurer]            
	@InsurerId VARCHAR(50) = NULL  
AS            
BEGIN            
 BEGIN TRY         
  SET NOCOUNT ON;    
	SELECT InsuranceCompanyName as InsurerName,InsurerId from  [MOTOR].[Reliance_PreviousInsurerMaster] WITH(NOLOCK)
	WHERE InsuranceCompanyID = @InsurerId 
 END TRY            
 BEGIN CATCH                       
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                    
 END CATCH            
END