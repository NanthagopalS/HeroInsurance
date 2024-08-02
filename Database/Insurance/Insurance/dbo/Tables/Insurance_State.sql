CREATE TABLE [dbo].[Insurance_State] (
    [StateId]   VARCHAR (50)  NULL,
    [StateName] VARCHAR (100) NULL,
    [CreatedBy] VARCHAR (50)  NULL,
    [CreatedOn] DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)  NULL,
    [UpdatedOn] DATETIME      NULL
);

