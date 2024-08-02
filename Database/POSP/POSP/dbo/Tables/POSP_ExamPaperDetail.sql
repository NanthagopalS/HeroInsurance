CREATE TABLE [dbo].[POSP_ExamPaperDetail] (
    [Id]             VARCHAR (500) CONSTRAINT [DF_POSP_ExamPaperDetail_Id] DEFAULT (newid()) NOT NULL,
    [ExamId]         VARCHAR (500) NOT NULL,
    [QuestionNo]     INT           NOT NULL,
    [QuestionId]     VARCHAR (500) NOT NULL,
    [StatusId]       VARCHAR (500) NOT NULL,
    [AnswerOptionId] VARCHAR (500) NULL,
    [IsActive]       BIT           CONSTRAINT [DF_POSP_ExamPaperDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]      VARCHAR (500) NULL,
    [CreatedOn]      DATETIME      CONSTRAINT [DF_POSP_ExamPaperDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]      VARCHAR (500) NULL,
    [UpdatedOn]      DATETIME      NULL,
    CONSTRAINT [PK_POSP_ExamPaperDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

