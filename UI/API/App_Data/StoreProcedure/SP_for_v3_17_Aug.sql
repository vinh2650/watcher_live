USE [amssystemondemand]
GO
/****** Object:  StoredProcedure [dbo].[Alarm_GetActiveAlarmRecordsByCellNameTest]    Script Date: 8/16/2016 5:42:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Alarm_GetActiveAlarmRecordsByCellName_V3]
	@operatorId NVARCHAR(MAX),
	@cellName VARCHAR(100),
	@skip INT,
	@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
Select @totalRecords=COUNT(*) FROM AlarmRecords h
		WHERE h.OperatorId = @operatorId AND  h.CellName LIKE '%' + @cellName +'%'
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
WHERE x.OperatorId = @operatorId AND  x.CellName LIKE '%' + @cellName +'%'
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
	@cellName VARCHAR(100),
	@skip INT,
	@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
Select @totalRecords=COUNT(*) FROM HistoryAlarmRecords h
		WHERE h.OperatorId = @operatorId AND  h.CellName LIKE '%' + @cellName +'%'
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
WHERE x.OperatorId = @operatorId AND  x.CellName LIKE '%' + @cellName +'%'
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
@skip INT,
@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
Select @totalRecords=COUNT(*) FROM AlarmRecords
		WHERE OperatorId= @operatorId
SELECT x.CellType, x.LocalCellId, x.CellName,x.BtsName ,x.Slogan, x.Attribute, x.Severity, x.MoClass, x.MoInstance, x.AdditionInfo, x.ENodeBId,
		x.SiteId
FROM AlarmRecords x
WHERE OperatorId= @operatorId
Order By CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
-----


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[Alarm_GetAllAlarmsAndHistories_V3]
(
	@operatorId NVARCHAR(MAX),
	@skip INT,
	@take INT,
	@totalRecords INT = 0 OUT
)
AS
BEGIN
	if @skip is null and @take is null
		return;
	;WITH paging AS(
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
				--a.ENodeBId,
				a.SiteId,
				a.[AlarmType],
				ROW_NUMBER() OVER (ORDER BY a.CreatedDateUtc DESC) AS RowCounts
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
			WHERE a.OperatorId = @operatorId
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
			WHERE a.OperatorId = @operatorId
		) a
	)


	SELECT	
			CreatedDateUtc, 
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
			[AlarmType]
	FROM paging x
	WHERE x.RowCounts > @skip AND x.RowCounts <= (@skip + @take)
	set @totalRecords = @@rowcount
END

----

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetCellsByCellName_V3]
(
	@operatorId NVARCHAR(MAX),
	@cellName VARCHAR(100),
	@skip INT,
	@take INT,
	@totalRecords INT = 0 OUT
)
AS
BEGIN
	--validate
	IF @cellName IS NULL OR @skip IS NULL OR @take IS NULL
		RETURN;

	BEGIN
		;WITH paging AS (
		SELECT c.*, ROW_NUMBER() OVER (ORDER BY c.[CreatedDateUtc] Desc) as RowCounts
		FROM CellDbms c
		WHERE c.CellName LIKE '%' + @cellName +'%'
		AND c.OperatorId =@operatorId			
		)
		
		SELECT
			x.CellId,
			x.CreatedDateUtc,
			x.CellName,
			x.InterferenceDbm,
			x.RtwpDbm,
			x.CellType
		FROM paging x
		WHERE  x.RowCounts > @skip AND x.RowCounts <= (@skip + @take)
		SET @totalRecords = @@ROWCOUNT			
END
END