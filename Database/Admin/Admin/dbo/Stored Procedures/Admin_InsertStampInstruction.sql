
CREATE     PROCEDURE [dbo].[Admin_InsertStampInstruction]       
(  
@SrNo varchar(50),
 @Instruction varchar(100)
 
)  
AS  
BEGIN  
 BEGIN TRY   
  INSERT INTO [HeroAdmin].[dbo].[Admin_StampInstruction]		
           ([SrNo],[Instruction] )  
     VALUES  
           (  @SrNo, @Instruction) 

 END TRY                          
 BEGIN CATCH                
             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList      
        
 END CATCH      
      
END 
