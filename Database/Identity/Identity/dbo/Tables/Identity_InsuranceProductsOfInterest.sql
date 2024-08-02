CREATE TABLE [dbo].[Identity_InsuranceProductsOfInterest] (
    [Id]            VARCHAR (100) CONSTRAINT [DF_Identity_InsuranceProductsOfInterest_Id] DEFAULT (newid()) NOT NULL,
    [ProductName]   VARCHAR (100) NULL,
    [PriorityIndex] INT           NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Identity_InsuranceProductsOfInterest_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Identity_InsuranceProductsOfInterest_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    CONSTRAINT [PK_Identity_InsuranceProductsOfInterest_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

