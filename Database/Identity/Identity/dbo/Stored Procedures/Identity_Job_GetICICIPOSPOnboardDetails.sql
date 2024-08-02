CREATE   PROCEDURE [dbo].[Identity_Job_GetICICIPOSPOnboardDetails]     
AS      
BEGIN      
	BEGIN TRY      
		
		SELECT TOP 100 USERTB.UserId, 
		ISNULL(USERTB.UserName,'')UserName, 
		ISNULL(USERTB.EmailId,'')EmailId, 
		ISNULL(USERTB.MobileNo,'')MobileNo,
		ISNULL(USERTB.POSPId,'')POSPId,
		ISNULL(USERINFO.AadhaarNumber,'')AadhaarNumber, 
		ISNULL(USERINFO.PAN,'')PAN,
		ISNULL(USERTB.Gender,'')Gender,
		ISNULL(USERINFO.DateofBirth,'')DateofBirth,
		ISNULL(CITY.CityName,'')CityName,
		ISNULL(ROL.ROLENAME,'') RoleName,IMID,
		ISNULL(USERSTATE.StateName,'')StateName,
		ISNULL(USERADDRESS.AddressLine1,'')AddressLine1,
		ISNULL(USERADDRESS.AddressLine2,'')AddressLine2,
		ISNULL(USERADDRESS.Pincode,'')Pincode
		FROM Identity_User USERTB WITH(NOLOCK)
		LEFT JOIN Identity_UserDetail USERINFO WITH(NOLOCK) ON USERINFO.UserId = USERTB.UserId      
		LEFT JOIN Identity_UserAddressDetail ADDRESSTB WITH(NOLOCK) ON ADDRESSTB.UserId = USERTB.UserId   
		LEFT JOIN Identity_City CITY WITH(NOLOCK) ON CITY.CityId = ADDRESSTB.CityId
		LEFT JOIN Identity_RoleMaster ROL WITH(NOLOCK) ON USERTB.RoleId= ROL.RoleId
		LEFT JOIN Identity_UserAddressDetail USERADDRESS WITH(NOLOCK) ON USERTB.UserId = USERADDRESS.UserId
		LEFT JOIN Identity_State USERSTATE WITH(NOLOCK) ON USERSTATE.StateId = USERADDRESS.StateId
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] PA WITH(NOLOCK) ON PA.UserId = USERTB.UserId
		WHERE ISNULL(ROL.ROLENAME,'') ='POSP' AND ISNULL(IMID,'') ='' AND PA.AgreementId IS NOT NULL AND (PA.IsRevoked IS NULL OR PA.IsRevoked = 0) AND USERTB.IsActive='1' AND USERTB.GENDER != '' AND USERTB.GENDER IS NOT NULL
		AND CITY.CityName != '' AND CITY.CityName IS NOT NULL AND USERSTATE.StateName != '' AND USERSTATE.StateName IS NOT NULL
		ORDER BY USERINFO.CreatedOn DESC

	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END 