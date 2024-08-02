CREATE TABLE [dbo].[POSP_CardStage] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (50) NULL,
    [StageValue] NVARCHAR (30) NULL,
    [IsActive]   BIT           DEFAULT ((1)) NULL,
    [CreatedOn]  DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedOn]  DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

