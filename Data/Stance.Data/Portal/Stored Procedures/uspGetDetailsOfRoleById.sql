﻿CREATE PROCEDURE [Portal].[uspGetDetailsOfRoleById]
	@roleId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT 
			r.Id
		,	r.Name
	FROM [AccessProtection].[Role] r
	WHERE r.id = @roleId;

    SELECT ResourceId
	FROM [AccessProtection].[RoleResource]
	WHERE RoleId = @roleId;
END

