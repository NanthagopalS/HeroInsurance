CREATE TABLE [dbo].[Admin_StampData] (
    [Id]          VARCHAR (100) CONSTRAINT [DF_Admin_StampData_Id] DEFAULT (newid()) NOT NULL,
    [SrNo]        VARCHAR (100) NULL,
    [StampData]   VARCHAR (100) NULL,
    [IsActive]    BIT           CONSTRAINT [DF_Admin_StampData_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]   VARCHAR (50)  NULL,
    [CreatedOn]   DATETIME      CONSTRAINT [DF_Admin_StampData_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)  NULL,
    [UpdatedOn]   DATETIME      NULL,
    [StampStatus] VARCHAR (50)  CONSTRAINT [DF_Admin_StampData_StampStatus] DEFAULT ('Available') NULL,
    CONSTRAINT [PK_Admin_StampData_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

