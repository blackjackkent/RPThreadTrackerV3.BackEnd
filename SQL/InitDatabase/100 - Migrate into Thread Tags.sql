IF (EXISTS (SELECT * 
	FROM INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_SCHEMA = 'dbo' 
	AND  TABLE_NAME = 'userblog'))
BEGIN
	INSERT INTO dbo.ThreadTags (
	TagId
	, TagText
	, ThreadId)
	SELECT
		CAST(tt.TagID as varchar),
		tt.TagText,
		tt.UserThreadID as ThreadId
		FROM dbo.userthreadTag tt left join userthread t on tt.userthreadid = t.UserThreadId where tt.userthreadid is not null 
		and EXISTS (SELECT
			1
		FROM dbo.threads
		WHERE threadid = tt.UserThreadId)
END
