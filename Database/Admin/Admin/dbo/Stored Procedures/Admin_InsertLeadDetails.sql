-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,30-Mar-2023>      
-- Description: <Description, Admin_InsertLeadDetails>
-- =========================================================================================         
 CREATE    PROCEDURE [dbo].[Admin_InsertLeadDetails]       
 (      
	  @LeadName VARCHAR(100),     
	  @LeadPhoneNumber VARCHAR(100),
	  @LeadEmailId VARCHAR(100),  
	  @UserId VARCHAR(100)

 )      
AS      
 BEGIN
 BEGIN TRY 

	BEGIN

	Insert into  [HeroInsurance].[dbo].[Insurance_LeadDetails] 
	(LeadName, PhoneNumber, Email, CreatedBy)
		values (@LeadName, @LeadPhoneNumber, @LeadEmailId, @UserId)

		Select Top(1) [LeadId] from [HeroInsurance].[dbo].[Insurance_LeadDetails] WITH(NOLOCK) where CreatedBy = @UserId

	END	
	
	END TRY                      
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                   
 END CATCH      
   
END 
