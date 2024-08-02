CREATE TABLE [dbo].[Insurance_ICConfig] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [InsurerId]     VARCHAR (50)   NULL,
    [PolicyTypeId]  VARCHAR (50)   NULL,
    [VehicleTypeId] VARCHAR (50)   NULL,
    [ConfigName]    VARCHAR (50)   NULL,
    [ConfigValue]   VARCHAR (1000) NULL,
    [CreatedBy]     INT            NULL,
    [CreatedOn]     DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]     INT            NULL,
    [UpdatedOn]     DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

