CREATE     PROCEDURE [dbo].[Admin_GetPOSPDetailsByDeactiveTicketId]       
 (      
	@DeactivateId VARCHAR(100) = Null	
 ) 
 As
 Begin  

	BEGIN TRY
select IU.POSPId,IU.UserId,IU.EmailId,DA.Id as TicketID,PV.Name as POSPName,PV.PanNumber,AD.AddressLine1,AD.AddressLine2,AD.Pincode,
CT.CityName as City,ST.StateName as State,IU.CreatedOn as JoinDate
from [HeroAdmin].[dbo].[Admin_DeActivatePOSP] as DA WITH(NOLOCK) inner join 
[HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on DA.DeActivatePospId=IU.POSPId inner join
[HeroIdentity].[dbo].[Identity_UserAddressDetail] as AD WITH(NOLOCK) on IU.UserId=AD.UserId inner join
[HeroIdentity].[dbo].[Identity_PanVerification] as PV WITH(NOLOCK) on PV.UserId=IU.UserId inner join
[HeroIdentity].[dbo].[Identity_City] as CT WITH(NOLOCK) on AD.CityId = CT.CityId inner join
[HeroIdentity].[dbo].[Identity_State] as ST WITH(NOLOCK) on AD.StateId = ST.StateId 
WHERE AD.IsActive=1 and DA.ID = @DeactivateId and PV.IsActive = 1

	END TRY

	
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
   
END