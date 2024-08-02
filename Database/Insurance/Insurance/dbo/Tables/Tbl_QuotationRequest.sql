CREATE TABLE [dbo].[Tbl_QuotationRequest] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [LeadId]      VARCHAR (50)   NULL,
    [RequestBody] NVARCHAR (MAX) NULL,
    [StageID]     VARCHAR (50)   NULL,
    [CreatedBy]   VARCHAR (100)  NULL,
    [CreatedOn]   DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (100)  NULL,
    [UpdatedOn]   DATETIME       NULL
);

