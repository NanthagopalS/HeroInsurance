CREATE TABLE [dbo].[Insurance_Model] (
    [ModelId]        VARCHAR (50)  NULL,
    [ModelName]      VARCHAR (100) NULL,
    [MakeId]         VARCHAR (50)  NULL,
    [CreatedBy]      INT           NULL,
    [CreatedOn]      DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]      INT           NULL,
    [UpdatedOn]      DATETIME      NULL,
    [IsPopularModel] BIT           NULL,
    [IsCommercial]   BIT           DEFAULT ((0)) NULL
);



