CREATE TABLE [dbo].[Insurance_AddOnsExtension] (
    [AddOnsExtensionId] VARCHAR (50)  DEFAULT (newid()) NOT NULL,
    [AddOnsExtension]   VARCHAR (100) NULL,
    [AddOnsId]          VARCHAR (50)  NULL,
    [IsActive]          BIT           NULL,
    [CreatedBy]         INT           NULL,
    [CreatedOn]         DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]         INT           NULL,
    [UpdatedOn]         DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([AddOnsExtensionId] ASC)
);

