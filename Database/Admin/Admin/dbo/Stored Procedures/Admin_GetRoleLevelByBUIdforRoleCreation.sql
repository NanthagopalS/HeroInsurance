  
          
 CREATE    PROCEDURE [dbo].[Admin_GetRoleLevelByBUIdforRoleCreation]         
 (        
	@BUId VARCHAR(100) = NULL  
 )        
AS  
  
BEGIN        
 BEGIN TRY 

	DECLARE @BULevelMaxPriorityIndex int = 0,
			@Counter int = 1

	DECLARE @TempDataTable TABLE(
		RoleLevelId VARCHAR(100), 
		RoleLevelName VARCHAR(10), 
		PriorityIndex int, 
		IsEnable bit
	)

	SET @BULevelMaxPriorityIndex = (	SELECT BL.PriorityIndex
										FROM Admin_BU BU WITH(NOLOCK)
										INNER JOIN Admin_BULevel BL WITH(NOLOCK) ON BL.BULevelId = BU.BULevelId
										WHERE BU.BUId = @BUId AND BU.IsActive = 1 AND BL.IsActive = 1
									)

	INSERT @TempDataTable
	SELECT DISTINCT RL.RoleLevelId, RL.RoleLevelName, RL.PriorityIndex, 0 
	FROM Admin_RoleLevel RL WITH(NOLOCK)
	WHERE RL.PriorityIndex <= @BULevelMaxPriorityIndex
	ORDER BY RL.PriorityIndex

	WHILE(@Counter <= @BULevelMaxPriorityIndex)
	BEGIN
		
		IF NOT EXISTS(SELECT TOP(1) RoleId FROM Admin_RoleMaster WITH(NOLOCK) WHERE BUId = @BUId AND IsActive = 1 AND RoleLevelID IN (SELECT RoleLevelId FROM @TempDataTable WHERE PriorityIndex = @Counter))
		BEGIN
				UPDATE @TempDataTable SET IsEnable = 1 WHERE PriorityIndex = @Counter
				BREAK
		END
		
		SET @Counter = @Counter + 1

	END

	SELECT RoleLevelId, RoleLevelName, PriorityIndex, IsEnable
	FROM @TempDataTable
	ORDER BY PriorityIndex
  
 END TRY                        
 BEGIN CATCH          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
     
END 
