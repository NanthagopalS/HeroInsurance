CREATE TABLE [dbo].[Admin_HelpAndSupport] (
    [Id]               VARCHAR (100) CONSTRAINT [DF_Admin_HelpAndSupport_Id] DEFAULT (newid()) NOT NULL,
    [ConcernTypeId]    VARCHAR (100) NULL,
    [SubConcernTypeId] VARCHAR (100) NULL,
    [SubjectText]      VARCHAR (500) NULL,
    [DetailText]       VARCHAR (MAX) NULL,
    [DocumentId]       VARCHAR (MAX) NULL,
    [IsActive]         BIT           CONSTRAINT [DF_Admin_HelpAndSupport_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (100) NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_Admin_HelpAndSupport_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (100) NULL,
    [UpdatedOn]        DATETIME      NULL,
    [Status]           VARCHAR (100) NULL,
    [UserId]           VARCHAR (100) NULL,
    [Description]      VARCHAR (500) NULL,
    CONSTRAINT [PK_Admin_HelpAndSupport_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

