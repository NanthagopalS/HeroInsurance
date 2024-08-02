-- EXEC [dbo].[Insurance_GetIFFCOCustomerType] '14918192-6AD0-47C0-A41C-4BED4C4631C7','TN','COMBTR' ,'Mr.'
CREATE     PROCEDURE [dbo].[Insurance_GetIFFCOCustomerType] 
@QuoteTransactionId VARCHAR(50) =NULL,
@StateCode VARCHAR(20) = NULL,
@CityCode VARCHAR(20) = NULL,
@Salutation VARCHAR(20) = NULL
AS  
BEGIN  
 BEGIN TRY  
  SET NOCOUNT ON;  
  
  DECLARE @CustomerType VARCHAR(100), @City VARCHAR(100), @State VARCHAR(100), @SalutationCode VARCHAR(50)

  SELECT @CustomerType = CarOwnedBy
  FROM Insurance_LeadDetails WITH(NOLOCK)  WHERE QuoteTransactionID = @QuoteTransactionId

  SELECT @State = STATE_NAME FROM MOTOR.ITGI_StateMasters WITH(NOLOCK) WHERE STATE_CODE = @StateCode

  SELECT @City = CITY_DESC FROM MOTOR.ITGI_CityMasters WITH(NOLOCK) WHERE CITY_CODE = @CityCode

  SELECT @SalutationCode = Mode FROM MOTOR.ITGI_SalutationMaster WITH(NOLOCK) WHERE TXT_Salutation = @Salutation 

  SELECT @CustomerType CustomerType,
  @State State,
  @City City,
  @SalutationCode Salutation
 
 END TRY                  
 BEGIN CATCH            
         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END