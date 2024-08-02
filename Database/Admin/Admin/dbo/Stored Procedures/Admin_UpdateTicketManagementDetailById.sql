-- =============================================    
-- Author:  <Author, Ankit>    
-- Create date: <Create Date,14-Feb-2023>    
-- Description: <Description,Admin_UpdateTicketManagementDetailById>    
-- Admin_UpdateTicketManagementDetailById   
-- =============================================    
 CREATE    PROCEDURE [dbo].[Admin_UpdateTicketManagementDetailById]     
(    
  @TicketId varchar(100),  
  @Description varchar(140),
  @Status VARCHAR(100)
)    
AS    
BEGIN    
 BEGIN TRY    
     
   UPDATE [HeroAdmin].[dbo].[Admin_HelpAndSupport]  
    SET Description = @Description, Status = @Status , UpdatedOn = GETDATE() 
    WHERE Id = @TicketId  
      
 END TRY                    
 BEGIN CATCH           
         
      
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
    
END
