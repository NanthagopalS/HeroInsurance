CREATE TABLE [MOTOR].[ITGI_OccupationMaster] (
    [OCCUPATION]     NVARCHAR (255) NULL,
    [CreatedBy]      VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]      DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]      VARCHAR (50)   NULL,
    [UpdatedOn]      DATETIME       NULL,
    [OccupationCode] VARCHAR (50)   NULL
);



