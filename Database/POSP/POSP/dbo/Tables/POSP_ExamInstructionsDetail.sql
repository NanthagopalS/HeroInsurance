CREATE TABLE [dbo].[POSP_ExamInstructionsDetail] (
    [Id]                VARCHAR (500) CONSTRAINT [DF_POSP_ExamInstructionsDetail_Id] DEFAULT (newid()) NOT NULL,
    [InstructionDetail] VARCHAR (MAX) NULL,
    [PriorityIndex]     INT           NULL,
    [IsActive]          BIT           CONSTRAINT [DF_POSP_ExamInstructionsDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]         VARCHAR (500) NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_POSP_ExamInstructionsDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (500) NULL,
    [UpdatedOn]         DATETIME      NULL,
    CONSTRAINT [PK_POSP_ExamInstructionsDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

