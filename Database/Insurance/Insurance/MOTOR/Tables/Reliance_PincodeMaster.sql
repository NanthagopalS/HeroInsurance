CREATE TABLE [MOTOR].[Reliance_PincodeMaster] (
    [StateID]           VARCHAR (20)  NULL,
    [StateName]         VARCHAR (100) NULL,
    [DistrictID]        VARCHAR (20)  NULL,
    [DistrictName]      VARCHAR (100) NULL,
    [CityOrVillageID]   VARCHAR (20)  NULL,
    [CityOrVillageName] VARCHAR (100) NULL,
    [AreaID]            VARCHAR (20)  NULL,
    [AreaName]          VARCHAR (100) NULL,
    [Pincode]           VARCHAR (10)  NULL,
    [CreatedBy]         VARCHAR (50)  DEFAULT ((1)) NULL,
    [CreatedOn]         DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (50)  NULL,
    [UpdatedOn]         DATETIME      NULL
);

