INSERT Users (`Id`, `FirstName`, `LastName`, `IsDeleted`, `IsActive`, `ProfilePhoto`, `Provider`, `Address`, `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`, `DeletedDate`, `DeletedBy`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`, `IsAdmin`) VALUES (N'1a5cf5b9-ead8-495c-8719-2d8be776f452', N'Norman', N'Russell', 0, 1, N'user-profile.jpg', NULL, NULL, CAST(N'2021-01-09T16:00:55.3200000' AS DATETIME(6)), NULL, CAST(N'2021-04-09T22:13:04.3607628' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', NULL, NULL, N'employee@gmail.com', N'EMPLOYEE@GMAIL.COM', N'employee@gmail.com', N'EMPLOYEE@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEKWs5TYpiKZTo10GsYT3ydUD92Xv9PzHyaE6IlWewhVAcBXpQ92H1g7zz9r2wNXTTw==', N'C6DDSWCQJIFOEWSOC2IEIDGXZ7YOHGAC', N'542d648b-582f-464b-9264-4efd2a4f8b1a', N'7684012345', 0, 0, NULL, 1, 0, 1);
 
INSERT Users (`Id`, `FirstName`, `LastName`, `IsDeleted`, `IsActive`, `ProfilePhoto`, `Provider`, `Address`, `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`, `DeletedDate`, `DeletedBy`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`, `IsAdmin`) VALUES (N'4b352b37-332a-40c6-ab05-e38fcf109719', N'Frederic', N'Holland', 0, 1, N'user-profile.jpg', NULL, NULL, CAST(N'2021-01-09T16:00:55.3200000' AS DATETIME(6)), NULL, CAST(N'2021-04-09T22:13:30.0911557' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', NULL, NULL, N'admin@gmail.com', N'ADMIN@GMAIL.COM', N'admin@gmail.com', N'ADMIN@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEEkx5K65gWhkIDvtcI3QVCom8fFRVWBIVlDWGqPujKdUWwSs2/0bB2fFzTaAq8z3pA==', N'EZNIRU4TFNZUE4VWL4CLRBHP7VMTICHA', N'56223fd5-d4f1-4811-a806-bf8bdff9bb5c', N'3360123459', 0, 0, NULL, 1, 0, 1);
 
INSERT EmailTemplates (`Id`, `Name`, `Subject`, `Body`, `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`, `DeletedDate`, `DeletedBy`, `IsDeleted`) VALUES (N'bf6bd6f0-75f2-45ee-9dd0-360776fe1bf2', N'Reset Pasword', N'Reset Password Request', N'<p>Hi ##UserName##,</p><p>We got the reset password request from the account please link on below Link to reset your password:</p><p>##link##</p><p><strong><span style="color:#0e8a16;">Thanks,</span></strong></p><p><strong><span style="color:#0e8a16;">Team ML Glob Tech</span></strong></p>', CAST(N'2021-04-15T12:15:37.9231606' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', CAST(N'2021-04-15T12:15:37.9231653' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', NULL, NULL, 0);
 
INSERT EmailTemplates (`Id`, `Name`, `Subject`, `Body`, `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`, `DeletedDate`, `DeletedBy`, `IsDeleted`) VALUES (N'634c2a67-e390-42bc-aaec-68fb0ada7b31', N'Welcome Email', N'Welcome to ML Glob Tech', N'<p>Hi ##UserName##,</p><p>Welcome to ML Glob Tech.</p><p>Please Fill free to contact.</p><p><strong><span style="color:#0e8a16;">Thanks</span></strong></p><p><strong><span style="color:#0e8a16;">Team ML Glob Tech</span></strong></p>', CAST(N'0001-01-01T00:00:00.0000000' AS DATETIME(6)), N'00000000-0000-0000-0000-000000000000', CAST(N'2021-04-15T12:15:54.7274775' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', NULL, NULL, 0);
 
INSERT PhysicalFolders (`Id`, `Name`, `ParentId`, `Size`, `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`, `DeletedDate`, `DeletedBy`, `IsDeleted`) VALUES (N'79073ec1-51e2-4772-95e6-9b06075a174b', N'全部', NULL, NULL, CAST(N'2021-06-01T00:00:00.0000000' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', CAST(N'2021-06-01T00:00:00.0000000' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', NULL, NULL, 0);
 
INSERT VirtualFolders (`Id`, `Name`, `ParentId`, `Size`, `PhysicalFolderId`, `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`, `DeletedDate`, `DeletedBy`, `IsDeleted`, `IsShared`) VALUES (N'a4d06132-d76c-49b5-8472-2bf78ac4147e', N'全部', NULL, NULL, N'79073ec1-51e2-4772-95e6-9b06075a174b', CAST(N'2021-06-01T00:00:00.0000000' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', CAST(N'2021-06-01T00:00:00.0000000' AS DATETIME(6)), N'4b352b37-332a-40c6-ab05-e38fcf109719', NULL, NULL, 0,0);
 
 
INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsFolderCreate',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsFileUpload',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsDeleteFileFolder',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsSharedFileFolder',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsSendEmail',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsRenameFile',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsDownloadFile',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsCopyFile',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsCopyFolder',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsMoveFile',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'1A5CF5B9-EAD8-495C-8719-2D8BE776F452', 'IsSharedLink',1);


INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsFolderCreate',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsFileUpload',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsDeleteFileFolder',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsSharedFileFolder',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsSendEmail',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsRenameFile',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsDownloadFile',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsCopyFile',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsCopyFolder',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsMoveFile',1);

INSERT  AspNetUserClaims ( `UserId`, `ClaimType`, `ClaimValue`) VALUES (N'4B352B37-332A-40C6-AB05-E38FCF109719', 'IsSharedLink',1);



CREATE DEFINER=`root`@`localhost` PROCEDURE `getPhysicalFolderChildsHierarchyById`(
p_Id char(36))
BEGIN
DECLARE v_IsShared TINYINT DEFAULT 0;

WITH RECURSIVE CTE (Id,
            Name,
			SystemFolderName,
            ParentId,
            PhysicalFolderId,
			Level,
            IsShared
) AS
(
 SELECT  Id,
            Name,
			SystemFolderName,
            ParentId,
            UUID() as PhysicalFolderId,
			0 AS Level,
            v_IsShared as IsShared
			FROM PhysicalFolders
WHERE ParentId = p_Id
  UNION ALL
 SELECT  
		f.Id,
		f.Name,
		f.SystemFolderName,
		f.ParentId, 
		UUID() as PhysicalFolderId, 
		Level + 1,
        v_IsShared as IsShared
	FROM PhysicalFolders f
    INNER JOIN CTE ON f.ParentId = CTE.Id
)
SELECT * FROM CTE;

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE `getSharedParentsHierarchyById`(
p_Id char(36))
BEGIN
DECLARE v_IsShared TINYINT DEFAULT 0;

WITH RECURSIVE CTE (
			Id,
            Name,
			SystemFolderName,
            ParentId,
            PhysicalFolderId,
			Level,
            IsShared
) AS
(
 SELECT  Id,
            Name,
			0 as SystemFolderName,
            ParentId,
            PhysicalFolderId,
			0 AS Level,
            IsShared
			FROM VirtualFolders
WHERE Id = p_Id
  UNION ALL
 SELECT  
		f.Id,
		f.Name,
		0 as SystemFolderName,
		f.ParentId, 
		f.PhysicalFolderId, 
		Level + 1,
         f.IsShared
	FROM VirtualFolders f
    INNER JOIN CTE ON f.Id = CTE.ParentId
)
SELECT  * FROM CTE WHERE Id!=p_Id AND IsShared=1 limit 1;

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE `getPhysicalFolderParentsHierarchyById`(
p_Id char(36))
BEGIN
DECLARE v_IsShared TINYINT DEFAULT 0;

WITH RECURSIVE CTE (
			Id,
            Name,
			SystemFolderName,
            ParentId,
            PhysicalFolderId,
			Level,
            IsShared
) AS
(
 SELECT  Id,
            Name,
			SystemFolderName,
            ParentId,
            UUID() as PhysicalFolderId,
			0 AS Level,
            v_IsShared as IsShared
			FROM PhysicalFolders
WHERE Id = p_Id
  UNION ALL
 SELECT  
		f.Id,
		f.Name,
		f.SystemFolderName,
		f.ParentId, 
		UUID() as PhysicalFolderId, 
		Level + 1,
        v_IsShared as IsShared
	FROM PhysicalFolders f
    INNER JOIN CTE ON f.Id = CTE.ParentId
)
SELECT * FROM CTE;

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE `getSharedChildsHierarchyById`(
p_Id char(36))
BEGIN

WITH RECURSIVE CTE (
			Id,
            Name,
			SystemFolderName,
            ParentId,
            PhysicalFolderId,
			Level,
            IsShared
) AS
(
 SELECT  Id,
            Name,
			0 as SystemFolderName,
            ParentId,
            PhysicalFolderId,
			0 AS Level,
             IsShared
			FROM VirtualFolders
WHERE ParentId = p_Id
  UNION ALL
 SELECT  
		f.Id,
		f.Name,
		0 as SystemFolderName,
		f.ParentId, 
		f.PhysicalFolderId, 
		Level + 1,
       f.IsShared
	FROM VirtualFolders f
    INNER JOIN CTE ON f.ParentId = CTE.Id
)
SELECT * FROM CTE WHERE IsShared=1 LIMIT 1;
END;

CREATE DEFINER=`root`@`localhost` PROCEDURE `getVirtualFolderChildsHierarchyById`(
p_Id char(36))
BEGIN
DECLARE v_IsShared TINYINT DEFAULT 0;

WITH RECURSIVE CTE (
			Id,
            Name,
			SystemFolderName,
            ParentId,
            PhysicalFolderId,
			Level,
            IsShared
) AS
(
 SELECT  Id,
            Name,
			0 as SystemFolderName,
            ParentId,
            PhysicalFolderId,
			0 AS Level,
             IsShared
			FROM VirtualFolders
WHERE ParentId = p_Id
  UNION ALL
 SELECT  
		f.Id,
		f.Name,
		0 as SystemFolderName,
		f.ParentId, 
		f.PhysicalFolderId, 
		Level + 1,
       f.IsShared
	FROM VirtualFolders f
    INNER JOIN CTE ON f.ParentId = CTE.Id
)
SELECT * FROM CTE;
END;

CREATE DEFINER=`root`@`localhost` PROCEDURE `getVirtualFolderParentsHierarchyById`(
p_Id char(36))
BEGIN

WITH RECURSIVE CTE (
			Id,
            Name,
			SystemFolderName,
            ParentId,
            PhysicalFolderId,
			Level,
            IsShared
) AS
(
 SELECT     Id,
            Name,
			0 as SystemFolderName,
            ParentId,
            PhysicalFolderId,
			0 AS Level,
             IsShared
			FROM VirtualFolders
WHERE Id = p_Id
  UNION ALL
 SELECT  
		f.Id,
		f.Name,
		0 as SystemFolderName,
		f.ParentId, 
		f.PhysicalFolderId, 
		Level + 1,
       f.IsShared
	FROM VirtualFolders f
    INNER JOIN CTE ON f.Id = CTE.ParentId
)
SELECT * FROM CTE;
END;

CREATE DEFINER=`root`@`localhost` PROCEDURE `NLog_AddEntry_p`(
  p_machineName nvarchar(200),
  p_logged datetime(3),
  p_level varchar(5),
  p_message longtext,
  p_logger nvarchar(300),
  p_properties longtext,
  p_callsite nvarchar(300),
  p_exception longtext
)
BEGIN
  INSERT INTO NLog (
	`Id`,
    `MachineName`,
    `Logged`,
    `Level`,
    `Message`,
    `Logger`,
    `Properties`,
    `Callsite`,
    `Exception`,
	`Source`
  ) VALUES (
    uuid(),
    p_machineName,
    p_logged,
    p_level,
    p_message,
    p_logger,
    p_properties,
    p_callsite,
    p_exception,
	'.Net Core'
  );
  END;

