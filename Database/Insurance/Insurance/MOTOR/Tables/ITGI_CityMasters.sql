CREATE TABLE [MOTOR].[ITGI_CityMasters] (
    [CITY_CODE]  NVARCHAR (255) NULL,
    [CITY_DESC]  NVARCHAR (255) NULL,
    [STATE_CODE] NVARCHAR (255) NULL,
    [IRDA_ZONE]  NVARCHAR (255) NULL,
    [CreatedBy]  VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]  DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]  VARCHAR (50)   NULL,
    [UpdatedOn]  DATETIME       NULL
);

