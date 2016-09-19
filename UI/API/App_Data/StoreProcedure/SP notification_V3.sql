USE [amssystemondemand]
GO

/****** Object:  StoredProcedure [dbo].[Notification_GetNotification]    Script Date: 8/18/2016 2:09:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[Notification_GetNotification_V3]
	@linkId NVARCHAR(128),
	@type INT,
	@onwerId NVARCHAR(128),
	@createdDateUtc DATETIME,
	@skip INT,
	@take INT,
	@operatorId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX),
	@totalRecords BIGINT = 0 OUT
AS
BEGIN
	--validate
	if @linkId is null or @type is NULL OR @jobId IS NULL OR @operatorId IS NULL
		return;

	if @skip is null and @take is null and @createdDateUtc is null
		return;			

		--get by skip and take
	IF @skip IS NOT NULL AND @take IS NOT NULL
	BEGIN
		
		SELECT @totalRecords=COUNT(*) FROM [dbo].[Notification]
		WHERE [LinkId] = @linkId AND [Type] = @type
		AND ((@onwerId IS NOT NULL AND OwnerId = @onwerId) OR (@onwerId IS NULL)) AND OperatorId = @operatorId AND JobId = @JobId

		SELECT x.[Id],x.[OwnerId],x.[Content],x.[CreatedDateUtc],x.[Subject],x.[AttachmentList], 
		(SELECT COUNT(*) FROM Notificationcomment p WHERE NotificationId = x.Id) AS Total, (u.FirstName + ' ' + u.LastName) AS Author
		FROM [dbo].[Notification] x JOIN [User] u ON u.Id = x.OwnerId
		WHERE  [LinkId] = @linkId AND [Type] = @type
		AND ((@onwerId IS NOT NULL AND OwnerId = @onwerId) OR (@onwerId IS NULL)) AND OperatorId = @operatorId AND JobId = @JobId
		ORDER BY x.CreatedDateUtc
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY

		RETURN;
	END

	--get by created date
	IF @createdDateUtc IS NOT NULL
	BEGIN
		SELECT @totalRecords=COUNT(*)	FROM [dbo].[Notification]
		WHERE [LinkId] = @linkId AND [Type] = @type
		AND ((@onwerId IS NOT NULL AND OwnerId = @onwerId) OR (@onwerId IS NULL))
		AND CreatedDateUtc > @createdDateUtc AND OperatorId = @operatorId AND JobId = @JobId

		
		SELECT x.[Id],x.[OwnerId],x.[Content],x.[CreatedDateUtc],x.[Subject],x.[AttachmentList], 
		(SELECT COUNT(*) FROM Notificationcomment p WHERE NotificationId = x.Id) AS Total, (u.FirstName + ' ' + u.LastName) AS Author
		FROM [dbo].[Notification] x JOIN [User] u ON u.Id = x.OwnerId
		WHERE  [LinkId] = @linkId AND [Type] = @type
		AND ((@onwerId IS NOT NULL AND OwnerId = @onwerId) OR (@onwerId IS NULL)) AND OperatorId = @operatorId AND JobId = @JobId
		ORDER BY x.CreatedDateUtc
		RETURN
	END	
END

GO

-------------------------

CREATE PROC [dbo].[Notification_NewNotification_V3]
(	
	@id NVARCHAR(128),
	@ownerId NVARCHAR(MAX),
	@jobId NVARCHAR(MAX),
	@operatorId NVARCHAR(MAX),
	@linkId NVARCHAR(MAX),
	@content NVARCHAR(MAX),
	@subject NVARCHAR(MAX),
	@attachmentList NVARCHAR(MAX),
	@updateDateUtc DATETIME,
	@createDateUtc DATETIME,
	@type INT
)
AS 
BEGIN 
	IF(@updateDateUtc IS NULL)
	SET @updateDateUtc = GETUTCDATE(); 

	INSERT INTO [dbo].[Notification](
		[Id],
		[OwnerId],
		[JobId],
		[OperatorId],
		[LinkId],
		[Content],
		[Subject],
		[AttachmentList],
		[UpdatedDateUtc],
		[CreatedDateUtc],
		[Type]
	)
	VALUES(
		@id,
		@ownerId,
		@jobId,
		@operatorId,
		@linkId,
		@content,
		@subject,
		@attachmentList, 
		@updateDateUtc,
		@createDateUtc,		
		@type
	)
END



GO

---------------------


CREATE PROC [dbo].[Notification_GetMessageByCellNames_V3]
	@cellNames VARCHAR(MAX),
	@type INT,
	@skip INT,
	@take INT,
	@totalRecords BIGINT = 0 OUT
AS
BEGIN
	Select @totalRecords=COUNT(*) FROM [dbo].[Notification] p
		WHERE ',' + @cellNames + ',' LIKE '%,' + [LinkId] + ',%' AND [Type] = @type

	SELECT x.[Id],x.[OwnerId],x.[Content],x.[Subject],x.[AttachmentList],x.[UpdatedDateUtc],x.[CreatedDateUtc]
	FROM [dbo].[Notification] x
	WHERE ',' + @cellNames + ',' LIKE '%,' + [LinkId] + ',%' AND [Type] = @type
	Order By CreatedDateUtc DESC
	OFFSET @skip ROWS
	FETCH NEXT @take ROWS ONLY


	RETURN;
END



GO


--------------

--------------------------------------------------------------------------

CREATE PROC [dbo].[Notification_GetComment_V3]
	@postId NVARCHAR(MAX) = '',
	@createdDateUtc DATETIME,
	@skip INT = 0,
	@take INT = 10,
	@totalRecords BIGINT = 0 OUT
AS
BEGIN
	--validate
	if @skip is null and @take is null and @createdDateUtc is null
		return;

	--get by skip and take
	IF @skip IS NOT NULL AND @take IS NOT NULL
	BEGIN
		Select @totalRecords=COUNT(*)  FROM [dbo].[PostComment]
		  WHERE PostId = @postId

		SELECT [Id]
			  ,[PostId]
			  ,[OwnerId]
			  ,[Content]
			  ,[CreatedDateUtc]
			  ,[Subject]
			  ,[AttachmentList]
		FROM [dbo].[PostComment]
		WHERE PostId = @postId
		Order By CreatedDateUtc DESC
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY
		RETURN;
	END

	--get by created date
	IF @createdDateUtc IS NOT NULL
	BEGIN
		Select @totalRecords=COUNT(*)  FROM [dbo].[PostComment]
		  WHERE PostId = @postId PostId = @postId AND CreatedDateUtc > @createdDateUtc

		SELECT [Id]
			  ,[PostId]
			  ,[OwnerId]
			  ,[Content]
			  ,[CreatedDateUtc]
			  ,[Subject]
			  ,[AttachmentList]
		FROM [dbo].[PostComment]
		WHERE PostId = @postId PostId = @postId AND CreatedDateUtc > @createdDateUtc
		ORDER BY CreatedDateUtc

		RETURN;
	END	
END

GO

------------

CREATE PROC [dbo].[Notification_GetNotificationComment_V3]
	@notificationId NVARCHAR(MAX) = '',
	@createdDateUtc DATETIME,
	@skip INT = 0,
	@take INT = 10,
	@totalRecords BIGINT = 0 OUT
AS
BEGIN
	--validate
	if @skip is null and @take is null and @createdDateUtc is null
		return;

	--get by skip and take
	IF @skip IS NOT NULL AND @take IS NOT NULL
	BEGIN
		Select @totalRecords=COUNT(*) FROM [dbo].[NotificationComment]
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
		Order By CreatedDateUtc DESC
		OFFSET @skip ROWS
		FETCH NEXT @take ROWS ONLY
		RETURN;
	END

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
		Order By CreatedDateUtc DESC
		RETURN;
	END	
END


GO