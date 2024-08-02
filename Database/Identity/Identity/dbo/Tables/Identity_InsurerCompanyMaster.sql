CREATE TABLE [dbo].[Identity_InsurerCompanyMaster] (
    [Id]                 VARCHAR (100) CONSTRAINT [DF_Identity_InsurerCompanyMaster_Id] DEFAULT (newid()) NOT NULL,
    [InsurerCompanyName] VARCHAR (100) NOT NULL,
    [IsActive]           BIT           CONSTRAINT [DF_Identity_InsurerCompanyMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]          VARCHAR (50)  NULL,
    [CreatedOn]          DATETIME      CONSTRAINT [DF_Identity_InsurerCompanyMaster_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]          VARCHAR (50)  NULL,
    [UpdatedOn]          DATETIME      NULL,
    CONSTRAINT [PK_InsurerCompanyMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

