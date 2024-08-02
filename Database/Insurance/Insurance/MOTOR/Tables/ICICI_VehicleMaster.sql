CREATE TABLE [MOTOR].[ICICI_VehicleMaster] (
    [VariantId]              VARCHAR (50)   NULL,
    [VehicleClassCode]       FLOAT (53)     NULL,
    [VehicleSubClassDesc]    NVARCHAR (255) NULL,
    [VehicleSubClassCode]    NVARCHAR (255) NULL,
    [VehicleManufactureCode] FLOAT (53)     NULL,
    [Manufacture]            NVARCHAR (255) NULL,
    [VehicleModelCode]       FLOAT (53)     NULL,
    [VehicleModel]           NVARCHAR (255) NULL,
    [CubicCapacity]          FLOAT (53)     NULL,
    [SeatingCapacity]        FLOAT (53)     NULL,
    [CarringCapacity]        FLOAT (53)     NULL,
    [VehicleModelStatus]     NVARCHAR (255) NULL,
    [ActiveFlag]             NVARCHAR (255) NULL,
    [ModelBuild]             NVARCHAR (255) NULL,
    [MaxPrice]               NVARCHAR (255) NULL,
    [MinimumPrice]           NVARCHAR (255) NULL,
    [CarCategory]            NVARCHAR (255) NULL,
    [FuelType]               NVARCHAR (255) NULL,
    [Segment]                NVARCHAR (255) NULL,
    [GVW]                    FLOAT (53)     NULL,
    [NumberOfWheels]         FLOAT (53)     NULL,
    [ExShowroomSlab]         NVARCHAR (255) NULL,
    [CreatedOn]              DATETIME       DEFAULT (getdate()) NULL,
    [CreatedBy]              VARCHAR (50)   DEFAULT ((1)) NULL,
    [UpdatedOn]              DATETIME       NULL,
    [UpdatedBy]              VARCHAR (50)   NULL,
    [IsManuallyMapped]       BIT            NULL
);



