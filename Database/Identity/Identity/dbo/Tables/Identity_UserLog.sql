CREATE TABLE [dbo].[Identity_UserLog] (
    [LogId]          VARCHAR (100) CONSTRAINT [DF_Identity_User_LogId] DEFAULT (newid()) NOT NULL,
    [UserId]         VARCHAR (100) NOT NULL,
    [LogInDateTime]  DATETIME      CONSTRAINT [DF_Identity_User_LogInDateTime] DEFAULT (getdate()) NULL,
    [LogOutDateTime] DATETIME      NULL,
    [IsActive]       BIT           CONSTRAINT [DF_Identity_UserLog_IsActive] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_Identity_User_LogId] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

