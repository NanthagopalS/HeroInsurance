
CREATE   PROCEDURE [dbo].[Insurance_GetTATAMasterData] @FieldType VARCHAR(20)
AS
BEGIN
	IF (@FieldType = 'GENDER')
		SELECT 'Male' NAME,'Male' VALUE
		UNION ALL
		SELECT 'Female' NAME,'Female' VALUE

	ELSE IF (@FieldType = 'MARITALSTATUS')
		SELECT 'Married' NAME,'Married' VALUE
		UNION ALL
		SELECT 'Single' NAME,'Single' VALUE

	ELSE IF (@FieldType = 'SALUTATION')
		SELECT Title Name, Title Value
		FROM MOTOR.TATA_SalutationMaster WITH (NOLOCK)

	ELSE IF (@FieldType = 'NOMINEERELATION')
		SELECT Nominee_Relationship Name,Nominee_Relationship Value
		FROM MOTOR.TATA_RelationShipMaster WITH (NOLOCK)

	ELSE IF (@FieldType = 'FINANCIER')
		SELECT DISTINCT FinancierName AS Name, FinancierName AS Value
		FROM MOTOR.TATA_FinancierMaster WITH (NOLOCK)

	ELSE IF (@FieldType = 'PINCODE')
		SELECT Pincode AS Name, Pincode AS Value
		FROM MOTOR.TATA_PincodeMaster WITH (NOLOCK)
		ORDER BY StateId ASC

	ELSE IF (@FieldType = 'STATE')
		SELECT DISTINCT StateName AS Name, StateName AS Value
		FROM MOTOR.TATA_PincodeMaster WITH (NOLOCK)
		ORDER BY StateName ASC

	ELSE IF (@FieldType = 'CITY')
		SELECT DISTINCT CityName AS Name, CityName AS Value --CityId as Id
		FROM MOTOR.TATA_PincodeMaster WITH (NOLOCK)
		ORDER BY CityName ASC

	ELSE IF (@FieldType = 'OCCUPATION')
		SELECT Occupation as Name, Occupation as Value from MOTOR.TATA_OccupationMaster

	ELSE IF (@FieldType = 'FINANCIERTYPE')
		SELECT 'Hire Purchase' NAME,'Hire Purchase' VALUE
		UNION ALL
		SELECT 'Hypothecation' NAME,'Hypothecation' VALUE
		UNION ALL
		SELECT 'Lease agreement' NAME,'Lease agreement' VALUE

	ELSE IF(@FieldType='YESNO')    
		SELECT 'Yes' NAME, 'Yes' VALUE   
		UNION ALL  
		SELECT 'No' NAME, 'No' VALUE 
END