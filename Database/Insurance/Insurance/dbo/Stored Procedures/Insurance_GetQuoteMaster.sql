--[Insurance_GetQuoteMaster] '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056',''
CREATE PROCEDURE [dbo].[Insurance_GetQuoteMaster]
(
	@VehicleTypeId VARCHAR(50) = NULL,
	@PolicyTypeId VARCHAR(50) = NULL
)
AS
BEGIN
	BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @TBLPACOVER AS TABLE (
			PACoverId VARCHAR(50),
			CoverName VARCHAR(50)
		);

		IF(ISNULL( @PolicyTypeId, '' ) = '' )  
		BEGIN  
			SELECT @PolicyTypeId = PreviousPolicyTypeId FROM Insurance_PreviousPolicyType WITH(NOLOCK) WHERE PreviousPolicyTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3' --COMPREHENSIVE PACKAGE  
		END

		SELECT DiscountId,DiscountName FROM Insurance_Discounts WITH (NOLOCK) 
		WHERE VehicleTypeId=@VehicleTypeId AND PolicyTypeId=@PolicyTypeId AND IsActive = 1

		SELECT AddOnId,AddOns,IsRecommended,Description FROM Insurance_AddOns WITH(NOLOCK) 
		WHERE VehicleTypeId=@VehicleTypeId AND PolicyTypeId=@PolicyTypeId AND IsActive = 1  ORDER BY IsRecommended DESC

		SELECT AccessoryId,Accessory,MinValue,MaxValue FROM Insurance_Accessory WITH(NOLOCK) 
		WHERE VehicleTypeId=@VehicleTypeId AND PolicyTypeId=@PolicyTypeId AND IsActive = 1 

		--IF(@VehicleTypeId ='2d566966-5525-4ed7-bd90-bb39e8418f39') --4W
		--BEGIN
		--	INSERT INTO @TBLPACOVER
		--	SELECT PACoverId,CoverName FROM Insurance_PACover WITH(NOLOCK) 
		--	WHERE IsActive = 1 AND PACoverId IN (
		--				'CED94D1E-88B3-4A64-AF21-72F50C276827',
		--				'8FB2819E-E63D-4A72-9D21-78E579F95C9E',
		--				'9B79EB34-3FD8-4B47-9EFE-A70D21E2D933')
		--END
		--ELSE IF(@VehicleTypeId ='6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056') --2W
		--BEGIN
		--	INSERT INTO @TBLPACOVER
		--	SELECT PACoverId,CoverName FROM Insurance_PACover WITH(NOLOCK) 
		--	WHERE IsActive = 1 AND PACoverId IN (
		--				'CED94D1E-88B3-4A64-AF21-72F50C276827',
		--				'9B79EB34-3FD8-4B47-9EFE-A70D21E2D933',
		--				'46C98A32-4C0D-492C-BA20-A4B9A243F4FA')
		--END

		--SELECT * FROM @TBLPACOVER
		--ORDER BY CoverName
		SELECT PACoverId,CoverName,IsDefault FROM Insurance_PACover WITH(NOLOCK) 
		WHERE VehicleTypeId=@VehicleTypeId AND PolicyTypeId=@PolicyTypeId AND IsActive = 1

		SELECT EXT.PACoverExtensionId, EXT.PACoverExtension, EXT.PACoverId
		FROM Insurance_PACoverExtension EXT WITH(NOLOCK)
		--INNER JOIN @TBLPACOVER PA ON PA.PACoverId = EXT.PACoverId 
		INNER JOIN Insurance_PACover PA WITH(NOLOCK) ON PA.PACoverId = EXT.PACoverId 
		WHERE PA.PolicyTypeId=@PolicyTypeId AND PA.VehicleTypeId=@VehicleTypeId AND EXT.IsActive = 1 
		ORDER BY CoverName

		SELECT EXT.DiscountExtensionId, EXT.DiscountExtension, EXT.DiscountId
		FROM Insurance_DiscountExtension EXT WITH(NOLOCK)
		INNER JOIN Insurance_Discounts PA WITH(NOLOCK) ON PA.DiscountId = EXT.DiscountId 
		WHERE PA.PolicyTypeId=@PolicyTypeId AND PA.VehicleTypeId=@VehicleTypeId AND EXT.IsActive = 1 ORDER BY DiscountOrder ASC

		SELECT EXT.AddOnsExtensionId, EXT.AddOnsExtension, EXT.AddOnsId
		FROM Insurance_AddOnsExtension EXT WITH(NOLOCK)
		INNER JOIN Insurance_AddOns PA WITH(NOLOCK) ON PA.AddOnId = EXT.AddOnsId 
		WHERE PA.PolicyTypeId=@PolicyTypeId AND PA.VehicleTypeId=@VehicleTypeId AND EXT.IsActive = 1 


	END TRY                
	BEGIN CATCH           
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                        
	END CATCH
END
