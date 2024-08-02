CREATE TABLE [MOTOR].[Bajaj_RelationMaster] (
    [RealtionId] VARCHAR (50)  DEFAULT (newid()) NULL,
    [Relation]   VARCHAR (100) NULL,
    [IsActive]   BIT           NULL,
    [CreatedBy]  INT           NULL,
    [CreatedOn]  DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]  INT           NULL,
    [UpdatedOn]  DATETIME      NULL
);

