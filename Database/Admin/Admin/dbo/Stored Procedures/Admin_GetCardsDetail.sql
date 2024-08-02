-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,23-Feb-2023>      
-- Description: <Description, Admin_GetCardsDetail>
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[Admin_GetCardsDetail]       
 (      
	@UserId VARCHAR(100) = Null     
 ) 
 As
 Begin  

 BEGIN TRY 
 
	BEGIN
	Select RL.RoleLevelName,  RM.RoleName, RM.RoleTypeID, IU.UserName, IU.EmailId, IU.UserId, IU.MobileNo from [HeroIdentity].[dbo].[Identity_User] (NOLOCK) IU
	Inner join [HeroAdmin].[dbo].[Admin_RoleMaster] (NOLOCK) RM on  RM.RoleId = IU.RoleId
	Inner join [HeroAdmin].[dbo].[Admin_RoleLevel] (NOLOCK) RL on RL.RoleLevelId = RM.RoleLevelID
	where IU.UserId = @UserId ORDER BY IU.CreatedOn DESC
	END
	
	BEGIN
	Select Ic.ConfigurationKey, Ic.ConfigurationValue from [HeroIdentity].[dbo].[Identity_Configuration] as Ic
	where Ic.ConfigurationKey = 'SupportTime' Or Ic.ConfigurationKey = 'SupportEmail' or Ic.ConfigurationKey = 'SupportNumber'
	END
	
	END TRY 
	
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END 
