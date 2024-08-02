﻿       
CREATE   PROCEDURE [dbo].[Admin_GetAlertType]        
AS        
BEGIN        
 BEGIN TRY

	Select AlertTypeId, AlertType  from [HeroAdmin].[dbo].[Admin_AlertType] (NOLOCK)
	Where IsActive = 1 AND IsDisplay = 1

 END TRY                        
 BEGIN CATCH        
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END       