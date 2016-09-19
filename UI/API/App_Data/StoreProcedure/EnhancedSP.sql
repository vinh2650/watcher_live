USE [amssystemondemand]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Organization_GetAllOperatorsForInsightusAdmin]
	@insightusAdminId varchar(128),
	@skip int = 0,
	@take int = 10
as
BEGIN 
		if @insightusAdminId is null
			return;

		;with paging as (
			SELECT l.[Id],l.[Name],l.[Band],l.[OwnerId],l.[OrganizationId],l.[CreatedDateUtc],
			u.[FirstName] as [OwnerFirstName], u.[LastName] as [OwnerLastName], u.[Email] as [OwnerEmail]
			FROM Operators l join [User] u on u.Id = l.OwnerId
			Where OwnerId <> @insightusAdminId
		)

		SELECT 	*
		FROM paging CROSS JOIN (SELECT COUNT_BIG(*) AS RowCounts FROM paging) AS tCountRecords
		ORDER BY CreatedDateUtc Desc
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY;
END

-------------------------------------------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Notification_GetNotificationComment]
	@notificationId nvarchar(max) = '',
	@createdDateUtc datetime,
	@skip int,
	@take int
as
begin
	--validate
	if @skip is null and @take is null and @createdDateUtc is null
		return;

	--get by skip and take
	if @skip is not null and @take is not null
	begin
		;with paging as(
			select [Id]
			  ,[NotificationId]
			  ,[OwnerId]
			  ,[Content]
			  ,[CreatedDateUtc]
			  ,[Subject]
			  ,[AttachmentList]
		  FROM [dbo].[NotificationComment]
		  Where NotificationId = @notificationId
		)

		SELECT 	*
		FROM paging CROSS JOIN (SELECT COUNT_BIG(*) AS RowCounts FROM paging) AS tCountRecords
		ORDER BY CreatedDateUtc DESC
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY;

		return;
	end

	--get by created date
	if @createdDateUtc is not null
	begin
		;with paging as(
			select [Id]
			  ,[NotificationId]
			  ,[OwnerId]
			  ,[Content]
			  ,[CreatedDateUtc]
			  ,[Subject]
			  ,[AttachmentList]
		  FROM [dbo].[NotificationComment]
		  Where NotificationId = @notificationId
		  and CreatedDateUtc > @createdDateUtc
		)

		SELECT 	*
		FROM paging CROSS JOIN (SELECT COUNT_BIG(*) AS RowCounts FROM paging) AS tCountRecords
		ORDER BY CreatedDateUtc DESC

		return;
	end	
END

-------------------------------------------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Notification_GetNotification]
	@linkId nvarchar(128),
	@type int,
	@onwerId nvarchar(128),
	@createdDateUtc datetime,
	@skip int,
	@take int
as
begin
	--validate
	if @linkId is null or @type is null
		return;

	if @skip is null and @take is null and @createdDateUtc is null
		return;			

	--get by created date
	if @createdDateUtc is not null
	begin
		;with paging as (
		SELECT [Id],[OwnerId],[Content],[CreatedDateUtc],[UpdatedDateUtc],[Subject],[AttachmentList]
		FROM [dbo].[Notification]
		WHERE [LinkId] = @linkId and [Type] = @type
		and ((@onwerId is not null and OwnerId = @onwerId) or (@onwerId is null))
		and CreatedDateUtc > @createdDateUtc)

		SELECT 	x.[Id],x.[OwnerId],x.[Content],x.[CreatedDateUtc],x.[Subject],x.[AttachmentList], 
		(select count(*) from Notificationcomment p where NotificationId = x.Id) as Total, (u.FirstName + ' ' + u.LastName) as Author,(SELECT COUNT_BIG(*) FROM paging)  AS RowCounts
		FROM paging x join [User] u on u.Id = x.OwnerId
		ORDER BY CreatedDateUtc Desc

		return;
	end	

	--get by skip and take
	if @skip is not null and @take is not null
	BEGIN
    
		;with paging as (
		SELECT [Id],[OwnerId],[Content],[CreatedDateUtc],[UpdatedDateUtc],[Subject],[AttachmentList]
		FROM [dbo].[Notification]
		WHERE [LinkId] = @linkId and [Type] = @type
		and ((@onwerId is not null and OwnerId = @onwerId) or (@onwerId is null)))

		SELECT 	x.[Id],x.[OwnerId],x.[Content],x.[CreatedDateUtc],x.[Subject],x.[AttachmentList], 
		(select count(*) from Notificationcomment p where NotificationId = x.Id) as Total, (u.FirstName + ' ' + u.LastName) as Author,(SELECT COUNT_BIG(*) FROM paging)  AS RowCounts
		FROM paging x join [User] u on u.Id = x.OwnerId
		ORDER BY CreatedDateUtc Desc
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY;
		return;
	end
END

-------------------------------------------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[Notification_GetMessageByCellNames]
	@cellNames varchar(max),
	@type int,
	@skip int,
	@take int
as
begin
	;with paging as (
		Select p.[Id],p.[OwnerId],p.[Content],p.[Subject],p.[AttachmentList],p.[UpdatedDateUtc],p.[CreatedDateUtc]
		FROM [dbo].[Notification] p
		Where ',' + @cellNames + ',' like '%,' + [LinkId] + ',%' and [Type] = @type 
	)

		SELECT 	*
		FROM paging CROSS JOIN (SELECT COUNT_BIG(*) AS RowCounts FROM paging) AS tCountRecords
		ORDER BY CreatedDateUtc Desc
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY;

		return;
