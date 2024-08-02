    
-- =============================================    
-- Author:  <Author,,Harsh Patel>    
-- Create date: <Create Date,,2/jan/2023>    
-- Description: <Description,GetExamDetail>    
-- =============================================    
CREATE    PROCEDURE [dbo].[POSP_GetExamDetail]     
(    
 @Id VARCHAR(500),    
 @UserId VARCHAR(500)    
)    
AS    
BEGIN    
 BEGIN TRY            
 SELECT TOP(1) ExamStartDateTime, ExamEndDateTime, CorrectAnswered, InCorrectAnswered, SkippedAnswered, FinalResult, IsCleared      
  FROM  [dbo].[POSP_Exam] WITH(NOLOCK) WHERE Id = @Id AND UserId = @UserId AND IsActive = 1    
  ORDER BY ExamStartDateTime DESC    
    
  IF EXISTS(SELECT * FROM  [dbo].[POSP_Exam] WITH(NOLOCK) WHERE Id = @Id AND UserId = @UserId AND IsActive = 1 AND IsCleared =0)    
   BEGIN    
  UPDATE POSP_Exam SET IsActive = 0 WHERE Id = @Id AND UserId = @UserId    
  EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId, 'D56CA403-6B9E-48F9-B608-F008472EFACC'  
   END    
  ELSE  
   BEGIN  
  SELECT * INTO #TMP_USR_EXAM FROM HeroIdentity.dbo.Identity_User WHERE UserId = @UserId  
  IF EXISTS (SELECT * FROM #TMP_USR_EXAM WHERE IIBStatus = 'NOT EXISTING' AND IIBUploadStatus='SUCCESS')  
   BEGIN  
    EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId, '6D26CFBA-B5FA-46C8-A048-3E96982D90B7'  
   END    
  ELSE  
   BEGIN  
    EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId, 'EE1FF4F7-3BF9-402E-9461-47FE0F8A06C1'  
   END  
   END  
    
  END TRY                            
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH       
END

