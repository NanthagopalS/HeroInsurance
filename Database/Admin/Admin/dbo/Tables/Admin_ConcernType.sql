CREATE TABLE [dbo].[Admin_ConcernType] (
    [ConcernTypeId]   VARCHAR (100) CONSTRAINT [DF_Admin_ConcernType_ConcernTypeId] DEFAULT (newid()) NOT NULL,
    [ConcernTypeName] VARCHAR (100) NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Admin_ConcernType_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Admin_ConcernType_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    CONSTRAINT [PK_Admin_ConcernType_ConcernTypeId] PRIMARY KEY CLUSTERED ([ConcernTypeId] ASC)
);

