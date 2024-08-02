
CREATE PROCEDURE [dbo].[Identity_GetErrorCode]    
@ErrorType VARCHAR(10) NULL   
AS          
BEGIN          
 
 BEGIN TRY
		
		If(@ErrorType = 'Backend')
		BEGIN
		
			SELECT Id, ErrorType, ErrorCode, ErrorKey, ErrorMessage    
				FROM Identity_ErrorCode WITH(NOLOCK) WHERE ErrorType = @ErrorType 			
		END
		ELSE IF(@ErrorType = 'Web')
		BEGIN
			
			SELECT Id, ErrorType, ErrorCode, ErrorKey, ErrorMessage    
				FROM Identity_ErrorCode WITH(NOLOCK) WHERE ErrorType = @ErrorType 
				
		END	

		ELSE IF(@ErrorType = 'Mobile')
		BEGIN
			
			SELECT Id, ErrorType, ErrorCode, ErrorKey, ErrorMessage    
				FROM Identity_ErrorCode WITH(NOLOCK) WHERE ErrorType = @ErrorType 
				
		END	
		

	END TRY   

                          
 BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
END     




