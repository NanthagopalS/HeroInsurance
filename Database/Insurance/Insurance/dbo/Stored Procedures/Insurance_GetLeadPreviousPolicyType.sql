CREATE   PROCEDURE [dbo].[Insurance_GetLeadPreviousPolicyType]        
@VehicleTypeId VARCHAR(50) = NULL,   
@VehicleNumber VARCHAR(20) = NULL, 
@PreviousPolicyTypeId VARCHAR(50) = NULL,
@YearId VARCHAR(50) = NULL
AS        
BEGIN        
	BEGIN TRY     
		SET NOCOUNT ON;
		DECLARE @Tenure INT, @Year VARCHAR(10)

		IF(@YearId = '0')
		BEGIN
			SELECT @Year = RIGHT(regDate,4) FROM Insurance_VehicleRegistration WITH(NOLOCK) WHERE regNo=@VehicleNumber
			SELECT @YearId = YearId FROM Insurance_Year WITH(NOLOCK) WHERE Year = @Year
		END
		ELSE
		BEGIN
			SELECT @Year = Year FROM Insurance_Year WITH(NOLOCK) WHERE YearId = @YearId
		END


		IF(@VehicleTypeId='2d566966-5525-4ed7-bd90-bb39e8418f39')
		BEGIN
			SET @Tenure = 3
		END
		ELSE IF(@VehicleTypeId='6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056')
		BEGIN
			SET @Tenure = 5
		END

		IF(@PreviousPolicyTypeId = '0')
		BEGIN
			IF(DATEDIFF(YY, CAST(@Year+'/01/01' AS DATE), CAST(GETDATE() AS DATE))  <= @Tenure)
			BEGIN
				SELECT @PreviousPolicyTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3' 
			END
			ELSE IF(DATEDIFF(YY, CAST(@Year+'/01/01' AS DATE),CAST(GETDATE() AS DATE)) > @Tenure)
			BEGIN
				SELECT @PreviousPolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B' 
			END
		END
		
		SELECT @PreviousPolicyTypeId AS PreviousPolicyTypeId,
				@YearId AS YearId

	END TRY        
	BEGIN CATCH                   
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
		SET @ErrorDetail=ERROR_MESSAGE()                                    
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                
	END CATCH        
END