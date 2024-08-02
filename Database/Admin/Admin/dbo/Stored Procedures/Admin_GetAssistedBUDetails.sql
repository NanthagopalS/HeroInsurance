
-- =============================================  
-- Author:  <Author,Neeraj >  
-- Create date: <Create Date,16/May/2023>  
-- Description: <Description,[GetAssistedBUDetails]>  
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_GetAssistedBUDetails]   
(  
 @RoleId VARCHAR(50) = NULL,
 @UserId VARCHAR(50) = NULL
)  
AS  
BEGIN  
 BEGIN TRY          
    
	-- select all the bu details
	SELECT BUId, BUName from [dbo].[Admin_BU] (NOLOCK) WHERE IsActive = 1   
	ORDER BY CreatedOn DESC
  
	-- selecte only the selected bu details based on roleid and user id
	SELECT ABU.BUId as SelectedBUId, BUName as SelectedBuName from [HeroAdmin].[dbo].[Admin_BU] ABU(NOLOCK) 
	LEFT JOIN [HeroAdmin].[dbo].[Admin_UserRoleMapping]  as AU WITH(NOLOCK) ON ABu.BUId= Au.BUId
	WHERE AU.UserId = @UserId AND AU.RoleId = @RoleId AND Abu.IsActive = 1  AND Au.IsActive=1
	ORDER BY AU.CreatedOn DESC
 
  END TRY                          
 BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH     
END