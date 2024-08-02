CREATE TABLE [dbo].[Insurance_City] (
    [CityId]    VARCHAR (50)  NULL,
    [CityName]  VARCHAR (100) NULL,
    [StateId]   VARCHAR (50)  NULL,
    [CreatedBy] VARCHAR (50)  NULL,
    [CreatedOn] DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)  NULL,
    [UpdatedOn] DATETIME      NULL,
    [IsPopular] BIT           NULL
);

