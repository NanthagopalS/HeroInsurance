CREATE TABLE [dbo].[Admin_RecipientType] (
    [RecipientTypeId] VARCHAR (100) CONSTRAINT [DF_Admin_RecipientType_RecipientTypeId] DEFAULT (newid()) NOT NULL,
    [RecipientType]   VARCHAR (100) NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Admin_RecipientType_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Admin_RecipientType_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    [IsDisplay]       BIT           NULL,
    CONSTRAINT [PK_Admin_RecipientType_RecipientTypeId] PRIMARY KEY CLUSTERED ([RecipientTypeId] ASC)
);

