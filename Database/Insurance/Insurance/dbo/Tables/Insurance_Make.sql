CREATE TABLE [dbo].[Insurance_Make] (
    [MakeId]    VARCHAR (50)   NULL,
    [MakeName]  VARCHAR (100)  NULL,
    [CreatedBy] INT            NULL,
    [CreatedOn] DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy] INT            NULL,
    [UpdatedOn] DATETIME       NULL,
    [IsPopular] BIT            NULL,
    [Logo]      NVARCHAR (MAX) NULL
);

