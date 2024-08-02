-- =============================================    
-- Author:  <Author,,AMBI GUPTA>    
-- Create date: <Create Date,,25-NOV-2022>    
-- Description: <Description [Insurance_GetPreviousPolicyType]>    
-- Modified : <Added CommercialVehicleCheck to display PC and TP for all the scenarios,Firoz S>
--[Insurance_GetPreviousPolicyType] '2018','1','6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056'    
-- =============================================    
CREATE  PROCEDURE [dbo].[Insurance_GetPreviousPolicyType]    
(    
 @RegDate VARCHAR(12)=NULL,    
 @IsBrandNew bit = NULL,    
 @VehicleTypeId VARCHAR(50) = NULL    
)    
AS    
BEGIN    
	BEGIN TRY    
		SET NOCOUNT ON;    
    
		DECLARE @VEHICLEINSURANCEUPTO VARCHAR(20) = NULL, @Tenure INT    
		IF(@VehicleTypeId='2d566966-5525-4ed7-bd90-bb39e8418f39')    
		BEGIN    
			SET @Tenure = 3    
		END    
		ELSE IF(@VehicleTypeId='6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056')    
		BEGIN    
			SET @Tenure = 5    
		END      
    
		IF(@VehicleTypeId='88a807b3-90e4-484b-b5d2-65059a8e1a91')
		BEGIN
			SELECT PreviousPolicyTypeId,PreviousPolicyType    
			FROM Insurance_PreviousPolicyType WITH(NOLOCK)    
			WHERE PreviousPolicyTypeId IN ('517D8F9C-F532-4D45-8034-ABECE46693E3','2AA7FDCA-9E36-4A8D-9583-15ADA737574B') --PACKAGE COMPREHENSIVE/SATP    
			AND ISACTIVE=1    
			ORDER BY PreviousPolicyType  
		END
		ELSE
		BEGIN
			IF(@IsBrandNew=1)    
			BEGIN    
				SELECT PreviousPolicyTypeId,PreviousPolicyType    
				FROM Insurance_PreviousPolicyType WITH(NOLOCK)    
				WHERE PreviousPolicyTypeId IN ('517D8F9C-F532-4D45-8034-ABECE46693E3') --PACKAGE COMPREHENSIVE    
				AND ISACTIVE=1    
				ORDER BY PreviousPolicyType    
			END    
			ELSE    
			BEGIN      
				IF(DATEDIFF(YY, CAST(@RegDate+'/01/01' AS DATE), CAST(GETDATE() AS DATE))  < @Tenure)    
				BEGIN    
					SELECT PreviousPolicyTypeId,PreviousPolicyType    
					FROM Insurance_PreviousPolicyType WITH(NOLOCK)    
					WHERE PreviousPolicyTypeId IN ('48B01586-C66A-4A4A-AAFB-3F07F8A31896') --SAOD    
					AND ISACTIVE=1    
					ORDER BY PreviousPolicyType    
				END    
				ELSE IF(DATEDIFF(YY, CAST(@RegDate+'/01/01' AS DATE),CAST(GETDATE() AS DATE)) >= @Tenure)    
				BEGIN    
					SELECT PreviousPolicyTypeId,PreviousPolicyType    
					FROM Insurance_PreviousPolicyType WITH(NOLOCK)    
					WHERE PreviousPolicyTypeId IN ('517D8F9C-F532-4D45-8034-ABECE46693E3','2AA7FDCA-9E36-4A8D-9583-15ADA737574B') --PACKAGE COMPREHENSIVE/SATP    
					AND ISACTIVE=1    
					ORDER BY PreviousPolicyType    
				END    
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
