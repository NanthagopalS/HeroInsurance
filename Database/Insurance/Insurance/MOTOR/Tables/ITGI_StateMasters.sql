CREATE TABLE [MOTOR].[ITGI_StateMasters] (
    [STATE_CODE] NVARCHAR (255) NULL,
    [STATE_NAME] NVARCHAR (255) NULL,
    [CreatedBy]  VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]  DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]  VARCHAR (50)   NULL,
    [UpdatedOn]  DATETIME       NULL
);



