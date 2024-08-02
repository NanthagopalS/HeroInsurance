CREATE    PROCEDURE [dbo].[Identity_DeleteBenefitsDetail]  
(  
@Id varchar(200)
)  
AS  
BEGIN  
 UPDATE [dbo].[Identity_BenefitsDetail]  
 SET   
  IsActive = 0,  
  UpdatedOn = GetDate()  
 WHERE Id = @Id  
END  
