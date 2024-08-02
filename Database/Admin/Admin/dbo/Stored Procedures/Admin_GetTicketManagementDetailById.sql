
       
CREATE    PROCEDURE [dbo].[Admin_GetTicketManagementDetailById]
(
	@TicketId VARCHAR(100) = NULL 
 )
AS 
        
BEGIN        
 BEGIN TRY


	Select HS.Id as TicketId, IU.POSPId, IU.UserName as POSPName, CT.ConcernTypeName as ConcernType, 
	SCT.SubConcernTypeName as SubConcernType, SubjectText as Summary, DetailText as SummaryDetails, 
	HS.Status, HS.Description, HS.DocumentId  
	from [HeroAdmin].[dbo].[Admin_HelpAndSupport] as HS WITH(NOLOCK)
	Left Join [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on IU.UserId = HS.UserId
	Left Join [HeroAdmin].[dbo].[Admin_ConcernType] as CT WITH(NOLOCK) on CT.ConcernTypeId = HS.ConcernTypeId
	LefT Join [HeroAdmin].[dbo].[Admin_SubConcernType] as SCT WITH(NOLOCK) on SCT.SubConcernTypeId = HS.SubConcernTypeId
	where Id = @TicketId

	

 END TRY                        
 BEGIN CATCH        
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END       
