CREATE TABLE [dbo].[Insurance_CommercialVehicleUsageNature] (
    [UsageNatureId]   INT           IDENTITY (1, 1) NOT NULL,
    [UsageNatureName] VARCHAR (100) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Insurance_CommercialVehicleUsageNature_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Insurance_CVUsageNature_IsActive] DEFAULT ((1)) NULL
);

