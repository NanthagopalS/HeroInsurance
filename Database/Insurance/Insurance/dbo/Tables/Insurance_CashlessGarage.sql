CREATE TABLE [dbo].[Insurance_CashlessGarage] (
    [GarageId]      VARCHAR (50)   CONSTRAINT [DF__Insurance__Garag__53D770D6] DEFAULT (newid()) NOT NULL,
    [WorkshopName]  VARCHAR (100)  NULL,
    [FullAddress]   NVARCHAR (MAX) NULL,
    [City]          VARCHAR (100)  NULL,
    [State]         VARCHAR (100)  NULL,
    [Pincode]       VARCHAR (6)    NULL,
    [Latitude]      VARCHAR (20)   NULL,
    [Longitude]     VARCHAR (20)   NULL,
    [ProductType]   VARCHAR (100)  NULL,
    [EmailId]       VARCHAR (100)  NULL,
    [MobileNumber]  VARCHAR (10)   NULL,
    [ContactPerson] VARCHAR (100)  NULL,
    [IsActive]      BIT            CONSTRAINT [DF__Insurance__IsAct__54CB950F] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (100)  NULL,
    [CreatedOn]     DATETIME       CONSTRAINT [DF_Insurance_CashlessGarage_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (100)  NULL,
    [UpdatedOn]     DATETIME       NULL,
    [InsurerId]     VARCHAR (50)   NULL
);

