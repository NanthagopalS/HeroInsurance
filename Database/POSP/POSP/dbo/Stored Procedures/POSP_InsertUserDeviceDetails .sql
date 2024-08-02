-- =========================================================================================         
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,5-Apr-2023>      
-- Description: <Description, POSP_InsertUserDeviceDetails    >
-- =========================================================================================         
 CREATE   PROCEDURE [dbo].[POSP_InsertUserDeviceDetails ]       
 (      
	  @UserId VARCHAR(100) = Null,  
	  @MobileDeviceId VARCHAR(100) = Null,  
	  @BrowserId VARCHAR(100),
	  @GfcToken VARCHAR(100)
 )      
AS      

BEGIN      
 BEGIN TRY 

	BEGIN

	Declare @InsertUser VARCHAR(100)
	Set @InsertUser = (Select UserId from [HeroPOSP].[dbo].[POSP_UserDeviceDetail] where UserId = @UserId)

	if (@InsertUser is not null)
		Update [HeroPOSP].[dbo].[POSP_UserDeviceDetail]
		Set MobileDeviceId = @MobileDeviceId, BrowserId = @BrowserId, GfcToken = @GfcToken, UpdatedOn = GETDATE()
		where UserId = @UserId

	else
		Insert Into [HeroPOSP].[dbo].[POSP_UserDeviceDetail]
		(UserId, MobileDeviceId, BrowserId, GfcToken)
		values (@UserId, @MobileDeviceId, @BrowserId, @GfcToken )

	END
	
	END TRY                      
 BEGIN CATCH        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                   
 END CATCH      
   
END 
