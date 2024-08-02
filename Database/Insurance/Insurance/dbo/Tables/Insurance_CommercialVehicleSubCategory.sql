CREATE TABLE [dbo].[Insurance_CommercialVehicleSubCategory] (
    [SubCategoryId]            INT           IDENTITY (1, 1) NOT NULL,
    [SubCategoryName]          VARCHAR (100) NULL,
    [CategoryId]               VARCHAR (100) NULL,
    [CreatedBy]                VARCHAR (50)  NULL,
    [CreatedOn]                DATETIME      NULL,
    [UpdatedBy]                VARCHAR (50)  NULL,
    [UpdatedOn]                DATETIME      NULL,
    [IsActive]                 BIT           CONSTRAINT [DF_Insurance_CommercialVehicleSubCategory_IsActive] DEFAULT ((1)) NULL,
    [CVBodyTypeOptions]        VARCHAR (100) NULL,
    [CVUsageNatureOptions]     VARCHAR (100) NULL,
    [AskForHazardusVehicleUse] BIT           DEFAULT ((0)) NULL,
    [AskForIfTrailer]          BIT           DEFAULT ((0)) NULL,
    [AskCarrierType]           BIT           DEFAULT ((0)) NULL,
    [UsageTypeId]              VARCHAR (100) NULL,
    [PCVVehicleCategoryId]     VARCHAR (100) NULL
);



