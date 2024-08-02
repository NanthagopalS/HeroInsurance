CREATE TABLE [dbo].[Insurance_ProposalField] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [InsurerId]   VARCHAR (500)   NULL,
    [Section]     VARCHAR (50)    NULL,
    [FieldKey]    VARCHAR (50)    NULL,
    [FieldText]   VARCHAR (50)    NULL,
    [FieldType]   VARCHAR (50)    NULL,
    [IsMandatory] BIT             NULL,
    [Validation]  NVARCHAR (1000) NULL,
    [IsMaster]    BIT             NULL,
    [MasterRef]   VARCHAR (1000)  NULL,
    [MasterData]  NVARCHAR (MAX)  NULL,
    [IsActive]    BIT             NULL,
    [CreatedBy]   INT             NULL,
    [CreatedOn]   DATETIME        DEFAULT (getdate()) NULL,
    [UpdatedBy]   INT             NULL,
    [UpdatedOn]   DATETIME        NULL,
    [ColumnRef]   VARCHAR (1000)  NULL,
    [DBKey]       VARCHAR (50)    NULL,
    [FieldOrder]  INT             NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

