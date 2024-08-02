-- =========================================================================================               
-- Author:  <Author, Ankit>            
-- Create date: <Create Date,19-Apr-2023>            
-- Description: <Description, Admin_DeActivateTicketManagementDetail>      
-- =========================================================================================               
 CREATE   PROCEDURE [dbo].[Admin_DeActivateTicketManagementDetail]             
 (            
	@SearchText VARCHAR(100) = null,
	@RelationshipManagerId VARCHAR(100) = null,
	@PolicyType VARCHAR(100) = null,      
	@StartDate VARCHAR(100) = null,    
	@EndDate VARCHAR(100) = null,    
	@CurrentPageIndex INT = 1,    
	@CurrentPageSize INT = 10    
 )            
AS     
 DECLARE 
	@SearchText1 VARCHAR(100) = @SearchText,
	@RelationshipManagerId1 VARCHAR(100) = @RelationshipManagerId,
	@PolicyType1 VARCHAR(100) = @PolicyType,    
	@StartDate1 VARCHAR(100) = @StartDate,        
	@EndDate1 VARCHAR(100) = @EndDate,        
	@CurrentPageIndex1 INT = @CurrentPageIndex,    
	@CurrentPageSize1 INT = @CurrentPageSize    
    
 DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1,     
		@NextPageIndex1 INT =  @CurrentPageIndex1 + 1    
       
     
 DECLARE @TempDataTable TABLE(    
  DeActivateTicketId VARCHAR(100),     
  POSPId VARCHAR(100),     
  POSPName VARCHAR(100),     
  MobileNumber VARCHAR(100),
  Email VARCHAR(100),
  RelationshipManager VARCHAR(100),     
  Status VARCHAR(100),
  Remark VARCHAR(100),
  PolicyType VARCHAR(100),
  CreatedOn VARCHAR(100)
 )    
   
BEGIN            
 BEGIN TRY            
          
   IF(@CurrentPageIndex1 = -1)    
   BEGIN    
      
  INSERT @TempDataTable    
	Select DA.Id as DeActivateTicketId, Da.DeActivatePospId as POSPId, IU.UserName as POSPName, IU.MobileNo as MobileNumber, IU.EmailId as Email,
	serUsr.UserName as RelationshipManager, DA.Status, DA.CreatedOn, DA.Remark, DA.PolicyType
	from [HeroAdmin].[dbo].[Admin_DeActivatePOSP] as DA WITH(NOLOCK)
		Left Join [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on IU.POSPId = DA.DeActivatePospId
		Left Join [HeroIdentity].[dbo].[Identity_UserDetail] as IUD WITH(NOLOCK) on IUD.UserId = IU.UserId 
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH(NOLOCK)ON IUD.ServicedByUserId = serUsr.UserId
  WHERE     
   (((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.UserName like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.MobileNo like '%' + @SearchText1 + '%'))
   or
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DA.DeActivatePospId like '%' + @SearchText1 + '%'))
   or
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DA.Id like '%' + @SearchText1 + '%')))
   AND     
    (CAST(DA.CreatedOn AS date) >=CAST(@StartDate AS date) OR ISNULL(@StartDate,'')='')
		 AND (CAST(DA.CreatedOn AS date) <=CAST(@EndDate AS date) OR ISNULL(@EndDate,'')='')
	AND ((@PolicyType IS NULL OR @PolicyType = '') OR (DA.PolicyType = @PolicyType1))
	AND ((@RelationshipManagerId1 IS NULL OR @RelationshipManagerId = '') OR (IUD.ServicedByUserId = @RelationshipManagerId1))
	ORDER BY DA.CreatedOn DESC     
        
  SELECT DeActivateTicketId, POSPId, POSPName, MobileNumber, Email, RelationshipManager, Status, CreatedOn
  FROM @TempDataTable ORDER BY CreatedOn desc    
      
  SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord     
    
   END    
   ELSE    
   BEGIN    
      
  INSERT @TempDataTable    
  Select DA.Id as DeActivateTicketId, Da.DeActivatePospId as POSPId, IU.UserName as POSPName, IU.MobileNo as MobileNumber, IU.EmailId as Email,
  serUsr.UserName as RelationshipManager, DA.Status, DA.CreatedOn, DA.Remark, DA.PolicyType
  from [HeroAdmin].[dbo].[Admin_DeActivatePOSP] as DA WITH(NOLOCK)
		Left Join [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on IU.POSPId = DA.DeActivatePospId
		Left Join [HeroIdentity].[dbo].[Identity_UserDetail] as IUD WITH(NOLOCK) on IUD.UserId = IU.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH(NOLOCK)ON IUD.ServicedByUserId = serUsr.UserId
	WHERE     
   (((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.UserName like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.MobileNo like '%' + @SearchText1 + '%'))   
   or
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DA.DeActivatePospId like '%' + @SearchText1 + '%'))
   or
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DA.Id like '%' + @SearchText1 + '%')))
   AND     
    (CAST(DA.CreatedOn AS date) >=CAST(@StartDate AS date) OR ISNULL(@StartDate,'')='')
		 AND (CAST(DA.CreatedOn AS date) <=CAST(@EndDate AS date) OR ISNULL(@EndDate,'')='')
	AND ((@PolicyType IS NULL OR @PolicyType = '') OR (DA.PolicyType = @PolicyType1))
	AND ((@RelationshipManagerId1 IS NULL OR @RelationshipManagerId = '') OR (IUD.ServicedByUserId = @RelationshipManagerId1))
   ORDER BY DA.CreatedOn DESC     
   
   
    
  Select DA.Id as DeActivateTicketId, Da.DeActivatePospId as POSPId, IU.UserName as POSPName, IU.MobileNo as MobileNumber, IU.EmailId as Email,
  serUsr.UserName as RelationshipManager, DA.Status, DA.CreatedOn, DA.Remark, DA.PolicyType
  from [HeroAdmin].[dbo].[Admin_DeActivatePOSP] as DA WITH(NOLOCK)
		Left Join [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on IU.POSPId = DA.DeActivatePospId
		Left Join [HeroIdentity].[dbo].[Identity_UserDetail] as IUD WITH(NOLOCK) on IUD.UserId = IU.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH(NOLOCK)ON IUD.ServicedByUserId = serUsr.UserId
	WHERE     
   (((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.UserName like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.MobileNo like '%' + @SearchText1 + '%'))  
   or 
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DA.DeActivatePospId like '%' + @SearchText1 + '%'))
   or
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DA.Id like '%' + @SearchText1 + '%')))
   AND     
   (CAST(DA.CreatedOn AS date) >=CAST(@StartDate AS date) OR ISNULL(@StartDate,'')='')
		 AND (CAST(DA.CreatedOn AS date) <=CAST(@EndDate AS date) OR ISNULL(@EndDate,'')='')
	AND ((@PolicyType IS NULL OR @PolicyType = '') OR (DA.PolicyType = @PolicyType1))
	AND ((@RelationshipManagerId1 IS NULL OR @RelationshipManagerId = '') OR (IUD.ServicedByUserId = @RelationshipManagerId1))
  ORDER BY DA.CreatedOn DESC  OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
      
    
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(DeActivateTicketId) from @TempDataTable) as TotalRecord    
    
   END      
 END TRY                            
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
         
END 
