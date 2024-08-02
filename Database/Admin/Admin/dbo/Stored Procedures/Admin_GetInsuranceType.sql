-- =============================================        
-- Author:  <Author, Ankit>        
-- Create date: <Create Date,28-Mar-2022>        
-- Description: <Description,[Admin_GetInsuranceType]>        
--[Admin_RoleType]        
-- =============================================        
CREATE   PROCEDURE [dbo].[Admin_GetInsuranceType]        
AS        
BEGIN        
 BEGIN TRY

	Select InsuranceTypeId, InsuranceName  from [HeroInsurance].[dbo].[Insurance_InsuranceType] WITH(NOLOCK) where IsActive = 1 and isEnable = 1 ORDER BY PriorityIndex ASC

 END TRY                        
 BEGIN CATCH        
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END       
