CREATE TABLE [dbo].[Identity_Module] (
    [ModuleId]      VARCHAR (100) CONSTRAINT [DF_Identity_Module_ModuleId] DEFAULT (newid()) NOT NULL,
    [ModuleName]    VARCHAR (100) NULL,
    [PriorityIndex] INT           NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Identity_Module_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Identity_Module_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    CONSTRAINT [PK_Identity_Module_ModuleId] PRIMARY KEY CLUSTERED ([ModuleId] ASC)
);

