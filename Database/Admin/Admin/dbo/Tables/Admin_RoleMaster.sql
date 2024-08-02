CREATE TABLE [dbo].[Admin_RoleMaster] (
    [RoleId]      VARCHAR (100) DEFAULT (newid()) NOT NULL,
    [RoleName]    VARCHAR (100) NULL,
    [RoleTypeID]  VARCHAR (100) NULL,
    [RoleLevelID] VARCHAR (100) NULL,
    [CreatedBy]   VARCHAR (100) NULL,
    [CreatedOn]   DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (100) NULL,
    [UpdatedOn]   DATETIME      NULL,
    [BUId]        VARCHAR (100) NULL,
    [IsActive]    BIT           NULL,
    CONSTRAINT [PK__Identity_RoleId] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);

