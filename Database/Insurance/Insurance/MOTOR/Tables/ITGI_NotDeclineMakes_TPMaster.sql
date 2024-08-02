CREATE TABLE [MOTOR].[ITGI_NotDeclineMakes_TPMaster] (
    [MAKE_CODE]        NVARCHAR (255) NULL,
    [MANUFACTURE]      NVARCHAR (255) NULL,
    [MODEL]            NVARCHAR (255) NULL,
    [VARIANT]          NVARCHAR (255) NULL,
    [CC]               FLOAT (53)     NULL,
    [SEATING_CAPACITY] FLOAT (53)     NULL,
    [CONTRACT_TYPE]    NVARCHAR (255) NULL,
    [FUEL_TYPE]        NVARCHAR (255) NULL,
    [ACTIVE]           NVARCHAR (255) NULL,
    [CreatedBy]        VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]        DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (50)   NULL,
    [UpdatedOn]        DATETIME       NULL
);



