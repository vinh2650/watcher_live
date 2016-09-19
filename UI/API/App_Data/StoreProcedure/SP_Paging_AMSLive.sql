USE [amssystem_live]
GO

/****** Object:  StoredProcedure [dbo].[Alarm_GetAllAlarmsAndHistories]    Script Date: 8/22/2016 2:02:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Alarm_GetAllAlarmsAndHistories]
	@operatorId NVARCHAR(MAX),
	@skip INT = 0,
	@take INT = 10,
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
			WHERE a.OperatorId = @operatorId) a

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
ORDER BY CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
GO





--------------------------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Alarm_GetAllActiveAlarmRecords]
@operatorId NVARCHAR(MAX),
@skip INT = 0,
@take INT = 10,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
SELECT @totalRecords=COUNT(*) FROM AlarmRecords
		WHERE OperatorId= @operatorId
SELECT x.CellType, x.LocalCellId, x.CellName,x.BtsName ,x.Slogan, x.Attribute, x.Severity, x.MoClass, x.MoInstance, x.AdditionInfo, x.ENodeBId,
		x.SiteId
FROM AlarmRecords x
WHERE OperatorId= @operatorId
ORDER BY CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
GO


---------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Alarm_GetActiveAlarmRecordsByCellName]
	@operatorId NVARCHAR(MAX),
	@cellName VARCHAR(100),
	@skip INT = 0,
	@take INT = 10,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
SELECT @totalRecords=COUNT(*) FROM AlarmRecords h
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
ORDER BY CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
GO

--------------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Alarm_GetAlarmHistoriesByCellName]
	@operatorId NVARCHAR(MAX),
	@cellName VARCHAR(100),
	@skip INT,
	@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
SELECT @totalRecords=COUNT(*) FROM HistoryAlarmRecords h
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
ORDER BY CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
GO
----------------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROC [dbo].[Alarm_GetTopAlarmDataByCellName]
(
	@OperatorId NVARCHAR(MAX),
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
	WHERE a.CellName=@CellName AND a.OperatorId = @OperatorId
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
	WHERE a.CellName=@CellName AND a.OperatorId = @OperatorId
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
	WHERE a.CellName=@CellName AND a.OperatorId = @OperatorId
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
	WHERE a.CellName=@CellName AND a.OperatorId = @OperatorId
	) q
ORDER BY q.AlarmType, q.CreatedDateUtc DESC
GO


---------------------


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Alarm_GetAlarmDataByKeywords]
(
	@OperatorId NVARCHAR(MAX),
	@Keywords NVARCHAR(MAX),
	@skip INT = 0,
	@take INT = 10,
	@totalRecords BIGINT = 0 OUT
)
AS
SELECT @totalRecords=COUNT(*) FROM dbo.AlarmRecords a
	WHERE (LOWER(a.Slogan) LIKE LOWER(@Keywords) OR LOWER(a.CellName) LIKE LOWER(@Keywords)) AND a.OperatorId = @OperatorId

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
	WHERE (LOWER(a.Slogan) LIKE LOWER(@Keywords) OR LOWER(a.CellName) LIKE LOWER(@Keywords)) AND a.OperatorId = @OperatorId
	) q
ORDER BY q.CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
GO


-----------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Notification_GetNotification]
	@linkId NVARCHAR(MAX),
	@type INT,
	@onwerId NVARCHAR(MAX),
	@createdDateUtc DATETIME = NULL,
	@skip INT,
	@take INT,
	@operatorId NVARCHAR(MAX),
	@totalRecords BIGINT = 0 OUT
AS
BEGIN
	--validate
	IF @linkId IS NULL OR @type IS NULL OR @operatorId IS NULL
		RETURN;

	IF @skip IS NULL AND @take IS NULL AND @createdDateUtc IS NULL
		RETURN;			
	IF @createdDateUtc IS NOT NULL
	BEGIN
		SELECT @totalRecords=COUNT(*)	FROM [dbo].[Notification]
		WHERE [LinkId] = @linkId AND [Type] = @type
		AND ((@onwerId IS NOT NULL AND OwnerId = @onwerId) OR (@onwerId IS NULL))
		AND CreatedDateUtc > @createdDateUtc AND OperatorId = @operatorId

		
		SELECT x.[Id],x.[OwnerId],x.[Content],x.[CreatedDateUtc],x.[Subject],x.[AttachmentList], 
		(SELECT COUNT(*) FROM Notificationcomment p WHERE NotificationId = x.Id) AS Total, (u.FirstName + ' ' + u.LastName) AS Author
		FROM [dbo].[Notification] x JOIN [User] u ON u.Id = x.OwnerId
		WHERE  [LinkId] = @linkId AND [Type] = @type
		AND ((@onwerId IS NOT NULL AND OwnerId = @onwerId) OR (@onwerId IS NULL)) AND x.CreatedDateUtc > @createdDateUtc AND OperatorId = @operatorId
		ORDER BY x.CreatedDateUtc
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY
		RETURN
	END	
		--get by skip and take
	IF @skip IS NOT NULL AND @take IS NOT NULL
	BEGIN
		
		SELECT @totalRecords=COUNT(*) FROM [dbo].[Notification]
		WHERE [LinkId] = @linkId AND [Type] = @type
		AND ((@onwerId IS NOT NULL AND OwnerId = @onwerId) OR (@onwerId IS NULL)) AND OperatorId = @operatorId

		SELECT x.[Id],x.[OwnerId],x.[Content],x.[CreatedDateUtc],x.[Subject],x.[AttachmentList], 
		(SELECT COUNT(*) FROM Notificationcomment p WHERE NotificationId = x.Id) AS Total, (u.FirstName + ' ' + u.LastName) AS Author
		FROM [dbo].[Notification] x JOIN [User] u ON u.Id = x.OwnerId
		WHERE  [LinkId] = @linkId AND [Type] = @type
		AND ((@onwerId IS NOT NULL AND OwnerId = @onwerId) OR (@onwerId IS NULL)) AND OperatorId = @operatorId
		ORDER BY x.CreatedDateUtc
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY

		RETURN;
	END

	--get by created date
	
END
GO


--------------------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCellsByCellName]
@operatorId NVARCHAR(MAX),
@cellName VARCHAR(100),
@skip INT,
@take INT,
@totalRecords BIGINT = 0 OUT
AS
BEGIN

SELECT @totalRecords=COUNT(*) FROM CellDbms c
		WHERE c.CellName LIKE '%' + @cellName +'%'
		AND c.OperatorId =@operatorId

SELECT 	x.CellId,
			x.CreatedDateUtc,
			x.CellName,
			x.InterferenceDbm,
			x.RtwpDbm,
			x.CellType
FROM CellDbms x
		WHERE x.CellName LIKE '%' + @cellName +'%'
		AND x.OperatorId =@operatorId
ORDER BY CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
GO

-----------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Notification_GetMessageByCellNames]
	@cellNames VARCHAR(MAX),
	@type INT,
	@skip INT,
	@take INT,
	@totalRecords BIGINT = 0 OUT
AS
BEGIN
	SELECT @totalRecords=COUNT(*) FROM [dbo].[Notification] p
		WHERE ',' + @cellNames + ',' LIKE '%,' + [LinkId] + ',%' AND [Type] = @type

	SELECT x.[Id],x.[OwnerId],x.[Content],x.[Subject],x.[AttachmentList],x.[UpdatedDateUtc],x.[CreatedDateUtc]
	FROM [dbo].[Notification] x
	WHERE ',' + @cellNames + ',' LIKE '%,' + [LinkId] + ',%' AND [Type] = @type
	ORDER BY CreatedDateUtc DESC
	OFFSET @skip ROWS
	FETCH NEXT @take ROWS ONLY


	RETURN;
END
GO
------------



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Notification_GetNotificationComment]
	@notificationId NVARCHAR(MAX) = '',
	@createdDateUtc DATETIME,
	@skip INT = 0,
	@take INT = 10,
	@totalRecords BIGINT = 0 OUT
AS
BEGIN
	--validate
	IF @skip IS NULL AND @take IS NULL AND @createdDateUtc IS NULL
		RETURN;

	--get by created date
	IF @createdDateUtc IS NOT NULL
	BEGIN
		SELECT @totalRecords=COUNT(*) FROM [dbo].[NotificationComment]
		WHERE NotificationId = @notificationId AND CreatedDateUtc > @createdDateUtc


		SELECT [Id]
			  ,[NotificationId]
			  ,[OwnerId]
			  ,[Content]
			  ,[CreatedDateUtc]
			  ,[Subject]
			  ,[AttachmentList]
		FROM [dbo].[NotificationComment]
		WHERE NotificationId = @notificationId AND CreatedDateUtc > @createdDateUtc
		ORDER BY CreatedDateUtc DESC
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY
		RETURN;
	END	

	--get by skip and take
	IF @skip IS NOT NULL AND @take IS NOT NULL
	BEGIN
		SELECT @totalRecords=COUNT(*) FROM [dbo].[NotificationComment]
		  WHERE NotificationId = @notificationId

		SELECT [Id]
			  ,[NotificationId]
			  ,[OwnerId]
			  ,[Content]
			  ,[CreatedDateUtc]
			  ,[Subject]
			  ,[AttachmentList]
		FROM [dbo].[NotificationComment]
		WHERE NotificationId = @notificationId
		ORDER BY CreatedDateUtc DESC
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY
		RETURN;
	END

	
END
GO
-----------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Organization_GetAllOperatorsForInsightusAdmin]
@insightusAdminId NVARCHAR(128),
@skip INT = 0,
@take INT = 10,
@totalRecords BIGINT = 0 OUT
AS
BEGIN
IF @insightusAdminId IS NULL
			RETURN;
SELECT @totalRecords=COUNT(*) FROM Operators l JOIN [User] u ON u.Id = l.OwnerId
			WHERE OwnerId <> @insightusAdminId
SELECT l.[Id],l.[Name],l.[Band],l.[OwnerId],l.[OrganizationId],l.[CreatedDateUtc],
			u.[FirstName] AS [OwnerFirstName], u.[LastName] AS [OwnerLastName], u.[Email] AS [OwnerEmail]
FROM Operators l JOIN [User] u ON u.Id = l.OwnerId
WHERE OwnerId <> @insightusAdminId
ORDER BY CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
GO
