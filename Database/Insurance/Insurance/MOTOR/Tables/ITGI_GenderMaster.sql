CREATE TABLE [MOTOR].[ITGI_GenderMaster] (
    [Description] NVARCHAR (255) NULL,
    [Mode]        NVARCHAR (255) NULL,
    [CreatedBy]   VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]   DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)   NULL,
    [UpdatedOn]   DATETIME       NULL
);



