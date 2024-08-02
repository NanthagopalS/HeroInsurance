
-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,26-Dec-2022>
-- Description:	<Description,Identity_GetBUDetails Admin>
/*
	 Identity_GetBUDetails 'Business Unit2','2022-12-25','2022-12-26'
	 Identity_GetBUDetails 'Business Unit2','','' 
	 Identity_GetBUDetails '','',''
*/
-- =============================================
 CREATE PROCEDURE [dbo].[Identity_GetBUDetails] 
 	
	 @BUName VARCHAR(50)=Null,	
	 @CreatedFrom VARCHAR(50)=Null,
     @CreatedTo VARCHAR(50)=Null
	 ----,@isActive bit =null
 
AS

BEGIN
	BEGIN TRY
	 --if (isnull(@BUName,'') = '' OR (isnull(@CreatedFrom,'') = '' OR isnull(@CreatedTo,'') = ''))	
	  if (@BUName = '' AND (@CreatedFrom = '' OR @CreatedTo = ''))
		 BEGIN	       
			  print('if Null')
			   SELECT B.BUID, B.RoleTypeID, B.BULevelID, 
					  B.BUName, B.IsActive, R.RoleTypeName,
					  L.BULevelName, L.BULevelID,
					  B.CreatedBy,  B.CreatedDate, B.RoleId, IR.RoleName            
			   FROM   Identity_BU B WITH(NOLOCK) LEFT OUTER JOIN
					  Identity_RoleType R WITH(NOLOCK) ON B.RoleTypeID = R.RoleTypeID INNER JOIN
					  Identity_BULevel L WITH(NOLOCK) ON B.BULevelID = L.BULevelID  INNER JOIN
                      Identity_RoleMaster IR WITH(NOLOCK) ON B.RoleId = IR.RoleId
		END
	ELSE
	  BEGIN
	           print('if Not Null')
	           SELECT B.BUID, B.RoleTypeID, B.BULevelID, 
					  B.BUName, B.IsActive, R.RoleTypeName,
					  L.BULevelName, L.BULevelID ,
					  B.CreatedBy,  B.CreatedDate, B.RoleId, IR.RoleName                
			   FROM   Identity_BU B WITH(NOLOCK) LEFT OUTER JOIN
					  Identity_RoleType R WITH(NOLOCK) ON B.RoleTypeID = R.RoleTypeID INNER JOIN
					  Identity_BULevel L WITH(NOLOCK) ON B.BULevelID = L.BULevelID INNER JOIN
                      Identity_RoleMaster IR WITH(NOLOCK) ON B.RoleId = IR.RoleId
					  WHERE
					  	 B.BUName= CASE WHEN @BUName IS NULL THEN B.BUName ELSE @BUName END 					
					OR				
					(cast(B.CreatedDate as date) BETWEEN @CreatedFrom and @CreatedTo )
	  END

	END TRY                
	BEGIN CATCH  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END

