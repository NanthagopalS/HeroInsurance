CREATE TABLE [dbo].[POSP_TestimonialsDetails] (
    [Id]            VARCHAR (50)  CONSTRAINT [DF_POSP_FeedbackDetailsmaster_Id] DEFAULT (newid()) NOT NULL,
    [POSP_Id]       VARCHAR (15)  NOT NULL,
    [Name]          VARCHAR (50)  NULL,
    [image]         VARCHAR (50)  NULL,
    [feedback]      VARCHAR (250) NULL,
    [starCount]     FLOAT (53)    CONSTRAINT [DF_POSP_FeedbackDetailsmaster_starCount] DEFAULT ((0)) NULL,
    [PriorityIndex] BIT           DEFAULT ((0)) NULL
);

