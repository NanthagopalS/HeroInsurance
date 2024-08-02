-- =============================================  
-- Author:  <Author,,Parth Gandhi>  
-- Create date: <Create Date,,25-APR-2023>  
-- Description: <Description [Insurance_GetBajajBreakinDetail]>  
--[Insurance_GetBajajBreakinDetail] '1389583265'  
-- =============================================  
CREATE   PROCEDURE [Insurance_GetBajajBreakinDetail]  
(  
 @TransationId VARCHAR(100) = NULL
)  
AS  
BEGIN  
 BEGIN TRY  
  SET NOCOUNT ON;  
    DECLARE @LeadId VARCHAR(100), @PrevPolicyTypeId VARCHAR(100), @PolicyTypeId VARCHAR(100)
	SET @LeadId = (SELECT LeadId from Insurance_QuoteTransaction WITH(NOLOCK) where TransactionId = @TransationId)
	SELECT @PrevPolicyTypeId = PrevPolicyTypeId, @PolicyTypeId = PolicyTypeId 
		FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadId
	IF(@PrevPolicyTypeId IS NOT NULL OR @PrevPolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B')
	BEGIN
		IF(@PolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D')
		BEGIN
			SELECT 1 as IsBreakin 
		END
		ELSE
		BEGIN
			SELECT 0 as IsBreakin 
		END
	END
 END TRY                  
 BEGIN CATCH            
         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END  