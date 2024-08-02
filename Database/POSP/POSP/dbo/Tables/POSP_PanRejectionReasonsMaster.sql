CREATE TABLE [dbo].[POSP_PanRejectionReasonsMaster] (
    [RejectionMessage] VARCHAR (100) NULL,
    [IsActive]         BIT           CONSTRAINT [DF_POSP_PanRejectionReasonsMaster_IsActive] DEFAULT ((1)) NULL,
    [Id]               INT           IDENTITY (1, 1) NOT NULL
);

