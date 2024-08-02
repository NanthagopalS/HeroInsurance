CREATE TABLE [dbo].[Insurance_PACoverExtension] (
    [PACoverExtensionId] VARCHAR (50)  CONSTRAINT [DF__Insurance__PACov__4A4E069C] DEFAULT (newid()) NOT NULL,
    [PACoverExtension]   VARCHAR (100) NULL,
    [PACoverId]          VARCHAR (50)  NULL,
    [IsActive]           BIT           CONSTRAINT [DF__Insurance__IsAct__4B422AD5] DEFAULT ((1)) NULL,
    [CreatedBy]          VARCHAR (100) NULL,
    [CreatedOn]          DATETIME      CONSTRAINT [DF_Insurance_PACoverExtension_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]          VARCHAR (100) NULL,
    [UpdatedOn]          DATETIME      NULL,
    CONSTRAINT [PK_Insurance_PACoverExtension] PRIMARY KEY CLUSTERED ([PACoverExtensionId] ASC),
    FOREIGN KEY ([PACoverId]) REFERENCES [dbo].[Insurance_PACover] ([PACoverId])
);

