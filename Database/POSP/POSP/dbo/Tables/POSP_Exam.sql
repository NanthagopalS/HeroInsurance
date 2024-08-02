CREATE TABLE [dbo].[POSP_Exam] (
    [Id]                   VARCHAR (500) CONSTRAINT [DF_POSP_Exam_Id] DEFAULT (newid()) NOT NULL,
    [UserId]               VARCHAR (500) NOT NULL,
    [ExamStartDateTime]    DATETIME      CONSTRAINT [DF_POSP_Exam_ExamStartDateTime] DEFAULT (getdate()) NOT NULL,
    [ExamEndDateTime]      DATETIME      NULL,
    [CorrectAnswered]      INT           CONSTRAINT [DF_POSP_Exam_CorrectAnswered] DEFAULT ((0)) NOT NULL,
    [InCorrectAnswered]    INT           CONSTRAINT [DF_POSP_Exam_InCorrectAnswered] DEFAULT ((0)) NOT NULL,
    [SkippedAnswered]      INT           CONSTRAINT [DF_POSP_Exam_SkippedAnswered] DEFAULT ((0)) NOT NULL,
    [FinalResult]          FLOAT (53)    CONSTRAINT [DF_POSP_Exam_FinalResult] DEFAULT ((0)) NOT NULL,
    [IsCleared]            BIT           CONSTRAINT [DF_POSP_Exam_IsCleared] DEFAULT ((0)) NULL,
    [IsActive]             BIT           CONSTRAINT [DF_POSP_Exam_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]            VARCHAR (500) NULL,
    [CreatedOn]            DATETIME      CONSTRAINT [DF_POSP_Exam_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]            VARCHAR (500) NULL,
    [UpdatedOn]            DATETIME      NULL,
    [ExamIdealEndDateTime] DATETIME      NULL,
    [DocumentId]           VARCHAR (255) NULL,
    CONSTRAINT [PK_POSP_Exam_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

