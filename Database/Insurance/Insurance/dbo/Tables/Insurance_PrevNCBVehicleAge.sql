CREATE TABLE [dbo].[Insurance_PrevNCBVehicleAge] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [VehicleAgeYear] VARCHAR (100) NULL,
    [PrevNCB]        INT           NULL,
    [NCBId]          VARCHAR (100) NULL,
    [CreatedBy]      VARCHAR (50)  NULL,
    [CreatedOn]      DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]      VARCHAR (50)  NULL,
    [UpdatedOn]      DATETIME      NULL,
    [IsActive]       BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

