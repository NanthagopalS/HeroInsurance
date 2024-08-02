CREATE TABLE [dbo].[Insurance_ApplicationConfig] (
    [ConfigID]    UNIQUEIDENTIFIER NULL,
    [ConfigName]  VARCHAR (50)     NULL,
    [ConfigValue] VARCHAR (MAX)    NULL,
    [CreatedBy]   VARCHAR (50)     NULL,
    [CreatedOn]   DATETIME         DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)     NULL,
    [UpdatedOn]   DATETIME         NULL
);

