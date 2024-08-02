
CREATE     PROCEDURE [dbo].[Admin_InsertStampData]       
(  
@SrNo varchar(50),
 @StampData varchar(100)
 
)  
AS  
BEGIN  
 BEGIN TRY   

	IF NOT EXISTS(SELECT SrNo FROM [HeroAdmin].[dbo].[Admin_StampData] WITH(NOLOCK) WHERE StampData = @StampData AND IsActive = 1)
	BEGIN
		INSERT INTO [HeroAdmin].[dbo].[Admin_StampData]		
           ([SrNo],[StampData] )  
		VALUES  
           ( @SrNo, @StampData)
	END

 END TRY                          
 BEGIN CATCH                
             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList      
        
 END CATCH      
      
END 
