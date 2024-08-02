CREATE TABLE [dbo].[Insurance_PaymentStatus] (
    [PaymentId]     VARCHAR (50)  CONSTRAINT [DF_Insurance_PaymentStatus] DEFAULT (newid()) NOT NULL,
    [PaymentStatus] VARCHAR (100) NULL,
    [IsActive]      BIT           NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    CONSTRAINT [PK_Insurance_Payment_PaymentId] PRIMARY KEY CLUSTERED ([PaymentId] ASC)
);



