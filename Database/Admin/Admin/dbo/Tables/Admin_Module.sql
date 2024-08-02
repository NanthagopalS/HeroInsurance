CREATE TABLE [dbo].[Admin_Module] (
    [ModuleId]        VARCHAR (100) CONSTRAINT [DF_Admin_Module_ModuleId] DEFAULT (newid()) NOT NULL,
    [ModuleName]      VARCHAR (100) NULL,
    [PriorityIndex]   INT           NULL,
    [AddOption]       BIT           CONSTRAINT [DF_Admin_Module_AddOption] DEFAULT ((1)) NULL,
    [EditOption]      BIT           CONSTRAINT [DF_Admin_Module_EditOption] DEFAULT ((1)) NULL,
    [ViewOption]      BIT           CONSTRAINT [DF_Admin_Module_ViewOption] DEFAULT ((1)) NULL,
    [DeleteOption]    BIT           CONSTRAINT [DF_Admin_Module_DeleteOption] DEFAULT ((1)) NULL,
    [DownloadOption]  BIT           CONSTRAINT [DF_Admin_Module_DownloadOption] DEFAULT ((1)) NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Admin_Module_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Admin_Module_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    [ModuleGroupName] VARCHAR (100) NULL,
    CONSTRAINT [PK_Admin_Module_ModuleId] PRIMARY KEY CLUSTERED ([ModuleId] ASC)
);

