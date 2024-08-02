
-- exec [Admin_GetAllExamManagementDetail] '','','','',1,10
Create     PROCEDURE [dbo].[Admin_GetAllExamManagementDetail] 
(
	@SearchText VARCHAR(100),
	@Category VARCHAR(100),
	@StartDate VARCHAR(100),
	@EndDate VARCHAR(100),
	@CurrentPageIndex INT = 1,
	@CurrentPageSize INT = 10
)
AS
       
	DECLARE @SearchText1 VARCHAR(100) = @SearchText,
			@Category1 VARCHAR(100) = @Category,
			@StartDate1 VARCHAR(100) = @StartDate,    
			@EndDate1 VARCHAR(100) = @EndDate,    
			@CurrentPageIndex1 INT = @CurrentPageIndex,
			@CurrentPageSize1 INT = @CurrentPageSize

	DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1, 
			@NextPageIndex1 INT =  @CurrentPageIndex1 + 1
			
	
	DECLARE @TempDataTable TABLE(
			QuestionId VARCHAR(500),
			QuestionValue VARCHAR(MAX),
			Category VARCHAR(100),
			QuestionIndex INT,
			IsActive bit,
			CreatedOn VARCHAR(100)
	)

BEGIN
 BEGIN TRY 

	IF(@CurrentPageIndex1 = -1)
	BEGIN

		INSERT @TempDataTable
		SELECT PM.Id AS QuestionId, PM.QuestionValue, PM.ExamModuleType as Category, PM.SequenceNo as QuestionIndex, PM.IsActive, CAST(PM.CreatedOn as date) as CreatedOn
		FROM [HeroPOSP].[dbo].[POSP_ExamQuestionPaperMaster] (NOLOCK) PM		 
		WHERE
			((@SearchText1 IS NULL OR @SearchText1 = '') OR (PM.QuestionValue like '%' + @SearchText1 + '%'))
			AND ((@Category IS NULL OR @Category = '') OR (PM.ExamModuleType = @Category))
			AND 
				(
					((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR 
						CAST(PM.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)
				)
		ORDER BY  PM.CreatedOn desc

		SELECT QuestionId, QuestionValue, Category, QuestionIndex, IsActive, CreatedOn FROM @TempDataTable ORDER BY QuestionId, QuestionIndex
		
		SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord

	END
	ELSE
	BEGIN

		INSERT @TempDataTable
		SELECT PM.Id AS QuestionId, PM.QuestionValue, PM.ExamModuleType as Category, PM.SequenceNo as QuestionIndex, PM.IsActive, CAST(PM.CreatedOn as date) as CreatedOn
		FROM [HeroPOSP].[dbo].[POSP_ExamQuestionPaperMaster] (NOLOCK) PM		 
		WHERE
			((@SearchText1 IS NULL OR @SearchText1 = '') OR (PM.QuestionValue like '%' + @SearchText1 + '%'))
			AND ((@Category IS NULL OR @Category = '') OR (PM.ExamModuleType = @Category))
			AND 
				(
					((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR 
						CAST(PM.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)
				)
		ORDER BY PM.CreatedOn desc


		SELECT PM.Id AS QuestionId, PM.QuestionValue, PM.ExamModuleType as Category, PM.SequenceNo as QuestionIndex, PM.IsActive, CAST(PM.CreatedOn as date) as CreatedOn
		FROM [HeroPOSP].[dbo].[POSP_ExamQuestionPaperMaster] (NOLOCK) PM		 
		WHERE
			((@SearchText1 IS NULL OR @SearchText1 = '') OR (PM.QuestionValue like '%' + @SearchText1 + '%'))
			AND ((@Category IS NULL OR @Category = '') OR (PM.ExamModuleType = @Category))
			AND 
				(
					((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR 
						CAST(PM.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)
				)
		ORDER BY PM.CreatedOn desc OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY		

		SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,
			   @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,
			   (SELECT COUNT(DISTINCT QuestionId) from @TempDataTable) as TotalRecord

	END

  END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END
