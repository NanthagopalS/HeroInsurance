CREATE TABLE [MOTOR].[ITGI_MaritalStatusMaster] (
    [Marital_Status]    NVARCHAR (255) NULL,
    [CreatedBy]         VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]         DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (50)   NULL,
    [UpdatedOn]         DATETIME       NULL,
    [MaritalStatusCode] VARCHAR (10)   NULL
);



