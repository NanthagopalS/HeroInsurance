CREATE TABLE [dbo].[Insurance_PCVVehicleCategory] (
    [PCVVehicleCategoryId] INT           IDENTITY (1, 1) NOT NULL,
    [CategoryName]         VARCHAR (50)  NULL,
    [CreatedBy]            VARCHAR (100) NULL,
    [CreatedOn]            DATETIME      NULL,
    [IsActive]             BIT           DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([PCVVehicleCategoryId] ASC)
);

