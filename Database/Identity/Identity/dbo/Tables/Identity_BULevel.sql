CREATE TABLE [dbo].[Identity_BULevel] (
    [BULevelId]     VARCHAR (100) CONSTRAINT [DF_Identity_BULevel_BULevelId] DEFAULT (newid()) NOT NULL,
    [BULevelName]   VARCHAR (20)  NULL,
    [PriorityIndex] INT           NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Identity_BULevel_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Identity_BULevel_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    CONSTRAINT [PK_Identity_BULevel_BULevelId] PRIMARY KEY CLUSTERED ([BULevelId] ASC)
);

