SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[Alarm_GetTopAlarmDataByCellName_V3]
(
	@OperatorId NVARCHAR(MAX),
	@JobId NVARCHAR(MAX),
	@CellName NVARCHAR(1000),
	@MaxRow INT,
	@totalRecords BIGINT = 0 OUT
)
AS
SELECT @totalRecords=COUNT(*) FROM 
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
			a.BtsName,
			a.MoClass,
			--a.ENodeBId,
			a.SiteId,
			1 AS [AlarmType],
			NULL AS [CeaseDateTimeUtc]
	FROM dbo.AlarmRecords a
	WHERE a.CellName=@CellName AND a.OperatorId = @OperatorId AND a.JobId = @JobId
	UNION
	SELECT	a.CreatedDateUtc, 
			a.Severity,
			a.Slogan,
			a.Attribute,
			a.MoInstance,
			a.AdditionInfo,
			a.CellType,
			a.LocalCellId,
			a.CellName,
			a.BtsName,
			a.MoClass,
			--a.ENodeBId,
			a.SiteId,
			2 AS [AlarmType],
			a.CeaseDateTimeUtc
	FROM dbo.HistoryAlarmRecords a
	WHERE a.CellName=@CellName AND a.OperatorId = @OperatorId AND a.JobId = @JobId
	) q

SELECT TOP(@MaxRow) q.*
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
			a.BtsName,
			a.MoClass,
			--a.ENodeBId,
			a.SiteId,
			1 AS [AlarmType],
			NULL AS [CeaseDateTimeUtc]
	FROM dbo.AlarmRecords a
	WHERE a.CellName=@CellName AND a.OperatorId = @OperatorId AND a.JobId = @JobId
	UNION
	SELECT	a.CreatedDateUtc, 
			a.Severity,
			a.Slogan,
			a.Attribute,
			a.MoInstance,
			a.AdditionInfo,
			a.CellType,
			a.LocalCellId,
			a.CellName,
			a.BtsName,
			a.MoClass,
			--a.ENodeBId,
			a.SiteId,
			2 AS [AlarmType],
			a.CeaseDateTimeUtc
	FROM dbo.HistoryAlarmRecords a
	WHERE a.CellName=@CellName AND a.OperatorId = @OperatorId AND a.JobId = @JobId
	) q
ORDER BY q.AlarmType, q.CreatedDateUtc DESC


GO


