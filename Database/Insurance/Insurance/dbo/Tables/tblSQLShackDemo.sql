CREATE TABLE [dbo].[tblSQLShackDemo] (
    [S.No.] INT              IDENTITY (0, 1) NOT NULL,
    [value] UNIQUEIDENTIFIER NULL,
    [Date]  DATETIME         DEFAULT (getdate()) NULL
);

