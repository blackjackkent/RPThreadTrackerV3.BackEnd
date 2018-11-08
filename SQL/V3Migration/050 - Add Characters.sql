USE [RPThreadTracker]
GO

/****** Object:  Table [dbo].[UserBlog]    Script Date: 9/9/2017 3:50:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Characters](
	[CharacterId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[UrlIdentifier] [nvarchar](max) NULL,
	[CharacterName] [nvarchar](max) NULL,
	[IsOnHiatus] [bit] NOT NULL,
	[PlatformId] [int] NOT NULL
 CONSTRAINT [PK_dbo.Character] PRIMARY KEY CLUSTERED 
(
	[CharacterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Characters] ADD  DEFAULT ((0)) FOR [IsOnHiatus]
GO

ALTER TABLE [dbo].[Characters]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Characters] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Characters]  WITH CHECK ADD  CONSTRAINT [FK_Platforms_Characters] FOREIGN KEY([PlatformId])
REFERENCES [dbo].[Platforms] ([PlatformId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Characters] CHECK CONSTRAINT [FK_AspNetUsers_Characters]
GO

ALTER TABLE [dbo].[Characters] CHECK CONSTRAINT [FK_Platforms_Characters]
GO

