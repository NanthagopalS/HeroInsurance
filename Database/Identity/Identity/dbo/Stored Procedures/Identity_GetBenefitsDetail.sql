  
  
  
-- =============================================  
-- Author:  <Author,,VISHAL KANJARIYA>  
-- Create date: <Create Date,,25-Nov-2022>  
-- Description: <Description,[GetUserDetail]>  
--[Identity_GetUserDetail]  
-- =============================================  
CREATE    PROCEDURE [dbo].[Identity_GetBenefitsDetail]  
  
AS  
BEGIN  
 BEGIN TRY  
  
  SELECT * FROM [dbo].[Identity_BenefitsDetail] WITH(NOLOCK)   WHERE IsActive =1
   
 END TRY                  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  
