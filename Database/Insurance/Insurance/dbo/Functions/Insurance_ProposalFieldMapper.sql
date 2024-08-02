-- SELECT [dbo].[Insurance_ProposalFieldMapper] ('customerName','HERO/ENQ/104181')

CREATE FUNCTION [dbo].[Insurance_ProposalFieldMapper](
    @DBKey VARCHAR(50),
	@LeadID VARCHAR(50)
)
RETURNS NVARCHAR(MAX)
AS 
BEGIN
	DECLARE @DefaultValue NVARCHAR(MAX), @VehicleNumber VARCHAR(20) , @RTOId VARCHAR(50)
	SELECT @VehicleNumber = VehicleNumber FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	SELECT @RTOId = RTOId FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID

	IF(@DBKey = 'firstName')
	BEGIN
		SELECT @DefaultValue = LeadName FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'middleName')
	BEGIN
		SELECT @DefaultValue = MiddleName FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'lastName')
	BEGIN
		SELECT @DefaultValue = LastName FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'dateOfBirth')
	BEGIN
		SELECT @DefaultValue = DOB FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'gender')
	BEGIN
		SELECT @DefaultValue = Gender FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'mobile')
	BEGIN
		SELECT @DefaultValue = PhoneNumber FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'emailId')
	BEGIN
		SELECT @DefaultValue = Email FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'flatNumber')
	BEGIN
		SELECT @DefaultValue = Address1 FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'streetNumber')
	BEGIN
		SELECT @DefaultValue = Address2 FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'street')
	BEGIN
		SELECT @DefaultValue = Address3 FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'pincode')
	BEGIN
		SELECT @DefaultValue = Pincode FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'pan')
	BEGIN
		SELECT @DefaultValue = PANNumber FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'aadhar')
	BEGIN
		SELECT @DefaultValue = AadharNumber FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'addressLine12')
	BEGIN
		SELECT @DefaultValue = Address1+ ' '+Address2 FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
		AND AddressType='PRIMARY'
	END
	ELSE IF(@DBKey = 'addressLine123')
	BEGIN
		SELECT @DefaultValue = Address1+ ' '+Address2+ ' '+Address3 FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
		AND AddressType='PRIMARY'
	END
	ELSE IF(@DBKey = 'city')
	BEGIN
		SELECT @DefaultValue = city FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'state')
	BEGIN
		SELECT @DefaultValue = state FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'country')
	BEGIN
		SELECT @DefaultValue = country FROM Insurance_LeadAddressDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'company')
	BEGIN
		SELECT @DefaultValue = CompanyName FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'gstNo')
	BEGIN
		SELECT @DefaultValue = GSTNo FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'engine')
	BEGIN
		SELECT @DefaultValue = REPLACE(engine,' ','') FROM Insurance_VehicleRegistration WITH(NOLOCK) WHERE regNo=@VehicleNumber
	END
	ELSE IF(@DBKey = 'chassis')
	BEGIN
		SELECT @DefaultValue = REPLACE(chassis,' ','') FROM Insurance_VehicleRegistration WITH(NOLOCK) WHERE regNo=@VehicleNumber
	END
	ELSE IF(@DBKey = 'vehicleNumber')
	BEGIN
		SELECT @DefaultValue = VehicleNumber FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
	ELSE IF(@DBKey = 'customerName')
	BEGIN
		DECLARE @FirstName VARCHAR(100), @LastName VARCHAR(100)
		SELECT @FirstName = LeadName, @LastName = LastName FROM Insurance_LeadDetails WHERE LeadId = @LeadID
		IF(@FirstName IS NOT NULL AND @LastName IS NOT NULL)
			BEGIN
				SELECT @DefaultValue = (LeadName + ' '+ LastName) FROM Insurance_LeadDetails WHERE LeadId = @LeadID
			END
		ELSE
			BEGIN
				SELECT @DefaultValue = LeadName FROM Insurance_LeadDetails WHERE LeadId = @LeadID
			END
	END
	ELSE IF(@DBKey = 'middleLastName')
	BEGIN
		DECLARE @MiddleName VARCHAR(100)
		SELECT @MiddleName = MiddleName, @LastName = LastName FROM Insurance_LeadDetails WHERE LeadId = @LeadID
		IF(@MiddleName IS NOT NULL AND @LastName IS NOT NULL)
			BEGIN
				SELECT @DefaultValue = (MiddleName + ' '+ LastName) FROM Insurance_LeadDetails WHERE LeadId = @LeadID
			END
		ELSE
			BEGIN
				SELECT @DefaultValue = LastName FROM Insurance_LeadDetails WHERE LeadId = @LeadID
			END
	END
	ELSE IF(@DBKey = 'dateOfIncorporation')
	BEGIN
		SELECT @DefaultValue = DateOfIncorporation FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END
		ELSE IF(@DBKey = 'salutation')
	BEGIN
		SELECT @DefaultValue = Salutation FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadID
	END

    RETURN @DefaultValue;
END;