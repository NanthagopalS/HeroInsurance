CREATE TABLE [dbo].[Admin_StampInstruction] (
    [Id]          VARCHAR (100) CONSTRAINT [DF_Admin_StampInstruction_Id] DEFAULT (newid()) NOT NULL,
    [SrNo]        VARCHAR (100) NULL,
    [Instruction] VARCHAR (100) NULL,
    [IsActive]    BIT           CONSTRAINT [DF_Admin_StampInstruction_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]   VARCHAR (50)  NULL,
    [CreatedOn]   DATETIME      CONSTRAINT [DF_Admin_StampInstruction_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)  NULL,
    [UpdatedOn]   DATETIME      NULL,
    CONSTRAINT [PK_Admin_Instruction_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

