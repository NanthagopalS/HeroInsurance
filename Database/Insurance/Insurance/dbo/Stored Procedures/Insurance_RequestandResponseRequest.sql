/*              
EXEC [Insurance_RequestandResponseRequest] '','77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA,78190CB2-B325-4764-9BD9-5B9806E99621,372B076C-D9D9-48DC-9526-6EB3D525CAB6','','','2023-08-28','2023-08-29',1,10      
*/      
CREATE      PROCEDURE [dbo].[Insurance_RequestandResponseRequest] (      
      
 @LeadId VARCHAR(100) = NULL,      
 @InsurerId VARCHAR(max) = NULL,      
 @ProductId VARCHAR(100) = NULL,      
 @StageId VARCHAR(max) = Null,      
 @StartDate VARCHAR(100),       
 @EndDate VARCHAR(100),      
 @CurrentPageIndex INT = 1,      
 @CurrentPageSize INT = 10      
 )      
AS      
BEGIN      
BEGIN TRY      
   DECLARE @REQ_RES_REPORT_TEMP AS TABLE (    
   LeadId VARCHAR(100),    
   InsurerName VARCHAR(200),    
   StageID VARCHAR(100),    
   Product VARCHAR(100),    
   ApiURL VARCHAR(max),    
   RequestBody VARCHAR(max),    
   RequestTime VARCHAR(50),    
   ResponseBody VARCHAR(max),    
   ResponseTime VARCHAR(100),      
   Responnsestatuscode VARCHAR(100),    
   ResponseError VARCHAR(max),    
   InsuranceName VARCHAR(200),    
   StageName VARCHAR(100)    
   )    
  INSERT @REQ_RES_REPORT_TEMP    
  SELECT TBL.LeadId, II.InsurerName as InsurerName,    
  SM.Stage,IType.InsuranceName AS Product,    
  TBL.ApiURL,    
  TBL.RequestBody,    
  TBL.RequestTime,    
  TBL.ResponseBody,    
  TBL.ResponseTime,      
  CASE WHEN (ISJSON(TBL.ResponseBody) = 1 OR LEN(TBL.ResponseBody)>150) THEN '200' ELSE ' ' END AS Responnsestatuscode,    
  CASE WHEN (ISJSON(TBL.ResponseBody) = 1 OR LEN(TBL.ResponseBody)>150) THEN ' ' ELSE TBL.ResponseBody END AS ResponseError    
  ,IType.InsuranceName, SM.Stage    
   from [HeroLogs].[dbo].[TblInsuranceLogs] TBL      
  LEFT JOIN [HeroInsurance].[dbo].[Insurance_LeadDetails]LD WITH(NOLOCK) ON LD.LeadId=TBL.LeadId      
  LEFT JOIN [HeroInsurance].[dbo].[Insurance_Insurer] II WITH(NOLOCK) ON II.InsurerId = TBL.Insurer      
  LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH(NOLOCK) ON SM.StageID=TBL.StageId      
  LEFT JOIN [HeroInsurance].[dbo].[Insurance_InsuranceType] IType WITH(NOLOCK) ON IType.InsuranceTypeId = LD.VehicleTypeId         
  WHERE       
  TBL.LeadId IS NOT NULL     
  AND      
  ( CAST(LD.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)  OR ISNULL(@StartDate, '') = '' )      
   AND     
   ( CAST(LD.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)  OR ISNULL(@EndDate, '') = ''  )    
   AND ((II.InsurerId IN (SELECT value FROM STRING_SPLIT(convert(varchar(max),@InsurerId), ','))) OR ISNULL(convert(varchar(max),@InsurerId),'') = '')    
   AND ((LD.VehicleTypeId = @ProductId ) or (ISNULL(@ProductId,'')=''))    
 AND ((TBL.StageID IN (SELECT value FROM STRING_SPLIT(@StageId, ','))) OR ISNULL(@StageId, '') = '')    
     
 SELECT *    
 INTO #REQ_AND_RES_TEMP_TABLE    
 FROM @REQ_RES_REPORT_TEMP    
 WHERE (ISNULL(@LeadId,'')='' OR  LeadId = @LeadId)    
    
   DECLARE @TotalRecords INT = (    
    SELECT COUNT(*)    
    FROM #REQ_AND_RES_TEMP_TABLE    
    )    
 
   IF(@CurrentPageSize = -1)    
     BEGIN    
      SELECT *    
       ,@TotalRecords AS TotalRecords    
      FROM #REQ_AND_RES_TEMP_TABLE    
     END    
    ELSE    
     BEGIN    
      SELECT *    
       ,@TotalRecords AS TotalRecords    
      FROM #REQ_AND_RES_TEMP_TABLE    
      ORDER BY RequestTime DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS    
      FETCH NEXT @CurrentPageSize ROWS ONLY    
     END    
    
 DROP TABLE IF EXISTS #REQ_AND_RES_TEMP_TABLE    
      
 END TRY      
      
 BEGIN CATCH      
  DECLARE @StrProcedure_Name VARCHAR(500)      
   ,@ErrorDetail VARCHAR(1000)      
   ,@ParameterList VARCHAR(2000)      
      
  SET @StrProcedure_Name = ERROR_PROCEDURE()      
  SET @ErrorDetail = ERROR_MESSAGE()      
      
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name      
   ,@ErrorDetail = @ErrorDetail      
   ,@ParameterList = @ParameterList      
 END CATCH      
END