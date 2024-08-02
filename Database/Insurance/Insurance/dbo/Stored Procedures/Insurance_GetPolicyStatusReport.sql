
CREATE     PROCEDURE [dbo].[Insurance_GetPolicyStatusReport]             
(        
	  
	 @UserId VARCHAR(100) = NULL,
	 @StartDate VARCHAR(100) = NULL,        
	 @EndDate VARCHAR(100) = NULL   

 )       
AS            
BEGIN            
 BEGIN TRY


  
	select  PDT.PolicyNo,  
		 CAST(LD.CreatedOn as Date) as DateTime
		FROM [HeroInsurance].[dbo].[Insurance_PolicyDumpTable] PDT WITH(NOLOCK)
		LEFT JOIN [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH(NOLOCK) ON LD.Email = PDT .UserEmail
		WHERE UserId=@UserId
		

		
	
	END TRY                            
BEGIN CATCH              
	DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
	SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
	SET @ErrorDetail=ERROR_MESSAGE()                                        
	EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
END CATCH            
         
END