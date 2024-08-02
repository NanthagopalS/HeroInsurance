-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,18-Feb-2023>      
-- Description: <Description, Admin_GetFileTypeByDocumentId>
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_GetFileTypeByDocumentId]       
 (      
  @DocumentId VARCHAR(100) 
 )      
AS      
BEGIN      
 BEGIN TRY      
    
	SELECT 
		CASE WHEN DocumentFileName like '%pdf' THEN 'PDF'
			ELSE 'Image' END as FileType 
	FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH(NOLOCK)
	WHERE DocumentId = @DocumentId
	    
 END TRY                      
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                   
 END CATCH      
   
END 
