CREATE TABLE [dbo].[Insurance_CommercialVehicleUsageType] (
    [UsageTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [UsageTypeName] VARCHAR (20)  NULL,
    [CreatedBy]     VARCHAR (100) NULL,
    [CreatedOn]     DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([UsageTypeId] ASC)
);

