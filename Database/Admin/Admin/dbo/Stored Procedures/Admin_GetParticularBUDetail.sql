  
          
 CREATE   PROCEDURE [dbo].[Admin_GetParticularBUDetail]         
 (        
	@BUId VARCHAR(100) = NULL  
 )        
AS  
  
BEGIN        
 BEGIN TRY        
      
	 SELECT BU.BUName, BU.BUHeadId, IU.UserName as BUHeadName, BU.BULevelId as HierarchyLevelId, BL.BULevelName as HierarchyLevelName, BU.IsSales  
	 FROM Admin_BU BU WITH(NOLOCK) 
	 LEFT JOIN Admin_BULevel BL WITH(NOLOCK) ON BL.BULevelId = BU.BULevelId AND BL.IsActive = 1
	 LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU WITH(NOLOCK) ON IU.UserId = BU.BUHeadId AND IU.IsActive = 1 
	 WHERE BU.BUId = @BUId
  
 END TRY                        
 BEGIN CATCH          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
     
END 
