  
    
    
    
-- =============================================    
-- Author:  <Author,,VISHAL KANJARIYA>    
-- Create date: <Create Date,,30-NOV-2022>    
-- Description: <Description,,UPDATE USER ADDRESS DETAIL>    
-- =============================================    
CREATE PROCEDURE [dbo].[Identity_UpdateUserAddressDetail]     
(    
	@UserId varchar(200),
	@AddressLine1 varchar(200),
	@AddressLine2 varchar(200),
	@Pincode varchar(200),
	@CityId varchar(200),
	@StateId varchar(200)
)    
AS    
BEGIN    
 BEGIN TRY  
	
	  IF EXISTS(SELECT DISTINCT Id FROM [dbo].[Identity_UserAddressDetail] WITH(NOLOCK) WHERE UserId = @UserId)    
	  BEGIN    
	   UPDATE [dbo].[Identity_UserAddressDetail] SET     
		AddressLine1 = @AddressLine1,     
		AddressLine2 = @AddressLine2,     
		Pincode = @Pincode,     
		CityId = @CityId,     
		StateId = @StateId, UpdatedOn = GETDATE()    
	   WHERE    
		UserId = @UserId    
	  END    
	  ELSE    
	  BEGIN    
	   INSERT INTO [dbo].[Identity_UserAddressDetail] (UserId, AddressLine1, AddressLine2, Pincode, CityId, StateId)     
	   VALUES     
	   (@UserId, @AddressLine1, @AddressLine2, @Pincode, @CityId, @StateId)    
	  END 
  
 END TRY                    
 BEGIN CATCH              
           
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList    
      
 END CATCH    
    
END 
