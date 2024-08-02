-- =============================================  
-- Author:  <Author,,AMBI GUPTA>  
-- Create date: <Create Date,,25-Nov-2022>  
-- Description: <Description,[GetInsurer]>  
--exec [dbo].[Insurance_GetInsurer_BKP] @IsCommercial=1
-- =============================================  
CREATE PROCEDURE [dbo].[Insurance_GetInsurer]  
@IsCommercial BIT = NULL
AS  
BEGIN  
 BEGIN TRY  
  -- SET NOCOUNT ON added to prevent extra result sets from  
  -- interfering with SELECT statements.  
  SET NOCOUNT ON;  
  IF(@IsCommercial is null)
  BEGIN
	SELECT CAST(InsurerId AS VARCHAR(50))InsurerId,InsurerName,Logo,IsActive,IsCommercialActive
	FROM Insurance_Insurer WITH(NOLOCK)   
	WHERE InsurerType='Motor'  --IsActive=1 AND 
	ORDER BY InsurerName
	
  END
  ELSE
  BEGIN
	IF(@IsCommercial = 1)
	BEGIN
		SELECT CAST(InsurerId AS VARCHAR(50))InsurerId,InsurerName,Logo,IsActive,IsCommercialActive
		FROM Insurance_Insurer WITH(NOLOCK)   
		WHERE InsurerType='Motor' AND IsCommercialActive = '1'
		ORDER BY InsurerName
	END
	ELSE IF(@IsCommercial = 0)
	BEGIN
		SELECT CAST(InsurerId AS VARCHAR(50))InsurerId,InsurerName,Logo,IsActive,IsCommercialActive 
		FROM Insurance_Insurer WITH(NOLOCK)   
		WHERE InsurerType='Motor' AND IsActive = '1' 
		ORDER BY InsurerName
	END
  END
   
   
 END TRY                  
 BEGIN CATCH            
         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END

