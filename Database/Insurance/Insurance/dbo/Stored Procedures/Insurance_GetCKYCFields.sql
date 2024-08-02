-- =============================================  
-- Author:  <Author,,AMBI GUPTA>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
--[Insurance_GetCKYCFields]'16413879-6316-4C1E-93A4-FF8318B14D37',0,0  
-- =============================================  
CREATE   PROCEDURE [dbo].[Insurance_GetCKYCFields]    
@InsurerID varchar(50),   
@IsPOI bit = null,  
@IsCompany bit = null,
@IsDocumentUpload bit = null
AS  
BEGIN   
	BEGIN TRY  
		DECLARE @Section VARCHAR(50) = 'PersonalDetails'  
		IF(@IsPOI = 1)  
		BEGIN  
			SELECT Section  
				,FieldKey  
				,FieldText  
				,FieldType  
				,IsMandatory  
				,Validation  
				,IsMaster  
				,MasterData,MasterRef,ColumnRef  
				INTO #TMP  
				FROM Insurance_CKYCField with(nolock)   
				WHERE InsurerId=@InsurerID AND Section = @Section and IsActive=1 
		 
				IF(@IsCompany = 1)  
				BEGIN  
					IF(@InsurerID = 'E656D5D1-5239-4E48-9048-228C67AE3AC3')
					BEGIN
						SELECT * FROM #TMP WITH(NOLOCK) WHERE FieldKey != 'lastName' AND FieldKey != 'dateOfBirth'  
					END
					SELECT * FROM #TMP WITH(NOLOCK) WHERE FieldKey != 'dateOfBirth'  
				END  
				ELSE  
				BEGIN  
					SELECT * FROM #TMP WITH(NOLOCK) WHERE FieldKey != 'dateOfInsertion'  
				END  
		END  
		ELSE IF(@IsDocumentUpload = 1)
		BEGIN
			SET @Section = 'DocumentUploadDetails'
			SELECT Section  
			 ,FieldKey  
			 ,FieldText  
			 ,FieldType  
			 ,IsMandatory  
			 ,Validation  
			 ,IsMaster  
			 ,MasterData,MasterRef,
			 ColumnRef INTO #DOC_UP_TEMP  
			 FROM Insurance_CKYCField with(nolock)   
			 WHERE InsurerId=@InsurerID AND Section = @Section AND IsActive=1

			 SELECT * FROM #DOC_UP_TEMP WITH(NOLOCK)
		END
		ELSE  
		BEGIN  
			SET @Section = 'AddressDetails'  
			 SELECT Section  
			 ,FieldKey  
			 ,FieldText  
			 ,FieldType  
			 ,IsMandatory  
			 ,Validation  
			 ,IsMaster  
			 ,MasterData,MasterRef,
			 ColumnRef INTO #POA_TEMP  
			 FROM Insurance_CKYCField with(nolock)   
			 WHERE InsurerId=@InsurerID AND Section = @Section AND IsActive=1  

			 IF(@IsCompany = 1 AND @InsurerID = 'E656D5D1-5239-4E48-9048-228C67AE3AC3')  
			 BEGIN  
				SELECT * FROM #POA_TEMP WITH(NOLOCK) WHERE FieldKey != 'salutation' AND FieldKey != 'lastName' AND FieldKey != 'dateOfBirth'  AND FieldKey != 'fatherFirstName' AND FieldKey != 'fatherLastName' AND FieldKey != 'gender' AND FieldKey != 'photographUpload'
			 END  
			 ELSE IF(@IsCompany = 0 AND @InsurerID = 'E656D5D1-5239-4E48-9048-228C67AE3AC3')  
			 BEGIN  
				SELECT * FROM #POA_TEMP WITH(NOLOCK) WHERE FieldKey != 'dateOfInsertion'
			 END
			 ELSE  
			 BEGIN  
			  SELECT * FROM #POA_TEMP WITH(NOLOCK) 
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
