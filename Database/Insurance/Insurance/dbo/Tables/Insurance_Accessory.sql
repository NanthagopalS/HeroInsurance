CREATE TABLE [dbo].[Insurance_Accessory] (
    [AccessoryId]   VARCHAR (50)    CONSTRAINT [DF__Insurance__Acces__2F9A1060] DEFAULT (newid()) NOT NULL,
    [Accessory]     VARCHAR (100)   NULL,
    [IsActive]      BIT             CONSTRAINT [DF_Insurance_Accessory_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (100)   NULL,
    [CreatedOn]     DATETIME        CONSTRAINT [DF_Insurance_Accessory_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (100)   NULL,
    [UpdatedOn]     DATETIME        NULL,
    [AccessoryCode] VARCHAR (50)    NULL,
    [MinValue]      DECIMAL (18, 2) NULL,
    [MaxValue]      DECIMAL (18, 2) NULL,
    [VehicleTypeId] VARCHAR (50)    NULL,
    [PolicyTypeId]  VARCHAR (50)    NULL,
    CONSTRAINT [PK_Insurance_Accessory] PRIMARY KEY CLUSTERED ([AccessoryId] ASC)
);

