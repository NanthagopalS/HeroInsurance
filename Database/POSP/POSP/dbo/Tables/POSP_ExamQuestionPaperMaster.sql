CREATE TABLE [dbo].[POSP_ExamQuestionPaperMaster] (
    [Id]             VARCHAR (500) CONSTRAINT [DF_POSP_ExamQuestionPaperMaster_Id] DEFAULT (newid()) NOT NULL,
    [SequenceNo]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [ExamModuleType] VARCHAR (500) NOT NULL,
    [QuestionValue]  VARCHAR (500) NOT NULL,
    [IsActive]       BIT           CONSTRAINT [DF_POSP_ExamQuestionPaperMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]      VARCHAR (500) NULL,
    [CreatedOn]      DATETIME      CONSTRAINT [DF_POSP_ExamQuestionPaperMaster_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]      VARCHAR (500) NULL,
    [UpdatedOn]      DATETIME      NULL,
    CONSTRAINT [PK_POSP_ExamQuestionPaperMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

