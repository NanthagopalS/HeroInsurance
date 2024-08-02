
CREATE      PROCEDURE [dbo].[Identity_CheckPANExists]           
(          
 @PANNumber VARCHAR(100)        
)          
AS          
BEGIN          
 BEGIN TRY          
  SELECT * from TestPAN where PANNumber = @PANNumber   
   AND PANNumber not in (select  PANNumber from Identity_PanVerification PV
							Left join [dbo].[Identity_User] IU on PV.UserId = IU.UserId
						where IU.IsActive = 1 and PV.PanNumber = @PANNumber and PV.IsActive = 1)  
 END TRY                          
 BEGIN CATCH            
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
          
END     

