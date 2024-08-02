CREATE TABLE [dbo].[Admin_SubConcernType] (
    [SubConcernTypeId]   VARCHAR (100) CONSTRAINT [DF_Admin_SubConcernType_SubConcernTypeId] DEFAULT (newid()) NOT NULL,
    [SubConcernTypeName] VARCHAR (100) NULL,
    [IsActive]           BIT           CONSTRAINT [DF_Admin_SubConcernType_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]          VARCHAR (50)  NULL,
    [CreatedOn]          DATETIME      CONSTRAINT [DF_Admin_SubConcernType_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]          VARCHAR (50)  NULL,
    [UpdatedOn]          DATETIME      NULL,
    [ConcernTypeId]      VARCHAR (100) NULL,
    CONSTRAINT [PK_Admin_SubConcernType_SubConcernTypeId] PRIMARY KEY CLUSTERED ([SubConcernTypeId] ASC)
);

