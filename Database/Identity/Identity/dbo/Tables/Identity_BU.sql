CREATE TABLE [dbo].[Identity_BU] (
    [BUID]        INT           IDENTITY (1, 1) NOT NULL,
    [RoleTypeID]  INT           NULL,
    [BULevelID]   INT           NULL,
    [BUName]      VARCHAR (50)  NULL,
    [IsActive]    BIT           DEFAULT ((0)) NULL,
    [CreatedBy]   VARCHAR (50)  NULL,
    [CreatedDate] DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)  NULL,
    [UpdatedDate] DATETIME      NULL,
    [RoleId]      VARCHAR (100) NULL,
    CONSTRAINT [PK_Identity_BU] PRIMARY KEY CLUSTERED ([BUID] ASC)
);

