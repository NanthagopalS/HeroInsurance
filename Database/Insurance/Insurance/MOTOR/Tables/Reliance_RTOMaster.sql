CREATE TABLE [MOTOR].[Reliance_RTOMaster] (
    [ModelRegionID]     VARCHAR (20)   NULL,
    [RegionName]        NVARCHAR (MAX) NULL,
    [RegionCode]        VARCHAR (20)   NULL,
    [StateID]           VARCHAR (20)   NULL,
    [CityOrVillageName] NVARCHAR (MAX) NULL,
    [DistrictName]      NVARCHAR (MAX) NULL,
    [StateName]         NVARCHAR (MAX) NULL,
    [ModelZoneName]     VARCHAR (20)   NULL,
    [CreatedBy]         VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]         DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (50)   NULL,
    [UpdatedOn]         DATETIME       NULL,
    [RTOId]             VARCHAR (50)   NULL
);

