CREATE TABLE [dbo].[POSP_Rating] (
    [Id]          VARCHAR (50)  CONSTRAINT [DF_POSP_Rating_Id] DEFAULT (newid()) NOT NULL,
    [UserId]      VARCHAR (50)  NOT NULL,
    [Rating]      INT           NOT NULL,
    [Description] VARCHAR (200) NOT NULL,
    [IsActive]    BIT           CONSTRAINT [DF_POSP_Rating_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]   VARCHAR (500) NULL,
    [CreatedOn]   DATETIME      CONSTRAINT [DF_POSP_Rating_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (500) NULL,
    [UpdatedOn]   DATETIME      NULL,
    CONSTRAINT [PK_POSP_Rating_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

