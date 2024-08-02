CREATE TABLE [dbo].[Insurance_CommercialVehicleBodyType] (
    [VehicleBodyId]   INT           IDENTITY (1, 1) NOT NULL,
    [VehicleBodyName] VARCHAR (100) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Insurance_CommercialVehicleBodyType_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Insurance_CommercialVehicleBodyType_IsActive] DEFAULT ((1)) NULL
);

