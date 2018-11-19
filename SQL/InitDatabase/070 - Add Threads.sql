USE [RPThreadTracker]
GO

/****** Object:  Table [dbo].[UserThread]    Script Date: 9/9/2017 4:12:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Threads](
	[ThreadId] [int] IDENTITY(1,1) NOT NULL,
	[CharacterId] [int] NOT NULL,
	[PostId] [nvarchar](max) NULL,
	[UserTitle] [nvarchar](max) NULL,
	[PartnerUrlIdentifier] [varchar](50) NULL,
	[IsArchived] [bit] NOT NULL,
	[DateMarkedQueued] [datetime] NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_dbo.Threads] PRIMARY KEY CLUSTERED 
(
	[ThreadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Threads] ADD  DEFAULT ((0)) FOR [IsArchived]
GO

ALTER TABLE [dbo].[Threads]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Threads_dbo.Characters_CharacterId] FOREIGN KEY([CharacterId])
REFERENCES [dbo].[Characters] ([CharacterId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Threads] CHECK CONSTRAINT [FK_dbo.Threads_dbo.Characters_CharacterId]
GO


