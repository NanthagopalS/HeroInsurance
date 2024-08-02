CREATE TABLE [dbo].[Admin_AlertType] (
    [AlertTypeId] VARCHAR (100) CONSTRAINT [DF_Admin_AlertType_AlertTypeId] DEFAULT (newid()) NOT NULL,
    [AlertType]   VARCHAR (100) NULL,
    [IsActive]    BIT           CONSTRAINT [DF_Admin_AlertType_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]   VARCHAR (50)  NULL,
    [CreatedOn]   DATETIME      CONSTRAINT [DF_Admin_AlertType_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)  NULL,
    [UpdatedOn]   DATETIME      NULL,
    [IsDisplay]   BIT           NULL,
    CONSTRAINT [PK_Admin_AlertType_AlertTypeId] PRIMARY KEY CLUSTERED ([AlertTypeId] ASC)
);

