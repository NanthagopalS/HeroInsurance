-- =============================================      
-- Author:  <Author,,AMBI GUPTA>      
-- Create date: <Create Date,25-Nov-2022>      
-- Description: <Description,GetMakeModelFuel>      
-- [Insurance_GetMakeModelFuel] '88a807b3-90e4-484b-b5d2-65059a8e1a91','833E4DC9-A7EE-4C7D-ABBB-C6B993BF428C'      
-- added vechicle category ID condition for CV -- Suraj 13-10-2023    
-- =============================================      
CREATE   PROCEDURE [dbo].[Insurance_GetUnitedIndiaFinancierDetails] (    
 @FinancierCode VARCHAR(50) = NULL   
 )    
AS    
BEGIN    
 BEGIN TRY      
  SET NOCOUNT ON;    
    
  SELECT BranchName AS Name, FinancierBranchCode AS Value
  FROM [MOTOR].[UIIC_FinancierBranchMaster] WITH(NOLOCK)
  WHERE FinancierCode = @FinancierCode
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