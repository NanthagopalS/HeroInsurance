CREATE TABLE [dbo].[Insurance_RTO] (
    [RTOId]                 VARCHAR (50)  CONSTRAINT [DF__Insurance__RTOId] DEFAULT (newid()) NULL,
    [RTOCode]               VARCHAR (100) NULL,
    [CityId]                VARCHAR (50)  NULL,
    [CreatedBy]             VARCHAR (50)  NULL,
    [CreatedOn]             DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]             VARCHAR (50)  NULL,
    [UpdatedOn]             DATETIME      NULL,
    [RTOName]               VARCHAR (100) NULL,
    [CityName]              VARCHAR (100) NULL,
    [StateName]             VARCHAR (100) NULL,
    [PrivateVehicleZone]    VARCHAR (10)  NULL,
    [CommercialVehicleZone] VARCHAR (10)  NULL,
    [IsActive]              BIT           NULL
);



