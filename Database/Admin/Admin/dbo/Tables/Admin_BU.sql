CREATE TABLE [dbo].[Admin_BU] (
    [BUId]            VARCHAR (100) CONSTRAINT [DF_Admin_BU_BUId] DEFAULT (newid()) NOT NULL,
    [BUName]          VARCHAR (100) NULL,
    [BUHeadId]        VARCHAR (100) NULL,
    [BULevelId]       VARCHAR (100) NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Admin_BU_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Admin_BU_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    [SalesandSupport] BIT           CONSTRAINT [DF_Admin_BU_SalesandSupport] DEFAULT ((0)) NULL,
    [IsSales]         BIT           DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Admin_BU_BUID] PRIMARY KEY CLUSTERED ([BUId] ASC)
);

