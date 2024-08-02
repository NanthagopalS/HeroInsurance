CREATE TABLE [dbo].[Insurance_ProductCategory] (
    [ProductCategoryId]   VARCHAR (100) CONSTRAINT [DF_Insurance_ProductCategory_ProductCategoryId] DEFAULT (newid()) NOT NULL,
    [ProductCategoryName] VARCHAR (100) NULL,
    [PriorityIndex]       INT           NULL,
    [IsActive]            BIT           CONSTRAINT [DF_Insurance_ProductCategory_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]           VARCHAR (50)  NULL,
    [CreatedOn]           DATETIME      CONSTRAINT [DF_Insurance_ProductCategory_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]           VARCHAR (50)  NULL,
    [UpdatedOn]           DATETIME      NULL,
    [Icon]                VARCHAR (100) NULL,
    CONSTRAINT [PK_Insurance_ProductCategory_ProductCategoryId] PRIMARY KEY CLUSTERED ([ProductCategoryId] ASC)
);

