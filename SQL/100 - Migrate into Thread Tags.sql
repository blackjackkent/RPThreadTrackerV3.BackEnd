INSERT INTO dbo.ThreadTags (
TagId
, TagText
, ThreadId)
SELECT
	tt.TagID,
	tt.TagText,
	tt.UserThreadID as ThreadId
    FROM dbo.userthreadTag tt left join userthread t on tt.userthreadid = t.UserThreadId where tt.userthreadid is not null