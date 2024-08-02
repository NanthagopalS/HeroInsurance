CREATE TABLE [dbo].[Insurance_KeyHighlightsDescription] (
    [InsurerId]         VARCHAR (50)  NULL,
    [PolicyTypeId]      VARCHAR (50)  NULL,
    [VehicleTypeId]     VARCHAR (50)  NULL,
    [SelfVideoClaims]   VARCHAR (MAX) NULL,
    [SelfDescription]   VARCHAR (MAX) NULL,
    [GarageDescription] VARCHAR (MAX) NULL,
    [CreatedBy]         VARCHAR (50)  NULL,
    [CreatedOn]         DATETIME      NULL,
    [UpdatedBy]         VARCHAR (50)  NULL,
    [UpdatedOn]         DATETIME      NULL
);

