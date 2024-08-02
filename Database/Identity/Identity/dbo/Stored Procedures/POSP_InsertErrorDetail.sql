  
-- =============================================  
-- Author:  <Author,Harsh Patel>  
-- Create date: <Create Date,,26-12-2022>  
-- Description: <Description,InsertErrorDetail>  
-- =============================================  
CREATE    PROCEDURE [dbo].[POSP_InsertErrorDetail]  
(  
 @StrProcedure_Name varchar(500),   
 @ErrorDetail varchar(1000),   
 @ParameterList varchar(2000)  
)  
AS  
BEGIN  
BEGIN TRAN  
 BEGIN TRY  
  INSERT INTO POSP_ErrorDetail  
  (  
   StrProcedure_Name, ErrorDetail, ParameterList, ErrorDate  
  )  
  VALUES  
  (  
   @StrProcedure_Name, @ErrorDetail, @ParameterList, GETDATE()  
  )  
  COMMIT TRAN  
 END TRY  
BEGIN CATCH  
 ROLLBACK TRAN  
END CATCH  
END  
