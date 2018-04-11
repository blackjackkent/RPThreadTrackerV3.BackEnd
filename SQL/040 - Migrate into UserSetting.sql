INSERT INTO dbo.ProfileSettings (
UserId
, ShowDashboardThreadDistribution
, UseInvertedTheme
, AllowMarkQueued)
    SELECT
        p.UserId ,
        p.ShowDashboardThreadDistribution,
        p.UseInvertedTheme,
        p.AllowMarkQueued
    FROM dbo.userprofile p
	WHERE EXISTS (SELECT
        1
    FROM dbo.aspnetusers
    WHERE id = p.UserId)