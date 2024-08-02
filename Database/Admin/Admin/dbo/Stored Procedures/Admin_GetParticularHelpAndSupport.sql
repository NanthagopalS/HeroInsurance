      
              
 CREATE    PROCEDURE [dbo].[Admin_GetParticularHelpAndSupport]             
 (            
 @RequestId VARCHAR(100) = NULL      
 )            
AS      
      
BEGIN            
 BEGIN TRY            
          
  SELECT hp.ConcernTypeId, hp.SubConcernTypeId,hp.SubjectText, hp.DetailText, hp.Description, hp.DocumentId, hp.IsActive,hp.Status,  
  ct.ConcernTypeName,sct.SubConcernTypeName,hp.CreatedOn,convert(date, hp.CreatedOn) as CreatedOnDate,
		 convert(varchar(8), convert(time, hp.CreatedOn)) as CreatedOnTime  
  FROM Admin_HelpAndSupport hp WITH(NOLOCK)     
  LEFT JOIN Admin_ConcernType ct WITH(NOLOCK) ON hp.ConcernTypeId = ct.ConcernTypeId      
  LEFT JOIN Admin_SubConcernType sct WITH(NOLOCK) ON hp.SubConcernTypeId = sct.SubConcernTypeId      
  WHERE hp.Id = @RequestId      
      
 END TRY                            
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
         
END 
