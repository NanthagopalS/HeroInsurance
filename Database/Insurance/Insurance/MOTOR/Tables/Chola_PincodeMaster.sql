CREATE TABLE [MOTOR].[Chola_PincodeMaster] (
    [StateCode]        VARCHAR (10)  NULL,
    [StateDesc]        VARCHAR (100) NULL,
    [CityDistrictCode] VARCHAR (10)  NULL,
    [CityDistrictDesc] VARCHAR (100) NULL,
    [AreaVillageCode]  VARCHAR (20)  NULL,
    [AreaVillageDesc]  VARCHAR (100) NULL,
    [Pincode]          VARCHAR (10)  NULL,
    [PincodeLocality]  VARCHAR (100) NULL,
    [CreatedBy]        VARCHAR (50)  DEFAULT ((1)) NULL,
    [CreatedOn]        DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (50)  NULL,
    [UpdatedOn]        DATETIME      NULL
);

