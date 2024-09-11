DELIMITER $$
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

END$$

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

END$$

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
END$$

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

END$$

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
END$$

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
END$$

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
  END$$
DELIMITER ;





