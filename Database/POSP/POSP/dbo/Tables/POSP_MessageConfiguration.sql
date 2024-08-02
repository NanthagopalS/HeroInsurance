CREATE TABLE [dbo].[POSP_MessageConfiguration] (
    [Id]            VARCHAR (100) CONSTRAINT [DF_POSP_MessageConfiguration_Id] DEFAULT (newid()) NOT NULL,
    [MessageKey]    VARCHAR (500) NULL,
    [TitleValue]    VARCHAR (MAX) NULL,
    [Subtitle1]     VARCHAR (MAX) NULL,
    [Subtitle2]     VARCHAR (MAX) NULL,
    [Subtitle3]     VARCHAR (MAX) NULL,
    [IsActive]      BIT           CONSTRAINT [DF_POSP_MessageConfiguration_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_POSP_MessageConfiguration_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    [PriorityIndex] INT           NULL,
    CONSTRAINT [PK_POSP_MessageConfiguration_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

