IF (EXISTS (SELECT * 
	FROM INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_SCHEMA = 'dbo' 
	AND  TABLE_NAME = 'userblog'))
BEGIN
	SET IDENTITY_INSERT dbo.Threads ON;

	INSERT INTO dbo.Threads (
	ThreadId
	, CharacterId
	, PostId
	, UserTitle
	, PartnerUrlIdentifier
	, IsArchived
	, DateMarkedQueued)
	SELECT
		t.Userthreadid as ThreadId,
		t.UserBlogId as CharacterId,
		t.PostId,
		t.UserTitle,
		t.WatchedShortname as PartnerUrlIdentifier,
		t.IsArchived,
		t.MarkedQueued as DateMarkedQueued
		FROM dbo.userthread t
		WHERE EXISTS (SELECT
			1
		FROM dbo.Characters
		WHERE CharacterId = t.UserBlogId)

	SET IDENTITY_INSERT dbo.Threads OFF;
END
