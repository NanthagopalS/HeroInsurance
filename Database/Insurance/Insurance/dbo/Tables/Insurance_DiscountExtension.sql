CREATE TABLE [dbo].[Insurance_DiscountExtension] (
    [DiscountExtensionId]  VARCHAR (50)  DEFAULT (newid()) NOT NULL,
    [DiscountExtension]    VARCHAR (100) NULL,
    [DiscountId]           VARCHAR (50)  NULL,
    [IsActive]             BIT           NULL,
    [CreatedBy]            INT           NULL,
    [CreatedOn]            DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]            INT           NULL,
    [UpdatedOn]            DATETIME      NULL,
    [DiscountValueInWords] VARCHAR (100) NULL,
    [DiscountOrder]        INT           NULL,
    PRIMARY KEY CLUSTERED ([DiscountExtensionId] ASC)
);

