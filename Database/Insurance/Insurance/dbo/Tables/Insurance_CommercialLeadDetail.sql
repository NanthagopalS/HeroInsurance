CREATE TABLE [dbo].[Insurance_CommercialLeadDetail] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [LeadId]                VARCHAR (100) NULL,
    [CarrierTypeId]         INT           NULL,
    [UsageNatureId]         INT           NULL,
    [VehicleBodyId]         INT           NULL,
    [IsHazardousVehicleUse] VARCHAR (10)  NULL,
    [IsTrailer]             VARCHAR (10)  NULL,
    [TrailerIDV]            VARCHAR (100) NULL,
    [CreatedBy]             VARCHAR (50)  NULL,
    [CreatedOn]             DATETIME      CONSTRAINT [DF_Insurance_CommercialLeadDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]             VARCHAR (50)  NULL,
    [UpdatedOn]             DATETIME      NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Insurance_CommercialLeadDetail_IsActive] DEFAULT ((1)) NULL,
    [VehicleCategoryId]     INT           NULL,
    [VehicleSubCategoryId]  INT           NULL,
    [UsageTypeId]           INT           NULL,
    [PCVVehicleCategoryId]  INT           NULL
);







