/*获取层次结构文件夹(CTE集合运算)*/

WITH RECURSIVE
  [tmp]([Id], [Name], [SystemFolderName],[ParentId],[PhysicalFolderId],[IsShared],[Level]) AS(
    SELECT 
            [Id], 
            [Name], 
            0 as SystemFolderName,
            ParentId,
            PhysicalFolderId,
            0 as IsShared,
            0 AS [Level]
    FROM   [VirtualFolders]
    WHERE  [ParentId] = '{id.ToString().ToUpper()}'
    UNION ALL
    SELECT 
            [VirtualFolders].[Id], 
            [VirtualFolders].[name] AS [Name], 
            0 as SystemFolderName,
            [VirtualFolders].ParentId as ParentId,
            [VirtualFolders].PhysicalFolderId as PhysicalFolderId,
            0 as IsShared,
            [tmp].[Level] + 1 AS [level]
    FROM   [VirtualFolders]
            JOIN [tmp] ON [VirtualFolders].[ParentId] = [tmp].[Id]
  )
SELECT * FROM  [tmp];


 /*获取层次结构文件夹(CTE集合运算)*/

WITH RECURSIVE
  [tmp]([Id], [Name], [SystemFolderName],[ParentId],[PhysicalFolderId],[IsShared],[Level]) AS(
    SELECT 
            [Id], 
            [Name], 
            0 as SystemFolderName,
            ParentId,
            PhysicalFolderId,
            0 as IsShared,
            0 AS [Level]
    FROM   [VirtualFolders]
    WHERE  [Id] = '{id.ToString().ToUpper()}'
    UNION ALL
    SELECT 
            [VirtualFolders].[Id], 
            [VirtualFolders].[name] AS [Name], 
            0 as SystemFolderName,
            [VirtualFolders].ParentId as ParentId,
            [VirtualFolders].PhysicalFolderId as PhysicalFolderId,
            0 as IsShared,
            [tmp].[Level] + 1 AS [level]
    FROM   [VirtualFolders]
            JOIN [tmp] ON [VirtualFolders].[Id] = [tmp].[ParentId]
  )
SELECT * FROM  [tmp];