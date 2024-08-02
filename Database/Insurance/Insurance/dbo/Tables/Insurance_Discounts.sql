CREATE TABLE [dbo].[Insurance_Discounts] (
    [DiscountId]    VARCHAR (50)  CONSTRAINT [DF__Insurance__Disco__32767D0B] DEFAULT (newid()) NOT NULL,
    [DiscountName]  VARCHAR (100) NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Insurance_Discounts_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (100) NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Insurance_Discounts_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdateBy]      VARCHAR (100) NULL,
    [UpdateOn]      DATETIME      NULL,
    [DiscountCode]  VARCHAR (50)  NULL,
    [VehicleTypeId] VARCHAR (50)  NULL,
    [PolicyTypeId]  VARCHAR (50)  NULL,
    CONSTRAINT [PK_Insurance_Discounts] PRIMARY KEY CLUSTERED ([DiscountId] ASC)
);

