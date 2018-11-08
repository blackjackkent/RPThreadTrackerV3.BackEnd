USE [RPThreadTracker]
GO

/****** Object:  Table [dbo].[UserThreadTag]    Script Date: 9/9/2017 4:39:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ThreadTags](
	[TagID] [varchar](128) NOT NULL,
	[TagText] [varchar](140) NOT NULL,
	[ThreadID] [int] NULL
);
GO
ALTER TABLE [dbo].[ThreadTags]
    ADD CONSTRAINT [PK_dbo.ThreadTags] PRIMARY KEY CLUSTERED ([TagID] ASC);
GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ThreadTags]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ThreadTags_dbo.Threads_ThreadId] FOREIGN KEY([ThreadID])
REFERENCES [dbo].[Threads] ([ThreadId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ThreadTags] CHECK CONSTRAINT [FK_dbo.ThreadTags_dbo.Threads_ThreadId]
GO


