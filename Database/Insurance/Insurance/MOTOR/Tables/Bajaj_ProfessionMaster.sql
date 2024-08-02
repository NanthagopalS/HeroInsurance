CREATE TABLE [MOTOR].[Bajaj_ProfessionMaster] (
    [ProfessionId] VARCHAR (50)  DEFAULT (newid()) NULL,
    [Profession]   VARCHAR (100) NULL,
    [Code]         VARCHAR (50)  NULL,
    [IsActive]     BIT           NULL,
    [CreatedBy]    VARCHAR (100) NULL,
    [CreatedOn]    DATETIME      DEFAULT (getdate()) NULL,
    [UpdateBy]     VARCHAR (100) NULL,
    [UpdateOn]     DATETIME      NULL
);

