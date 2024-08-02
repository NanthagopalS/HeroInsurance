CREATE TABLE [MOTOR].[Oriental_voluntaryExcessMaster] (
    [Code]        VARCHAR (50) NULL,
    [Description] VARCHAR (50) NULL,
    [CreatedBy]   VARCHAR (50) DEFAULT ((1)) NULL,
    [CreatedOn]   DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50) NULL,
    [UpdatedOn]   DATETIME     NULL
);

