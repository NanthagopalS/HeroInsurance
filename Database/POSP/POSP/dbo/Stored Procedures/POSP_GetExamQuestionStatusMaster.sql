﻿
CREATE  PROCEDURE [dbo].[POSP_GetExamQuestionStatusMaster]          
AS        
BEGIN        
 BEGIN TRY        
  SELECT [Id],[StatusValue],[IsActive],[CreatedBy],[CreatedOn],[UpdatedBy],[UpdatedOn]  FROM  [dbo].[POSP_ExamQuestionStatusMaster] WITH(NOLOCK)   
  END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END   
