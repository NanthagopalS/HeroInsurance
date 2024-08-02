
CREATE   PROCEDURE [dbo].[Identity_InsertBankVerificationDetails]  
@BankVerificationId VARCHAR(50) = null,  
@Task VARCHAR(50) = NULL,  
@Id VARCHAR(100) = NULL,  
@PatronId VARCHAR(100) = NULL,  
@Active VARCHAR(100) = NULL,  
@Reason VARCHAR(100) = NULL,  
@NameMatch VARCHAR(100) = NULL,  
@MobileMatch VARCHAR(100) = NULL,  
@SignzyReferenceId VARCHAR(100) = NULL,  
@NameMatchScore VARCHAR(100) = NULL,  
@Nature VARCHAR(100) = NULL,  
@Value VARCHAR(100) = NULL,  
@Timestamp VARCHAR(100) = NULL,  
@Response VARCHAR(100) = NULL,  
@BankRRN VARCHAR(100) = NULL,  
@BeneName VARCHAR(100) = NULL,  
@BeneMMID VARCHAR(100) = NULL,  
@BeneMobile VARCHAR(100) = NULL,  
@BeneIFSC VARCHAR(100) = NULL,  
@CreatedBy VARCHAR(100) = NULL,  
@CreatedOn DATETIME = NULL  
AS  
BEGIN  
 BEGIN TRY  
  IF EXISTS (SELECT Active FROM dbo.Identity_BankVerification WITH (NOLOCK) WHERE BankVerificationId = @BankVerificationId)  
   BEGIN  
    UPDATE dbo.Identity_BankVerification SET Task = @Task,Id = @Id,PatronId =@PatronId,Active = @Active,Reason = @Reason,  
    NameMatch = @NameMatch,MobileMatch = @MobileMatch,SignzyReferenceId = @SignzyReferenceId,NameMatchScore = @NameMatchScore,  
    Nature = @Nature,Value = @Value,Timestamp  =@Timestamp,Response = @Response,BankRRN = @BankRRN,  
    BeneName = @BeneName,BeneMMID  =@BeneMMID,BeneMobile = @BeneMobile,BeneIFSC = @BeneIFSC,UpdatedBy = '1',UpdatedOn = GETDATE()    
    WHERE BankVerificationId  = @BankVerificationId  
   END  
  ELSE  
   BEGIN  
    INSERT INTO dbo.Identity_BankVerification   
    (Task,Id,PatronId,Active,Reason,NameMatch,MobileMatch,SignzyReferenceId,NameMatchScore,  
    Nature,Value,Timestamp,Response,BankRRN,BeneName,BeneMMID,BeneMobile,BeneIFSC,CreatedBy,CreatedOn)   
    VALUES (@Task,@Id,@PatronId,@Active,@Reason,@NameMatch,@MobileMatch,@SignzyReferenceId,@NameMatchScore,  
    @Nature,@Value,@Timestamp,@Response,@BankRRN,@BeneName,@BeneMMID,@BeneMobile,@BeneIFSC,'1',GETDATE())  
   END  

   --SELECT top 1 NameMatchScore from dbo.Identity_BankVerification where BeneIFSC=@BeneIFSC order by CreatedOn desc
 END TRY  
 BEGIN CATCH             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC dbo.Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                          
 END CATCH  
END
