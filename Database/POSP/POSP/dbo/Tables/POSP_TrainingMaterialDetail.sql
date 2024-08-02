CREATE TABLE [dbo].[POSP_TrainingMaterialDetail] (
    [Id]                 VARCHAR (500) CONSTRAINT [DF_POSP_TrainingMaterialDetail_Id] DEFAULT (newid()) NOT NULL,
    [TrainingModuleType] VARCHAR (500) NULL,
    [MaterialFormatType] VARCHAR (10)  NULL,
    [VideoDuration]      VARCHAR (20)  NULL,
    [LessonNumber]       VARCHAR (500) NULL,
    [LessonTitle]        VARCHAR (500) NULL,
    [DocumentFileName]   VARCHAR (500) NULL,
    [DocumentId]         VARCHAR (100) NULL,
    [PriorityIndex]      INT           CONSTRAINT [DF_POSP_TrainingMaterialDetail_PriorityIndex] DEFAULT ((0)) NULL,
    [IsActive]           BIT           CONSTRAINT [DF_POSP_TrainingMaterialDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]          VARCHAR (500) NULL,
    [CreatedOn]          DATETIME      CONSTRAINT [DF_POSP_TrainingMaterialDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]          VARCHAR (500) NULL,
    [UpdatedOn]          DATETIME      NULL,
    CONSTRAINT [PK_POSP_TrainingMaterialDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

