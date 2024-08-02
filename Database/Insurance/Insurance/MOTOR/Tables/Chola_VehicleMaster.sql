CREATE TABLE [MOTOR].[Chola_VehicleMaster] (
    [Make]             VARCHAR (50)  NULL,
    [MakeCode]         VARCHAR (20)  NULL,
    [VehicleModel]     VARCHAR (100) NULL,
    [ModelCode]        VARCHAR (20)  NULL,
    [BodyType]         VARCHAR (50)  NULL,
    [CubicCapacity]    VARCHAR (50)  NULL,
    [KiloWatt]         VARCHAR (50)  NULL,
    [SeatingCapacity]  VARCHAR (50)  NULL,
    [FuelType]         VARCHAR (50)  NULL,
    [NumStateCode]     VARCHAR (10)  NULL,
    [State]            VARCHAR (10)  NULL,
    [StateName]        VARCHAR (100) NULL,
    [City]             VARCHAR (100) NULL,
    [ExShowRoom]       VARCHAR (20)  NULL,
    [VehicleTypeId]    VARCHAR (50)  NULL,
    [VarientId]        VARCHAR (50)  NULL,
    [CreatedOn]        DATETIME      DEFAULT (getdate()) NULL,
    [CreatedBy]        VARCHAR (50)  DEFAULT ((1)) NULL,
    [UpdatedOn]        DATETIME      NULL,
    [UpdatedBy]        VARCHAR (50)  NULL,
    [VehicleClass]     VARCHAR (20)  NULL,
    [PolicyTypeId]     VARCHAR (50)  NULL,
    [IsManuallyMapped] BIT           NULL
);





