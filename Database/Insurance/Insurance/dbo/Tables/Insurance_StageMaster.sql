CREATE TABLE [dbo].[Insurance_StageMaster] (
    [StageID]       VARCHAR (50) DEFAULT (newid()) NOT NULL,
    [Stage]         VARCHAR (50) NULL,
    [CreatedBy]     VARCHAR (50) NULL,
    [CreatedOn]     DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50) NULL,
    [UpdatedOn]     DATETIME     NULL,
    [PriorityIndex] INT          NULL,
    PRIMARY KEY CLUSTERED ([StageID] ASC)
);

