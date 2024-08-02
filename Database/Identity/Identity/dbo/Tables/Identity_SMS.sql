CREATE TABLE [dbo].[Identity_SMS] (
    [SMSId]           UNIQUEIDENTIFIER CONSTRAINT [DF__Identity___SMSId__2C3393D0] DEFAULT (newid()) NOT NULL,
    [SMSText]         VARCHAR (80)     NULL,
    [SMSOTP]          VARCHAR (4)      NULL,
    [SMSSendDateTime] DATETIME         NULL,
    [UserId]          VARCHAR (50)     NULL,
    [SMSReferenceId]  VARCHAR (20)     NULL,
    [CreatedBy]       VARCHAR (50)     NULL,
    [CreatedOn]       DATETIME         CONSTRAINT [DF__Identity___Creat__2D27B809] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)     NULL,
    [UpdatedOn]       DATETIME         NULL,
    [StageId]         VARCHAR (20)     NULL,
    CONSTRAINT [PK__Identity__244A5CDB253F072A] PRIMARY KEY CLUSTERED ([SMSId] ASC)
);

