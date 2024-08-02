﻿       
CREATE     PROCEDURE [dbo].[Admin_GetRenewalDetail]       
(  
	@POSPId VARCHAR(100),
	@PolicyNo VARCHAR(100),
	@CustomerName VARCHAR(100),	
	@PolicyType VARCHAR(100) = NULL,	
	@StartDate VARCHAR(100),  
	@EndDate VARCHAR(100),  
	@PageIndex INT = 1	   
 ) 

AS      
	DECLARE @RowsOfPage  INT
	SET @RowsOfPage = 10     
	
BEGIN      
 BEGIN TRY 	
	BEGIN
		Select 0 as POSPId, 0 as PolicyNo, 0 as PolicyType, LeadName as CustomerName, 0 as Premium, 
		CreatedOn as StartDate, GetDate() AS 'ExpiryDate'
		from [HeroInsurance].[dbo].[Insurance_LeadDetails] WITH(NOLOCK)

		 WHERE		

		 (((@CustomerName IS NULL OR @CustomerName = '') OR (LeadName like @CustomerName + '%'))) AND
	--(((@PolicyType IS NULL OR @PolicyType = '') OR (PolicyType = @PolicyType))) AND
	(((@StartDate IS NULL OR @StartDate = '') AND (@EndDate IS NULL OR @EndDate = '')) OR  CAST(CreatedOn as date) BETWEEN
		CAST(@StartDate as date) and CAST(@EndDate as date))
		order by LeadName  OFFSET (@PageIndex-1)*@RowsOfPage ROWS FETCH NEXT @RowsOfPage ROWS ONLY  

	END

	
	
 END TRY                      
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END 
