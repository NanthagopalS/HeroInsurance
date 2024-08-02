CREATE TABLE [MOTOR].[Reliance_CV_ProductCode] (
    [ProductCode]   VARCHAR (20)  NULL,
    [Category]      VARCHAR (20)  NULL,
    [PolicyType]    VARCHAR (100) NULL,
    [SubCategoryId] VARCHAR (20)  NULL,
    [CarrierType]   INT           DEFAULT (NULL) NULL,
    [CreatedBy]     VARCHAR (100) NULL,
    [CreatedOn]     DATETIME      DEFAULT (getdate()) NULL
);

