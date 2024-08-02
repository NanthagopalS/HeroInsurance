  
-- =============================================  
-- Author:  <Author, AMBI GUPTA>  
-- Create date: <Create Date,07-DEC-2022>  
-- Description: <Description,,CHECK USER BY MOBILE NO>  
--Identity_CheckUserByMobileNo '9980858553'  
-- =============================================  
CREATE    PROCEDURE [dbo].[Identity_CheckUserByMobileNo]   
(  
 @MobileNo VARCHAR(10) = NULL  
)  
AS  
BEGIN  
 BEGIN TRY  
    
  IF EXISTS (SELECT 1 FROM Identity_User WITH(NOLOCK) WHERE MobileNo = @MobileNo AND IsActive = 1)  
   BEGIN  
     -- Check if the RoleId is 2D6B0CE9-15C7-4839-93D1-8387944BC42F  
    IF EXISTS (SELECT 1 FROM Identity_User WITH(NOLOCK) WHERE MobileNo = @MobileNo AND RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F')  
    BEGIN  
     -- If the RoleId matches, retrieve the UserId  
     SELECT UserId FROM Identity_User WITH(NOLOCK) WHERE MobileNo = @MobileNo AND IsActive = 1;  
    END  
    ELSE  
    BEGIN  
     -- If the RoleId doesn't match, set UserId as 0  
     SELECT 0 AS UserId;  
    END  
   END  
  ELSE  
   BEGIN  
    SELECT null AS UserId;  
   END  
          
    
 END TRY                  
 BEGIN CATCH            
         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END
