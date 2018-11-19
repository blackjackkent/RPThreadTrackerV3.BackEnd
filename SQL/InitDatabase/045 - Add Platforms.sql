USE [RPThreadTracker]
GO

/****** Object:  Table [dbo].[Platforms]    Script Date: 2/5/2018 7:38:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Platforms](
	[PlatformId] [int] IDENTITY(1,1) NOT NULL,
	[PlatformName] [nchar](255) NULL,
 CONSTRAINT [PK_Platforms] PRIMARY KEY CLUSTERED 
(
	[PlatformId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET IDENTITY_INSERT dbo.Platforms ON;

INSERT INTO dbo.Platforms (PlatformId, PlatformName) VALUES (1, 'Tumblr');
GO

SET IDENTITY_INSERT dbo.Platforms OFF;

