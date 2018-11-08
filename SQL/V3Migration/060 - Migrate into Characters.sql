IF (EXISTS (SELECT * 
	FROM INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_SCHEMA = 'dbo' 
	AND  TABLE_NAME = 'userblog'))
BEGIN
	SET IDENTITY_INSERT dbo.Characters ON;

	INSERT INTO dbo.characters (
	CharacterId
	, UserId
	, UrlIdentifier
	, IsOnHiatus
	, PlatformId)
	SELECT
		b.UserBlogId as CharacterId,
		b.UserId,
		b.BlogShortname as UrlIdentifier,
		b.OnHiatus as IsOnHiatus,
		1 as PlatformId
		FROM dbo.userblog b
		WHERE EXISTS (SELECT
			1
		FROM dbo.aspnetusers
		WHERE id = b.UserId)

	SET IDENTITY_INSERT dbo.Characters OFF;
END
