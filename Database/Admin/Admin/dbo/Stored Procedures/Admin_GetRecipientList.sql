-------- =============================================    
-- Author:  <Author, Ankit>    
-- Create date: <Create Date, 4-April-2023>    
-- Description: <Description,,Admin_GetRecipientList>    
-- =============================================    
CREATE    PROCEDURE [dbo].[Admin_GetRecipientList]     
(    
 @SearchText VARCHAR(100),  
   @RecipientType VARCHAR(100)  
)    
AS    
BEGIN    
 BEGIN TRY   
 BEGIN  
  
 Select Top (100) IU.UserId, IU.UserName, IU.EmpID, IU.POSPId  from [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) 
 where  
  ((@SearchText IS NULL OR @SearchText = '') OR (IU.UserName like '%' + @SearchText + '%'))  
  and POSPId is not null  and IU.IsActive = 1
 
    
 END  
  
 END TRY                    
 BEGIN CATCH              
 IF @@TRANCOUNT > 0    
        ROLLBACK      
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
    
END 
