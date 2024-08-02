      
             
CREATE     PROCEDURE [dbo].[Admin_GetPOSPDetailsByIDToDeActivate]      
(      
 @POSPId VARCHAR(100) = NULL       
 )      
AS       
              
BEGIN              
 BEGIN TRY      
      
      
 select IU.POSPId, IU.UserName as POSPName, IU.MobileNo as MobileNumber, UD.AadhaarNumber, UD.PAN,    
 CONCAT(UAD.AddressLine1 , ' ', UAD.AddressLine2, ', ', City.CityName, ', ', State.StateName, ', ',UAD.Pincode) as Address, DA.EmailAttachmentDocumentId,      
   DA.Remark, DA.Status, DA.PolicyType, IU.EmailId,  serUsr.UserName, DA.IsNocGenerated,
   DA.BusinessTeamApprovalAttachmentDocumentId from [HeroAdmin].[dbo].[Admin_DeActivatePOSP] as DA WITH(NOLOCK)      
   Left Join [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on IU.POSPId = DA.DeActivatePospId      
   Left Join [HeroIdentity].[dbo].[Identity_UserDetail] as UD WITH(NOLOCK) on UD.UserId = IU.UserId      
   Left Join [HeroIdentity].[dbo].[Identity_UserAddressDetail] as UAD WITH(NOLOCK) on UAD.UserId = IU.UserId     
   Left Join [HeroIdentity].[dbo].[Identity_City] City on City.CityId = UAD.CityId    
   Left Join [HeroIdentity].[dbo].[Identity_State] State on State.StateId = UAD.StateId 
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH(NOLOCK)ON UD.ServicedByUserId = serUsr.UserId
   where DA.DeActivatePospId = @POSPId     
      
    
 END TRY                              
 BEGIN CATCH              
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                          
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                          
  SET @ErrorDetail=ERROR_MESSAGE()                                          
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                           
 END CATCH              
END       
