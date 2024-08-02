

CREATE PROCEDURE [dbo].[Identity_GetAllRelationshipManager]
(
	@UserId VARCHAR(100)
)
AS 
BEGIN   
	BEGIN TRY
	If (@UserId != Null or @UserId != '')
   SELECT distinct 
      UD.ServicedByUserId,
      U.UserName
   FROM [Identity_UserDetail] UD WITH(NOLOCK)
   INNER JOIN [Identity_User] U WITH(NOLOCK) ON U.UserId = UD.ServicedByUserId
   where UD.UserId = @UserId
	  
	  ELSE
		 SELECT distinct
      UD.ServicedByUserId,
      U.UserName
   FROM [Identity_UserDetail] UD WITH(NOLOCK)
   INNER JOIN [Identity_User] U WITH(NOLOCK) ON U.UserId = Ud.ServicedByUserId
END TRY

BEGIN CATCH                          
    DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                                    
    SET @StrProcedure_Name=ERROR_PROCEDURE()                                                    
    SET @ErrorDetail=ERROR_MESSAGE()                                                    
    EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                                     
   END CATCH 
 END 


 --select * from Identity_UserDetail where ServicedByUserId is not null and UserId in (Select UserId from Identity_User)
 --and ServicedByUserId in (Select UserId from Identity_User)

 --select * from Identity_User where UserId = 'E38E6B76-1EE1-4951-B587-5E2EACEEA27A'