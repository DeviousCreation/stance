﻿CREATE TABLE [Identity].[AuthenticatorApp]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Key] VARCHAR(500) NOT NULL
    ,   [WhenEnrolled] DATETIME2 NOT NULL
    ,   [WhenRevoked] DATETIME2 NULL
    ,   [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
)