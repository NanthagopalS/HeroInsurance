CREATE TABLE [dbo].[Insurance_AddOnsPackage] (
    [AddOnId]       VARCHAR (50) NULL,
    [AddOns]        VARCHAR (50) NULL,
    [AddOnCode]     VARCHAR (50) NULL,
    [VehicleTypeId] VARCHAR (50) NULL,
    [PolicyTypeId]  VARCHAR (50) NULL,
    [InsurerId]     VARCHAR (50) NULL,
    [PackageName]   VARCHAR (50) NULL,
    [PackageFlag]   VARCHAR (50) NULL,
    [IsActive]      VARCHAR (2)  NULL,
    [CreatedBy]     VARCHAR (50) NULL,
    [CreatedOn]     DATETIME     CONSTRAINT [DF_Insurance_AddOnsPackage_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50) NULL,
    [UpdatedOn]     DATETIME     NULL
);

