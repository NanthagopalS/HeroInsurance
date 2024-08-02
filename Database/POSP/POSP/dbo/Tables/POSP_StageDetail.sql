CREATE TABLE [dbo].[POSP_StageDetail] (
    [StageId]           VARCHAR (100) CONSTRAINT [DF_POSP_StageDetail_StageId] DEFAULT (newid()) NOT NULL,
    [StageName]         VARCHAR (50)  NOT NULL,
    [PriorityIndex]     INT           NOT NULL,
    [IsActive]          BIT           CONSTRAINT [DF_POSP_StageDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]         VARCHAR (500) NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_POSP_StageDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (500) NULL,
    [UpdatedOn]         DATETIME      NULL,
    [GroupNumber]       INT           NULL,
    [VisibleForFilters] BIT           NULL,
    CONSTRAINT [PK_POSP_StageDetail_StageId] PRIMARY KEY CLUSTERED ([StageId] ASC)
);





