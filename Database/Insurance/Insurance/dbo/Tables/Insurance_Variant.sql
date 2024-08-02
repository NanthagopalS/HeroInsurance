CREATE TABLE [dbo].[Insurance_Variant] (
    [VariantId]       VARCHAR (50)    CONSTRAINT [DF__Insurance__Varia__4316F928] DEFAULT (newid()) NULL,
    [VehicleCode]     VARCHAR (50)    NULL,
    [VariantName]     VARCHAR (100)   NULL,
    [ModelId]         VARCHAR (50)    NULL,
    [FuelId]          VARCHAR (50)    NULL,
    [VehicleTypeId]   VARCHAR (50)    NULL,
    [BodyType]        VARCHAR (50)    NULL,
    [SeatingCapacity] INT             NULL,
    [Power]           DECIMAL (18, 2) NULL,
    [CubicCapacity]   INT             NULL,
    [GVW]             INT             NULL,
    [NoOfWheels]      INT             NULL,
    [ABS]             CHAR (1)        NULL,
    [AirBags]         INT             NULL,
    [ExShowRoomPrice] DECIMAL (18, 2) NULL,
    [PriceYear]       VARCHAR (20)    NULL,
    [Production]      VARCHAR (50)    NULL,
    [Manufacturing]   VARCHAR (50)    NULL,
    [Length]          VARCHAR (50)    NULL,
    [IsActive]        BIT             NULL,
    [CreatedBy]       VARCHAR (50)    NULL,
    [CreatedOn]       DATETIME        CONSTRAINT [DF__Insurance__Creat__440B1D61] DEFAULT (getdate()) NULL,
    [UpdatedOn]       DATETIME        NULL,
    [UpdatedBy]       VARCHAR (50)    NULL,
    [H1_0_Variant_ID] VARCHAR (100)   NULL,
    [CVSubCategoryId] INT             NULL
);





