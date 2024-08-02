
-- =============================================
-- Author:		<Author,,Harsh Patel >
-- Create date: <Create Date,,15-12-2022>
-- Description:	<Description,,>
-- =============================================
CREATE    PROCEDURE [dbo].[Identity_GetRejectedDocument]   
@UserId VARCHAR(100)
AS
BEGIN
  BEGIN TRY        
  


SELECT  * FROM  [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH(NOLOCK) WHERE  UserId =@UserId AND IsApprove = 0;     
  
 END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH  
END
