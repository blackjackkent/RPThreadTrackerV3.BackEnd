USE [RPThreadTracker]
GO

/****** Object:  Table [dbo].[UserSettings]    Script Date: 6/19/2017 7:24:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProfileSettings](
	[SettingsId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ShowDashboardThreadDistribution] [bit] NOT NULL,
	[UseInvertedTheme] [bit] NOT NULL,
	[AllowMarkQueued] [bit] NOT NULL,
	[LastNewsReadDate] [datetime] NULL

 CONSTRAINT [PK_ProfileSettings] PRIMARY KEY CLUSTERED 
(
	[SettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ProfileSettings]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_ProfileSettings] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProfileSettings] CHECK CONSTRAINT [FK_AspNetUsers_ProfileSettings]
GO


