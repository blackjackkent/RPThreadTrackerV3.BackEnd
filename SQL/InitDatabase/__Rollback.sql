USE RPThreadTracker;
GO

IF OBJECT_ID('dbo.Logging', 'U') IS NOT NULL 
  DROP TABLE dbo.Logging; 
IF OBJECT_ID('dbo.ThreadTags', 'U') IS NOT NULL 
  DROP TABLE dbo.ThreadTags;
IF OBJECT_ID('dbo.Threads', 'U') IS NOT NULL 
  DROP TABLE dbo.Threads;
IF OBJECT_ID('dbo.Characters', 'U') IS NOT NULL 
  DROP TABLE dbo.Characters;
IF OBJECT_ID('dbo.Platforms', 'U') IS NOT NULL 
  DROP TABLE dbo.Platforms;
IF OBJECT_ID('dbo.ProfileSettings', 'U') IS NOT NULL 
  DROP TABLE dbo.ProfileSettings; 
IF OBJECT_ID('dbo.RefreshTokens', 'U') IS NOT NULL 
  DROP TABLE dbo.RefreshTokens; 
IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL 
  DROP TABLE dbo.AspNetUserRoles; 
IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL 
  DROP TABLE dbo.AspNetRoles; 
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL 
  DROP TABLE dbo.AspNetUserClaims; 
IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL 
  DROP TABLE dbo.AspNetUsers; 
GO