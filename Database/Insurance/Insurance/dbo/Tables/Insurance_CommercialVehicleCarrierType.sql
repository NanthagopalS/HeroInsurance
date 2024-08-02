CREATE TABLE [dbo].[Insurance_CommercialVehicleCarrierType] (
    [CarrierTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [CarrierTypeName] VARCHAR (100) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Insurance_CommercialCarrierType_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Insurance_CommercialCarrierType_IsActive] DEFAULT ((1)) NULL
);

