CREATE TABLE [dbo].[Insurance_SharePlanDetails] (
    [Id]           VARCHAR (50)   DEFAULT (newid()) NOT NULL,
    [UserId]       VARCHAR (50)   NULL,
    [LeadId]       VARCHAR (50)   NULL,
    [MobileNumber] VARCHAR (10)   NULL,
    [EmailId]      VARCHAR (100)  NULL,
    [Type]         VARCHAR (50)   NULL,
    [Link]         NVARCHAR (MAX) NULL,
    [CreatedBy]    VARCHAR (100)  NULL,
    [CreatedOn]    DATETIME       DEFAULT (getdate()) NULL,
    [StageId]      VARCHAR (50)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

