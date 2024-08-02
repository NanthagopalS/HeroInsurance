CREATE TABLE [dbo].[Identity_PremiumRangeTypeMaster] (
    [Id]               VARCHAR (100)  CONSTRAINT [DF_Identity_PremiumRangeTypeMaster_Id] DEFAULT (newid()) NOT NULL,
    [PremiumRangeType] NVARCHAR (100) NULL,
    [IsActive]         BIT            CONSTRAINT [DF_Identity_PremiumRangeTypeMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (50)   NULL,
    [CreatedOn]        DATETIME       CONSTRAINT [DF_Identity_PremiumRangeTypeMaster_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]        VARCHAR (50)   NULL,
    [UpdatedOn]        DATETIME       NULL,
    [OrderBy]          VARCHAR (255)  NULL,
    CONSTRAINT [PK_PremiumRange_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);





