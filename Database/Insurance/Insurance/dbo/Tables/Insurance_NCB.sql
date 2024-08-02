CREATE TABLE [dbo].[Insurance_NCB] (
    [NCBId]     UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [NCBName]   VARCHAR (100)    NULL,
    [NCBValue]  INT              NULL,
    [CreatedBy] VARCHAR (50)     NULL,
    [CreatedOn] DATETIME         DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)     NULL,
    [UpdatedOn] DATETIME         NULL,
    [IsActive]  BIT              NULL,
    PRIMARY KEY CLUSTERED ([NCBId] ASC)
);

