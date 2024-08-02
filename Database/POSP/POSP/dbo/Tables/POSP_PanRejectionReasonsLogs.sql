CREATE TABLE [dbo].[POSP_PanRejectionReasonsLogs] (
    [Id]                VARCHAR (50)  CONSTRAINT [DF_POSP_PanRejectionReasonsLogs_Id] DEFAULT (newid()) NULL,
    [UserId]            VARCHAR (100) NULL,
    [RejectionReasonId] VARCHAR (300) NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_POSP_PanRejectionReasonsLogs_CreatedOn] DEFAULT (getdate()) NULL,
    [RejectedBy]        VARCHAR (100) NULL,
    [RejectedPanNumber] VARCHAR (15)  NULL
);

