USE [RPThreadTracker]
GO

/****** Object:  Table [dbo].[ProfileSettings]    Script Date: 7/17/2018 9:47:57 PM ******/
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
	[LastNewsReadDate] [datetime] NULL,
	[ThreadTablePageSize] [int] NOT NULL,
 CONSTRAINT [PK_ProfileSettings] PRIMARY KEY CLUSTERED 
(
	[SettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [dbo].[ProfileSettings] ADD  CONSTRAINT [DF_ProfileSettings_ThreadTablePageSize]  DEFAULT ((10)) FOR [ThreadTablePageSize]
GO

ALTER TABLE [dbo].[ProfileSettings]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_ProfileSettings] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProfileSettings] CHECK CONSTRAINT [FK_AspNetUsers_ProfileSettings]
GO
