CREATE TABLE [MOTOR].[ITGI_SalutationMaster] (
    [TXT_Salutation] NVARCHAR (255) NULL,
    [CreatedBy]      VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]      DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]      VARCHAR (50)   NULL,
    [UpdatedOn]      DATETIME       NULL,
    [Mode]           VARCHAR (50)   NULL
);



