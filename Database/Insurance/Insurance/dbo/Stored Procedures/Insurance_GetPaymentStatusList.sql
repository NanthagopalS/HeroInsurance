/*
exec [Insurance_GetPaymentStatusList] 
*/
CREATE  PROCEDURE [dbo].[Insurance_GetPaymentStatusList]             
AS            
BEGIN            
 BEGIN TRY
	SELECT PaymentId,PaymentStatus FROM Insurance_PaymentStatus WITH(NOLOCK) WHERE IsActive=1
END TRY                            
BEGIN CATCH              
	DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
	SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
	SET @ErrorDetail=ERROR_MESSAGE()                                        
	EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
END CATCH            
		 
END