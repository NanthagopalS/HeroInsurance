    
-- =============================================    
-- Author: <Author,Harsh Patel>    
-- Create date: <Create Date, 24-01-2022>    
-- Description: <Description,POSP_GetUserCertificateDetail>    
-- =============================================    
CREATE PROCEDURE [dbo].[POSP_GetUserCertificateDetail]      
@UserId VARCHAR(100)
AS    
BEGIN    
 BEGIN TRY    
      
  --SELECT Id, IsCleared, DocumentId    
  --FROM [dbo].[POSP_Exam] WHERE UserId = @UserId AND IsActive = 1    
   SELECT PE.Id, PE.IsCleared, PE.DocumentId,IU.UserName,IU.EmailId,US.StateName, UD.AadhaarNumber,
     UD.PAN,UA.AddressLine1,UA.AddressLine2,IU.POSPId
	 FROM [dbo].[POSP_Exam] as PE WITH(NOLOCK)  
   INNER JOIN [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK)  ON PE.UserId = IU.UserId  
   LEFT JOIN [HeroIdentity].[dbo].[Identity_UserAddressDetail] as UA WITH(NOLOCK) on UA.UserId = @UserId
   LEFT JOIN [HeroIdentity].[dbo].[Identity_State] as US WITH(NOLOCK) on US.StateId = UA.StateId
   LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] as UD WITH(NOLOCK) on UD.UserId = @UserId
  WHERE PE.UserId=@UserId AND PE.IsActive = 1  
  
  
 END TRY                    
 BEGIN CATCH              
           
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList    
 END CATCH    
END
