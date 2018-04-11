INSERT INTO dbo.aspnetusers (
id
, AccessFailedCount
, ConcurrencyStamp
, Email
, EmailConfirmed
, LockoutEnabled
, LockoutEnd
, NormalizedEmail
, NormalizedUserName
, PasswordHash
, PhoneNumber
, PhoneNumberConfirmed
, SecurityStamp
, TwoFactorEnabled
, UserName)
SELECT
	u.UserId,
	0 AccessFailedCount,
	NULL ConcurrencyStamp,
	u.Email,
	0 EmailConfirmed,
	0 LockoutEnabled,
	NULL LockoutEnd,
	UPPER(u.Email) NormalizedEmail,
	UPPER(u.UserName) NormalizedUsername,
	m.Password PasswordHash,
	NULL PhoneNumber,
	0 PhoneNumberConfirmed,
	NULL SecurityStamp,
	0 TwoFactorEnabled,
	u.UserName
    FROM dbo.userprofile u
        INNER JOIN dbo.webpages_membership m
            ON m.userid = u.UserId
    WHERE 
	Email is NOT NULL
	AND
	NOT EXISTS (SELECT
        1
    FROM dbo.aspnetusers
    WHERE id = u.UserId)


  UPDATE AspNetUsers SET SecurityStamp = NewID() WHERE SecurityStamp is null