CREATE TABLE [dbo].[POSP_ExamQuestionPaperOptionMaster] (
    [Id]              VARCHAR (500) CONSTRAINT [DF_POSP_ExamQuestionPaperOptionMaster_Id] DEFAULT (newid()) NOT NULL,
    [QuestionId]      VARCHAR (500) NOT NULL,
    [OptionIndex]     INT           NOT NULL,
    [OptionValue]     VARCHAR (100) NOT NULL,
    [IsCorrectAnswer] BIT           NOT NULL,
    [IsActive]        BIT           CONSTRAINT [DF_POSP_ExamQuestionPaperOptionMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]       VARCHAR (500) NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_POSP_ExamQuestionPaperOptionMaster_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (500) NULL,
    [UpdatedOn]       DATETIME      NULL,
    CONSTRAINT [PK_POSP_ExamQuestionPaperOptionMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

