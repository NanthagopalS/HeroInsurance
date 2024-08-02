    
CREATE    PROCEDURE [dbo].[POSP_InsertPOSPExamDetail]     
(    
 @UserId VARCHAR(500),    
 @ExamStatus VARCHAR(20) --(Start / Finish)    
)    
AS    
BEGIN    
    
 DECLARE @ExamId VARCHAR(500) = NULL, @StatusId VARCHAR(500) = NULL    
 DECLARE @IdealEndDateTime DATETIME    
    
 BEGIN TRY    
      
  If(@ExamStatus = 'Start')    
  BEGIN    
   IF NOT EXISTS(SELECT [Id] FROM [HeroPOSP].[dbo].[POSP_Exam] WITH(NOLOCK) WHERE UserId = @UserId AND IsActive = 1)    
    BEGIN    
     SET @IdealEndDateTime = (SELECT DATEADD(MINUTE, 2, GETDATE()) AS DateAddedValue)    
    
    UPDATE POSP_Exam SET IsActive = 0, UpdatedOn = GETDATE() WHERE UserId = @UserId    
    
    UPDATE POSP_ExamPaperDetail SET IsActive = 0, UpdatedOn = GETDATE() WHERE ExamId in (SELECT Id from POSP_Exam WHERE IsActive = 0 AND UserId = @UserId)    
       
    INSERT INTO POSP_Exam (UserId, ExamIdealEndDateTime) VALUES (@UserId, @IdealEndDateTime)    
    
    --Set 20 Question for this POSP...    
    --Get ExamId    
    SET @ExamId = (SELECT TOP(1) Id FROM POSP_Exam WHERE IsActive = 1 AND UserId = @UserId ORDER BY ExamStartDateTime DESC)    
    
    --Get StatusId    
    SET @StatusId = (SELECT TOP(1) Id FROM POSP_ExamQuestionStatusMaster WHERE StatusValue = 'Not Visited')    
    
    -- Insert 10 General Insurance Question    
    INSERT INTO POSP_ExamPaperDetail (ExamId, QuestionNo, QuestionId, StatusId)    
    SELECT TOP(10) @ExamId AS ExamId , ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS QuestionNo, Id AS QuestionId, @StatusId AS StatusId FROM POSP_ExamQuestionPaperMaster WHERE ExamModuleType = 'General Insurance' and IsActive = 1 ORDER BY RAND()    
    
    INSERT INTO POSP_ExamPaperDetail (ExamId, QuestionNo, QuestionId, StatusId)    
    SELECT TOP(10) @ExamId AS ExamId , ((ROW_NUMBER() OVER(ORDER BY (SELECT 1))) + 10) AS QuestionNo, Id AS QuestionId, @StatusId AS StatusId FROM POSP_ExamQuestionPaperMaster WHERE ExamModuleType = 'Life Insurance' and IsActive = 1 ORDER BY RAND()    
    
	EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserID, 'D56CA403-6B9E-48F9-B608-F008472EFACC'

	END       
      
  END    
  ELSE IF(@ExamStatus = 'Finish')    
  BEGIN    
       
   UPDATE [dbo].[POSP_Exam] SET ExamEndDateTime = GETDATE(), UpdatedOn = GETDATE() WHERE IsActive = 1 AND UserId = @UserId    
    
   --check its clreared or not...    
   IF EXISTS(SELECT Id FROM [dbo].[POSP_Exam] WITH(NOLOCK) WHERE IsActive = 1 AND UserId = @UserId AND IsCleared = 1)    
   BEGIN    
        
    SET @StatusId = (SELECT Id FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH(NOLOCK) WHERE StatusName = 'Training & Exam' AND PriorityIndex = 7)    
        
    --UPDATE Breadcrum stage for PSOP User            
      UPDATE [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail] SET StatusId = @StatusId, UpdatedOn = GETDATE() WHERE UserId = @UserId AND StatusId IN (SELECT Id from [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WHERE StatusName = 'Training & Exam' AND PriorityIndex IN (5, 6))    
        -- update StageId    
  
     END      
  END     
    
  SELECT TOP(1) Id, ExamStartDateTime, ExamEndDateTime, ExamIdealEndDateTime FROM [dbo].[POSP_Exam] WITH(NOLOCK) WHERE IsActive = 1 AND UserId = @UserId ORDER BY ExamStartDateTime DESC      
    
 END TRY                    
 BEGIN CATCH              
           
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
    
END
