CREATE   PROCEDURE [dbo].[Insurance_GetICICIBreakinStatus]  
@BreakinId VARCHAR(50),  
@IsBreakinApproved BIT  
AS  
begin  
 BEGIN TRY  
  DECLARE @StageId VARCHAR(100)  
  
  SET NOCOUNT ON;  
  
  IF(@IsBreakinApproved = 1)  
  BEGIN  
   SET @StageId = 'ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2'  
  END  
  ELSE  
  BEGIN  
   SET @StageId = '405F4696-CDFB-4065-B364-9410B56BC78D'  
  END  
  
  
  UPDATE Insurance_LeadDetails SET IsBreakinApproved = @IsBreakinApproved, StageId = @StageId  
  WHERE BreakinId = @BreakinId  
  
  SELECT PolicyNumber ProposalNumber,BreakinId, RequestBody,ResponseBody, TransactionId, Ld.VehicleTypeId , ISNULL(LD.UpdatedBy, LD.CreatedBy) AS UserId,
  LD.LeadId as LeadId
  FROM Insurance_LeadDetails LD WITH(NOLOCK) LEFT JOIN Insurance_QuoteTransaction QT WITH(NOLOCK)  
  ON LD.QuoteTransactionID = QT.QuoteTransactionId  
  WHERE LD.BreakinId = @BreakinId  
  
 END TRY            
 BEGIN CATCH           
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END