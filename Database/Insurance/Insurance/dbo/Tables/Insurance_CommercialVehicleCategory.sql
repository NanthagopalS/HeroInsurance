CREATE TABLE [dbo].[Insurance_CommercialVehicleCategory] (
    [CategoryId]     INT           IDENTITY (1, 1) NOT NULL,
    [CategoryName]   VARCHAR (100) NULL,
    [IsOtherDetails] BIT           CONSTRAINT [DF_Insurance_CommercialVehicleCategory_IsOtherDetails] DEFAULT ((1)) NULL,
    [CreatedBy]      VARCHAR (50)  NULL,
    [CreatedOn]      DATETIME      NULL,
    [UpdatedBy]      VARCHAR (50)  NULL,
    [UpdatedOn]      DATETIME      NULL,
    [IsActive]       BIT           CONSTRAINT [DF_Insurance_CommercialVehicleCategory_IsActive] DEFAULT ((1)) NULL
);

