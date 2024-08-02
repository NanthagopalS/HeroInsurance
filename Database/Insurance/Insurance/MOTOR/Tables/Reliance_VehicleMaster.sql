CREATE TABLE [MOTOR].[Reliance_VehicleMaster] (
    [MakeID]           VARCHAR (20)  NULL,
    [MakeName]         VARCHAR (100) NULL,
    [ModelID]          VARCHAR (20)  NULL,
    [ModelName]        VARCHAR (100) NULL,
    [Variance]         VARCHAR (100) NULL,
    [Wheels]           VARCHAR (10)  NULL,
    [OperatedBy]       VARCHAR (50)  NULL,
    [CC]               VARCHAR (20)  NULL,
    [SeatingCapacity]  VARCHAR (10)  NULL,
    [CarryingCapacity] VARCHAR (10)  NULL,
    [VehTypeID]        VARCHAR (10)  NULL,
    [VehTypeName]      VARCHAR (50)  NULL,
    [CreatedBy]        VARCHAR (50)  CONSTRAINT [DF__Reliance___Creat__6A85CC04] DEFAULT ((1)) NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF__Reliance___Creat__6B79F03D] DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (50)  NULL,
    [UpdatedOn]        DATETIME      NULL,
    [VarientId]        VARCHAR (50)  NULL,
    [IsManuallyMapped] BIT           NULL
);





