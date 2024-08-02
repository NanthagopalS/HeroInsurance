
-- =============================================
-- Author:		<Author,Vishal >
-- Create date: <Create Date,10/03/2023>
-- Description:	<Description,Admin_GetPOSPStageDetail>
-- =============================================
CREATE   PROCEDURE [dbo].[Admin_GetPOSPStageDetail] 
AS
BEGIN
 BEGIN TRY        
	
	SELECT StageId, StageName, PriorityIndex, IsActive, GroupNumber, VisibleForFilters FROM [HeroPOSP].[dbo].[POSP_StageDetail] WITH(NOLOCK) ORDER BY PriorityIndex

  END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                     
 END CATCH   
END

