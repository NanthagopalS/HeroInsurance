CREATE TABLE [MOTOR].[GoDigit_RTO] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [Sid]                   VARCHAR (100) NULL,
    [RegisteredCityName]    VARCHAR (100) NULL,
    [RegisteredStateName]   VARCHAR (100) NULL,
    [RegionCode]            VARCHAR (100) NULL,
    [PrivateVehicleZone]    VARCHAR (100) NULL,
    [CommercialVehicleZone] VARCHAR (100) NULL,
    [IsActive]              VARCHAR (100) NULL,
    [CreatedDate]           VARCHAR (100) NULL,
    [LastModifiedDate]      VARCHAR (100) NULL,
    [RTOId]                 VARCHAR (50)  NULL
);