END

-------------------------------------------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[GetCellsByCellName]
(
	@operatorId nvarchar(max),
	@cellName VARCHAR(100),
	@skip int,
	@take int
)
AS
BEGIN
	--validate
	IF @cellName IS NULL OR @skip IS NULL OR @take IS NULL
		RETURN;

	BEGIN
	;WITH paging AS ( SELECT c.*
						FROM CellDbms c
						WHERE c.CellName LIKE '%' + @cellName +'%' AND c.OperatorId =@operatorId
					)

	SELECT 	CellId,
			CreatedDateUtc,
			CellName,
			InterferenceDbm,
			RtwpDbm,
			CellType,
			RowCounts
	FROM paging CROSS JOIN (SELECT COUNT_BIG(*) AS RowCounts FROM paging) AS tCountRecords
	ORDER BY CreatedDateUtc DESC
	OFFSET @skip ROWS
	FETCH NEXT @take ROWS ONLY;
	END
END

-------------------------------------------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Alarm_GetAllAlarmsAndHistories]
(
	@operatorId nvarchar(max),
	@skip int,
	@take int
)
AS
BEGIN
	if @skip is null and @take is null
		return;
	;WITH paging AS (SELECT	a.CreatedDateUtc, 
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
										--a.ENodeBId,
										2 AS [AlarmType]
								FROM dbo.HistoryAlarmRecords a
								WHERE a.OperatorId = @operatorId
  
							)a
						)

	SELECT 	*			
	FROM paging CROSS JOIN (SELECT COUNT_BIG(*) AS RowCounts FROM paging) AS tCountRecords
	ORDER BY CreatedDateUtc DESC
	OFFSET @skip ROWS
	FETCH NEXT @take ROWS ONLY;

	
END

-------------------------------------------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Alarm_GetAlarmHistoriesByCellName]
(
	@operatorId nvarchar(max),
	@cellName VARCHAR(100),
	@skip int,
	@take int
)
AS
BEGIN
	--validate
	IF  @operatorId IS NULL or @cellName IS NULL OR @skip IS NULL OR @take IS NULL
		RETURN;

	BEGIN

	DECLARE @RowCounts bigint 
	SELECT @RowCounts=COUNT(*) From [dbo].[HistoryAlarmRecords] h Where h.OperatorId = @operatorId and  h.CellName LIKE '%' + @cellName +'%'
	SELECT  x.Slogan,
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
			@RowCounts as RowCounts
	FROM dbo.HistoryAlarmRecords x
	WHERE x.OperatorId = @operatorId and x.CellName LIKE '%' + @cellName +'%'
	ORDER By CreatedDateUtc DESC
	OFFSET @skip ROWS
	FETCH NEXT @take ROWS ONLY
		
	END
END

-------------------------------------------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Alarm_GetAlarmDataByKeywords]
(
	@OperatorId nvarchar(max),
	@Keywords nvarchar(max),
	@skip INT,
	@take INT
)
AS
BEGIN
	DECLARE @RowCounts bigint 
	SELECT @RowCounts=COUNT(*) From [dbo].[AlarmRecords] a Where (LOWER(a.Slogan) LIKE @Keywords or LOWER(a.CellName) LIKE @Keywords) and a.OperatorId = @OperatorId
	SELECT  a.CreatedDateUtc, 
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
			@RowCounts as RowCounts
	FROM dbo.AlarmRecords a
	WHERE (LOWER(a.Slogan) LIKE @Keywords or LOWER(a.CellName) LIKE @Keywords) and a.OperatorId = @OperatorId
	ORDER By CreatedDateUtc DESC
	OFFSET @skip ROWS
	FETCH NEXT @take ROWS ONLY
END

-------------------------------------------------------------------------------------


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Alarm_GetActiveAlarmRecordsByCellName]
(
	@operatorId nvarchar(max),
	@cellName VARCHAR(100),
	@skip int,
	@take int
)
AS
BEGIN
	--validate
	IF  @operatorId IS NULL or @cellName IS NULL OR @skip IS NULL OR @take IS NULL
		RETURN;

	BEGIN
		DECLARE @RowCounts bigint 
		SELECT @RowCounts=COUNT(*) From [dbo].[AlarmRecords] h Where h.OperatorId = @operatorId and  h.CellName LIKE '%' + @cellName +'%'
		SELECT	x.Slogan,
				x.Attribute,
				x.Severity,
				x.MoInstance,
				x.AdditionInfo,
				NULL as CeaseDateTimeUtc,
				x.CreatedDateUtc,
				x.CellType,
				x.LocalCellId,
				x.CellName,
				x.MoClass,
				x.OperatorId,@RowCounts as RowCounts
		FROM dbo.AlarmRecords x
		WHERE x.OperatorId = @operatorId and  x.CellName LIKE '%' + @cellName +'%'
		Order By CreatedDateUtc DESC
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY

		
	END
ENd

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Operator_GetAllOperatorsNotWorkspace]
	@skip int = 0,
	@take int = 10
as
BEGIN 
		;with paging as (
			SELECT * FROM [Operators] WHERE Id not in (select OperatorId from OperatorWorkspace) and IsDeleted = 0
		)

		SELECT 	*
		FROM paging CROSS JOIN (SELECT COUNT_BIG(*) AS RowCounts FROM paging) AS tCountRecords
		ORDER BY CreatedDateUtc Desc
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY;
END

-------------------------------------------------------------------------------------