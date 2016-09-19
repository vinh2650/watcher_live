USE [amssystemondemand]
GO

CREATE PROC [dbo].[Organization_GetAllOperatorsForInsightusAdmin_v3]
	@insightusAdminId VARCHAR(128),
	@skip INT = 0,
	@take INT = 10,
	@totalRecords BIGINT = 0 OUT
AS
BEGIN
IF @insightusAdminId IS NULL
			RETURN;
Select @totalRecords=COUNT(*) FROM Operators l JOIN [User] u ON u.Id = l.OwnerId
			WHERE OwnerId <> @insightusAdminId
SELECT l.[Id],l.[Name],l.[Band],l.[OwnerId],l.[OrganizationId],l.[CreatedDateUtc],
			u.[FirstName] AS [OwnerFirstName], u.[LastName] AS [OwnerLastName], u.[Email] AS [OwnerEmail]
FROM Operators l JOIN [User] u ON u.Id = l.OwnerId
WHERE OwnerId <> @insightusAdminId
Order By CreatedDateUtc DESC
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY
END
GO