CREATE TABLE [MOTOR].[HDFC_RelationMaster] (
    [SL_NO]             INT          IDENTITY (1, 1) NOT NULL,
    [TXT_RELATION_DESC] VARCHAR (25) NULL,
    [CreatedOn]         DATETIME     NULL,
    [UpdatedBy]         VARCHAR (50) NULL,
    [UpdatedOn]         DATETIME     NULL,
    [IsActive]          BIT          DEFAULT ((1)) NULL,
    [IsCommercial]      BIT          CONSTRAINT [IsCom] DEFAULT ((0)) NULL
);





