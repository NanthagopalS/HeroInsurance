-------- =============================================  
-- Author:  <Author, Ankit>  
-- Create date: <Create Date, 6-Feb-2023>  
-- Description: <Description,,Admin_InsertBUDetails>  
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_InsertBUDetails]   
(  
	@BUName varchar(50),
	@BuHeadId varchar(100),
	@HierarchyLevelId varchar(100),
	@IsSales bit = 0
)  
AS  
BEGIN  
 BEGIN TRY   
	
	IF NOT EXISTS(SELECT TOP 1 BUId FROM Admin_BU WITH(NOLOCK) WHERE BUName =  @BUName AND BUHeadId = @BUHeadId AND BULevelId = @HierarchyLevelId)
	BEGIN
		INSERT INTO [dbo].[Admin_BU]		
			   (BUName, BUHeadId, BULevelId, IsSales )  
		 VALUES  
			   ( @BUName, @BuHeadId, @HierarchyLevelId, @IsSales)
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
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
