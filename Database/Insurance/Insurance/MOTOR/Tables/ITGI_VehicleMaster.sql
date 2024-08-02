CREATE TABLE [MOTOR].[ITGI_VehicleMaster] (
    [MAKE_CODE]        NVARCHAR (255) NULL,
    [MANUFACTURE]      NVARCHAR (255) NULL,
    [MODEL]            NVARCHAR (255) NULL,
    [VARIANT]          NVARCHAR (255) NULL,
    [CC]               FLOAT (53)     NULL,
    [SEATING_CAPACITY] FLOAT (53)     NULL,
    [CONTRACT_TYPE]    NVARCHAR (255) NULL,
    [FUEL_TYPE]        NVARCHAR (255) NULL,
    [FROM_YEAR]        FLOAT (53)     NULL,
    [TO_YEAR]          FLOAT (53)     NULL,
    [VariantId]        VARCHAR (50)   NULL,
    [CreatedBy]        VARCHAR (50)   NULL,
    [CreatedOn]        DATETIME       NULL,
    [UpdatedOn]        DATETIME       NULL,
    [UpdatedBy]        VARCHAR (50)   NULL,
    [IsCommercial]     BIT            NULL,
    [SubClass]         VARCHAR (50)   NULL,
    [ExShowroomPrice]  VARCHAR (150)  NULL,
    [GVW]              VARCHAR (50)   NULL,
    [IsManuallyMapped] BIT            NULL
);





