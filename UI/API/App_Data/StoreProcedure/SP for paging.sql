USE [amssystemondemand]
GO
/****** Object:  StoredProcedure [dbo].[Alarm_GetActiveAlarmRecordsByCellNameTest]    Script Date: 8/16/2016 5:42:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Alarm_GetActiveAlarmRecordsByCellName_V3]
	@operatorId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX),
	@cellName VARCHAR(100),
	@skip INT,
	@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
Select @totalRecords=COUNT(*) FROM AlarmRecords h
		WHERE h.OperatorId = @operatorId AND h.JobId = @jobId AND  h.CellName LIKE '%' + @cellName +'%'
SELECT x.Slogan,
			x.Attribute,
			x.Severity,
			x.MoInstance,
			x.AdditionInfo,
			NULL AS CeaseDateTimeUtc,
			x.CreatedDateUtc,
			x.CellType,
			x.LocalCellId,
			x.CellName,
			x.MoClass,
			x.OperatorId,
			x.SiteId
FROM AlarmRecords x
WHERE x.OperatorId = @operatorId AND x.JobId = @jobId AND  x.CellName LIKE '%' + @cellName +'%'
Order By CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END

-------------
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Alarm_GetAlarmHistoriesByCellName_V3]
	@operatorId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX),
	@cellName VARCHAR(100),
	@skip INT,
	@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
Select @totalRecords=COUNT(*) FROM HistoryAlarmRecords h
		WHERE h.OperatorId = @operatorId AND h.JobId = @jobId AND  h.CellName LIKE '%' + @cellName +'%'
SELECT x.Slogan,
			x.Attribute,
			x.Severity,
			x.MoInstance,
			x.AdditionInfo,
			x.CeaseDateTimeUtc,
			x.CreatedDateUtc,
			x.CellType,
			x.LocalCellId,
			x.CellName,
			x.MoClass,
			x.OperatorId,
			x.SiteId
FROM HistoryAlarmRecords x
WHERE x.OperatorId = @operatorId AND x.JobId = @jobId AND  x.CellName LIKE '%' + @cellName +'%'
Order By CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END

-----

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Alarm_GetAllActiveAlarmRecords_V3]
@operatorId NVARCHAR(MAX),
@jobId NVARCHAR(MAX),
@skip INT,
@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
Select @totalRecords=COUNT(*) FROM AlarmRecords
		WHERE OperatorId= @operatorId AND JobId = @jobId
SELECT x.CellType, x.LocalCellId, x.CellName,x.BtsName ,x.Slogan, x.Attribute, x.Severity, x.MoClass, x.MoInstance, x.AdditionInfo, x.ENodeBId,
		x.SiteId
FROM AlarmRecords x
WHERE OperatorId= @operatorId AND JobId = @jobId
Order By CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
-----


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[Alarm_GetAllAlarmsAndHistories_V3]
	@operatorId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX),
	@skip INT,
	@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN

Select @totalRecords=COUNT(*) FROM (SELECT	
					a.CreatedDateUtc, 
					a.Severity,
					a.Slogan,
					a.Attribute,
					a.MoInstance,
					a.AdditionInfo,
					a.CellType,
					a.LocalCellId,
					a.CellName,
					a.MoClass,
					--a.ENodeBId,
					a.SiteId,
					1 AS [AlarmType]
			FROM dbo.AlarmRecords a
			WHERE a.OperatorId = @operatorId AND a.JobId = @jobId
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
					a.MoClass,
					a.SiteId,
					--a.ENodeBId,
					2 AS [AlarmType]
			FROM dbo.HistoryAlarmRecords a
			WHERE a.OperatorId = @operatorId AND a.JobId = @jobId) a

SELECT 	CreatedDateUtc, 
			Severity,
			Slogan,
			Attribute,
			MoInstance,
			AdditionInfo,
			CellType,
			LocalCellId,
			CellName,
			MoClass,
			--ENodeBId,
			SiteId,
			a.[AlarmType]
FROM (SELECT	
					a.CreatedDateUtc, 
					a.Severity,
					a.Slogan,
					a.Attribute,
					a.MoInstance,
					a.AdditionInfo,
					a.CellType,
					a.LocalCellId,
					a.CellName,
					a.MoClass,
					--a.ENodeBId,
					a.SiteId,
					1 AS [AlarmType]
			FROM dbo.AlarmRecords a
			WHERE a.OperatorId = @operatorId AND a.JobId = @jobId
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
					a.MoClass,
					a.SiteId,
					--a.ENodeBId,
					2 AS [AlarmType]
			FROM dbo.HistoryAlarmRecords a
			WHERE a.OperatorId = @operatorId AND a.JobId = @jobId
		) a
Order By CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END




----

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[GetCellsByCellName_V3]
@operatorId NVARCHAR(MAX),
@jobId NVARCHAR(MAX),
@cellName VARCHAR(100),
@skip INT,
@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN

Select @totalRecords=COUNT(*) FROM CellDbms c
		WHERE c.CellName LIKE '%' + @cellName +'%'
		AND c.OperatorId =@operatorId AND c.JobId = @jobId

SELECT 	x.CellId,
			x.CreatedDateUtc,
			x.CellName,
			x.InterferenceDbm,
			x.RtwpDbm,
			x.CellType
FROM CellDbms x
		WHERE x.CellName LIKE '%' + @cellName +'%'
		AND x.OperatorId =@operatorId AND x.JobId = @jobId
Order By CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END


-----

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Alarm_CountActiveAlarmTillNow_V3]
	@operatorId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX)
AS
BEGIN
	SELECT a.Slogan, SUM(1) AS [Count] FROM AlarmRecords a	
	WHERE a.OperatorId = @operatorId AND a.JobId = @jobId
	GROUP BY a.Slogan
END

GO

-----


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[Alarm_GetAlarmInTimeRange_V3]
	@operatorId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX),
	@listCellName NVARCHAR(MAX),
	@startDate DATETIME,
	@endDate DATETIME
AS
BEGIN
	IF @startDate IS NULL OR @endDate IS NULL
		RETURN;

	SELECT CreatedDateUtc, COUNT(CreatedDateUtc) AS [Count] 
	FROM AlarmRecords 
	WHERE OperatorId = @operatorId AND JobId = @jobId 
	AND CellName IN (SELECT CAST(data AS NVARCHAR(MAX)) FROM [nois_splitstring_to_table](@listCellName, ','))
	AND CreatedDateUtc >= @startDate
	AND CreatedDateUtc <= @endDate
	GROUP BY CreatedDateUtc
	ORDER BY CreatedDateUtc
	RETURN;
END

GO

----

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
	@take INT
)
AS
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
			a.SiteId,
			ROW_NUMBER() OVER (ORDER BY a.[CreatedDateUtc] DESC) AS RowCounts
	FROM dbo.AlarmRecords a
	WHERE (LOWER(a.Slogan) LIKE @Keywords OR LOWER(a.CellName) LIKE @Keywords) AND a.OperatorId = @OperatorId AND a.JobId = @jobId
	) q
WHERE q.RowCounts > @skip AND q.RowCounts <= (@skip + @take)
ORDER BY q.CreatedDateUtc DESC


GO

----

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[Alarm_GetTopAlarmDataByCellName_V3]
(
	@OperatorId NVARCHAR(MAX),
	@JobId NVARCHAR(MAX),
	@CellName NVARCHAR(1000),
	@MaxRow INT
)
AS
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

------
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[GetCellDbmsByPeriod_V3]
(
	@operatorId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX),
	@listCellName NVARCHAR(MAX),
	@period INT,
	@date DATETIME
)
AS
BEGIN
	--validate
	IF @operatorId IS NULL OR @jobId IS NULL OR @period IS NULL OR @date IS NULL
		RETURN;

	BEGIN
				
		SELECT
			*
		FROM CellDbms x
		WHERE x.OperatorId =@operatorId AND x.JobId =@jobId
		AND x.CellName IN (SELECT CAST(data AS NVARCHAR(MAX)) FROM [nois_splitstring_to_table](@listCellName, ','))
		 AND x.CreatedDateUtc > DATEADD(DAY,-@period,@date) 
	END
END


GO