  
-- EXEC  [dbo].[Identity_InsertPOSP] 'IM-1303316','POS/HERO/10009'  
  
-- =============================================  
-- Author:  <Author,,Parth Gandhi >  
-- Create date: <Create Date,09/05/2023>  
-- Description: <Description,Identity_InsertPOSP>  
-- =============================================  
CREATE PROCEDURE [dbo].[Identity_InsertPOSP]  
@POSPId VARCHAR(100) = NULL,
@HDFCPospId VARCHAR(20) = NULL
AS  
BEGIN  
   BEGIN TRY    
		UPDATE Identity_User SET [HDFCPospId] = @HDFCPospId, UpdatedOn = GETDATE() WHERE POSPId = @POSPId  
   END TRY    
   BEGIN CATCH    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END