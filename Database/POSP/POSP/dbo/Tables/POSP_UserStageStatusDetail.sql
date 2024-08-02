CREATE TABLE [dbo].[POSP_UserStageStatusDetail] (
    [Id]        VARCHAR (100) CONSTRAINT [DF_POSP_UserStageStatusDetail_Id] DEFAULT (newid()) NOT NULL,
    [UserId]    VARCHAR (100) NOT NULL,
    [StageId]   VARCHAR (100) NOT NULL,
    [IsActive]  BIT           CONSTRAINT [DF_POSP_UserStageStatusDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy] VARCHAR (50)  NULL,
    [CreatedOn] DATETIME      CONSTRAINT [DF_POSP_UserStageStatusDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)  NULL,
    [UpdatedOn] DATETIME      NULL,
    CONSTRAINT [PK_POSP_UserStageStatusDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

