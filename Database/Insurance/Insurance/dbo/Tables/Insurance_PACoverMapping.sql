﻿CREATE TABLE [dbo].[Insurance_PACoverMapping] (
    [PACoverId] VARCHAR (50)  NULL,
    [InsurerId] VARCHAR (50)  NULL,
    [CreatedBy] VARCHAR (100) NULL,
    [CreatedOn] DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (100) NULL,
    [UpdatedOn] DATETIME      NULL
);
