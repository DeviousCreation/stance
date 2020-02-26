﻿CREATE VIEW [ReadAggregation].[vwUser]
AS
SELECT 
		u.Id
	,	u.EmailAddress
	,	Cast(Case when u.WhenLocked is null then 0 else 1 end as bit) as IsLocked
	,	u.IsLockable
	,	u.WhenLastAuthenticated
	,	u.WhenCreated
	,	p.FirstName
	,	p.LastName
FROM [Identity].[User] u
LEFT JOIN [Identity].Profile p
	ON u.Id = p.UserId
