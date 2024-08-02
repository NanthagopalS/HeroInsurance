CREATE TABLE [dbo].[Insurance_InsuranceType] (
    [InsuranceTypeId]    VARCHAR (100)    DEFAULT (newid()) NOT NULL,
    [InsuranceName]      VARCHAR (80)     NULL,
    [InsuranceType]      VARCHAR (50)     NULL,
    [IsActive]           BIT              NULL,
    [CreatedBy]          UNIQUEIDENTIFIER NULL,
    [CreatedOn]          DATETIME         DEFAULT (getdate()) NULL,
    [UpdatedBy]          UNIQUEIDENTIFIER NULL,
    [UpdatedOn]          DATETIME         NULL,
    [IsOfferApplicable]  BIT              NULL,
    [OfferContent]       NVARCHAR (2000)  NULL,
    [SignzyVehicleClass] VARCHAR (50)     NULL,
    [ProductCategoryId]  VARCHAR (50)     NULL,
    [ImageURL]           VARCHAR (500)    NULL,
    [IsEnable]           BIT              NULL,
    [PriorityIndex]      INT              NULL,
    PRIMARY KEY CLUSTERED ([InsuranceTypeId] ASC)
);

