USE [amssystemondemand]
GO

/****** Object:  StoredProcedure [dbo].[Alarm_GetAlarmDataByKeywords_V3]    Script Date: 8/18/2016 9:53:33 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[Alarm_GetAlarmDataByKeywords_V3]
(
	@OperatorId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX),
	@Keywords NVARCHAR(MAX),
	@skip INT,
	@take INT,
	@totalRecords BIGINT = 0 OUT
)
AS
SELECT @totalRecords=COUNT(*) FROM dbo.AlarmRecords a
	WHERE (LOWER(a.Slogan) LIKE LOWER(@Keywords) OR LOWER(a.CellName) LIKE LOWER(@Keywords)) AND a.OperatorId = @OperatorId AND a.JobId = @jobId

SELECT q.*
FROM 
	(
	SELECT	a.CreatedDateUtc, 
			a.Severity,
			a.Slogan,
			a.Attribute,
			a.MoInstance,
			a.AdditionInfo,
			a.CellType,
			a.LocalCellId,
			a.CellName,
			a.MoClass,
			a.BtsName,
			a.SiteId
	FROM dbo.AlarmRecords a
	WHERE (LOWER(a.Slogan) LIKE LOWER(@Keywords) OR LOWER(a.CellName) LIKE LOWER(@Keywords)) AND a.OperatorId = @OperatorId AND a.JobId = @jobId
	) q
ORDER BY q.CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY



GO


