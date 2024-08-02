-- =============================================  
-- Author:  <Author, Ankit>  
-- Create date: <Create Date,14-Feb-2023>  
-- Description: <Description,Admin_UpdateBUDetails>  
-- Admin_UpdateBUDetails 2,true  
-- =============================================  
 CREATE     PROCEDURE [dbo].[Admin_UpdateBUDetails]   
(  
	 @BUID varchar(100),
	 @BUName varchar(100),
	 @BUHeadId varchar(100),
	 @HierarchyLevelId varchar(100),
	 @IsSales bit = 0
)  
AS  
BEGIN  
 BEGIN TRY  
   
   UPDATE Admin_BU  
    SET [BUName] =  @BUName, [BUHeadId] = @BUHeadId, [BULevelId] = @HierarchyLevelId, IsSales = @IsSales, CreatedOn= GETDATE()
    WHERE BUID = @BUID

	Create table #UserId (UserId varchar (100))
	Insert into  #UserId (UserId)(select UserId from HeroIdentity.dbo.[Func_GetUsersOfBU](@BUID,''))

	UPDATE [HeroIdentity].[dbo].[Identity_User]
	Set IsActive = 0, CreatedOn= GETDATE() where Userid in (select UserId from #UserId)
    
 END TRY                  
 BEGIN CATCH         
       
    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END
