--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20210822054753_Initial','5.0.7'),('20210822054818_Initial_Sql_Data','5.0.7');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;



--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FirstName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LastName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsDeleted` tinyint(1) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `ProfilePhoto` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Provider` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Address` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `ModifiedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsAdmin` tinyint(1) NOT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES ('1a5cf5b9-ead8-495c-8719-2d8be776f452','Norman','Russell',0,1,'user-profile.jpg',NULL,NULL,'2021-01-09 16:00:55',NULL,'2021-04-09 22:13:04','4b352b37-332a-40c6-ab05-e38fcf109719',NULL,NULL,1,'employee@gmail.com','EMPLOYEE@GMAIL.COM','employee@gmail.com','EMPLOYEE@GMAIL.COM',0,'AQAAAAEAACcQAAAAEKWs5TYpiKZTo10GsYT3ydUD92Xv9PzHyaE6IlWewhVAcBXpQ92H1g7zz9r2wNXTTw==','C6DDSWCQJIFOEWSOC2IEIDGXZ7YOHGAC','542d648b-582f-464b-9264-4efd2a4f8b1a','7684012345',0,0,NULL,1,0),('4b352b37-332a-40c6-ab05-e38fcf109719','Frederic','Holland',0,1,'user-profile.jpg',NULL,NULL,'2021-01-09 16:00:55',NULL,'2021-04-09 22:13:30','4b352b37-332a-40c6-ab05-e38fcf109719',NULL,NULL,1,'admin@gmail.com','ADMIN@GMAIL.COM','admin@gmail.com','ADMIN@GMAIL.COM',0,'AQAAAAEAACcQAAAAEEkx5K65gWhkIDvtcI3QVCom8fFRVWBIVlDWGqPujKdUWwSs2/0bB2fFzTaAq8z3pA==','EZNIRU4TFNZUE4VWL4CLRBHP7VMTICHA','56223fd5-d4f1-4811-a806-bf8bdff9bb5c','3360123459',0,0,NULL,1,0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `RoleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `physicalfolders`
--

DROP TABLE IF EXISTS `physicalfolders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `physicalfolders` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `SystemFolderName` bigint NOT NULL AUTO_INCREMENT,
  `ParentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Size` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `AK_PhysicalFolder` (`SystemFolderName`),
  KEY `IX_PhysicalFolders_Name_IsDeleted_ParentId` (`Name`,`IsDeleted`,`ParentId`),
  KEY `IX_PhysicalFolders_ParentId` (`ParentId`),
  CONSTRAINT `FK_PhysicalFolders_PhysicalFolders_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `physicalfolders` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `physicalfolders`
--

LOCK TABLES `physicalfolders` WRITE;
/*!40000 ALTER TABLE `physicalfolders` DISABLE KEYS */;
INSERT INTO `physicalfolders` VALUES ('79073ec1-51e2-4772-95e6-9b06075a174b','All Files',1,NULL,NULL,'2021-06-01 00:00:00','4b352b37-332a-40c6-ab05-e38fcf109719','2021-06-01 00:00:00','4b352b37-332a-40c6-ab05-e38fcf109719',NULL,NULL,0);
/*!40000 ALTER TABLE `physicalfolders` ENABLE KEYS */;
UNLOCK TABLES;
--
-- Table structure for table `documents`
--

DROP TABLE IF EXISTS `documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documents` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhysicalFolderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Extension` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Path` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Size` bigint NOT NULL,
  `ThumbnailPath` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Documents_CreatedBy` (`CreatedBy`),
  KEY `IX_Documents_DeletedBy` (`DeletedBy`),
  KEY `IX_Documents_ModifiedBy` (`ModifiedBy`),
  KEY `IX_Documents_Name_IsDeleted_PhysicalFolderId` (`Name`,`IsDeleted`,`PhysicalFolderId`),
  KEY `IX_Documents_PhysicalFolderId` (`PhysicalFolderId`),
  CONSTRAINT `FK_Documents_PhysicalFolders_PhysicalFolderId` FOREIGN KEY (`PhysicalFolderId`) REFERENCES `physicalfolders` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Documents_Users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_Documents_Users_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_Documents_Users_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documents`
--

LOCK TABLES `documents` WRITE;
/*!40000 ALTER TABLE `documents` DISABLE KEYS */;
/*!40000 ALTER TABLE `documents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documentaudittrails`
--

DROP TABLE IF EXISTS `documentaudittrails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentaudittrails` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Comment` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DocumentAuditTrails_DocumentId` (`DocumentId`),
  CONSTRAINT `FK_DocumentAuditTrails_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentaudittrails`
--

LOCK TABLES `documentaudittrails` WRITE;
/*!40000 ALTER TABLE `documentaudittrails` DISABLE KEYS */;
/*!40000 ALTER TABLE `documentaudittrails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documentcomments`
--

DROP TABLE IF EXISTS `documentcomments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentcomments` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Comment` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DocumentComments_CreatedBy` (`CreatedBy`),
  KEY `IX_DocumentComments_DeletedBy` (`DeletedBy`),
  KEY `IX_DocumentComments_DocumentId` (`DocumentId`),
  KEY `IX_DocumentComments_ModifiedBy` (`ModifiedBy`),
  CONSTRAINT `FK_DocumentComments_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DocumentComments_Users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_DocumentComments_Users_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_DocumentComments_Users_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentcomments`
--

LOCK TABLES `documentcomments` WRITE;
/*!40000 ALTER TABLE `documentcomments` DISABLE KEYS */;
/*!40000 ALTER TABLE `documentcomments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documentdeleteds`
--

DROP TABLE IF EXISTS `documentdeleteds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentdeleteds` (
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`DocumentId`,`UserId`),
  KEY `IX_DocumentDeleteds_CreatedBy` (`CreatedBy`),
  KEY `IX_DocumentDeleteds_DeletedBy` (`DeletedBy`),
  KEY `IX_DocumentDeleteds_ModifiedBy` (`ModifiedBy`),
  KEY `IX_DocumentDeleteds_UserId` (`UserId`),
  CONSTRAINT `FK_DocumentDeleteds_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DocumentDeleteds_Users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_DocumentDeleteds_Users_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_DocumentDeleteds_Users_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_DocumentDeleteds_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentdeleteds`
--

LOCK TABLES `documentdeleteds` WRITE;
/*!40000 ALTER TABLE `documentdeleteds` DISABLE KEYS */;
/*!40000 ALTER TABLE `documentdeleteds` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documentreminders`
--

DROP TABLE IF EXISTS `documentreminders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentreminders` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `StartDate` datetime NOT NULL,
  `Frequency` int NOT NULL,
  `Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DocumentReminders_DocumentId` (`DocumentId`),
  CONSTRAINT `FK_DocumentReminders_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentreminders`
--

LOCK TABLES `documentreminders` WRITE;
/*!40000 ALTER TABLE `documentreminders` DISABLE KEYS */;
/*!40000 ALTER TABLE `documentreminders` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Table structure for table `documentshareablelinks`
--

DROP TABLE IF EXISTS `documentshareablelinks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentshareablelinks` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `LinkExpiryTime` datetime DEFAULT NULL,
  `Password` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LinkCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsLinkExpired` tinyint(1) NOT NULL,
  `IsAllowDownload` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DocumentShareableLinks_DocumentId` (`DocumentId`),
  CONSTRAINT `FK_DocumentShareableLinks_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentshareablelinks`
--

LOCK TABLES `documentshareablelinks` WRITE;
/*!40000 ALTER TABLE `documentshareablelinks` DISABLE KEYS */;
/*!40000 ALTER TABLE `documentshareablelinks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documentstarreds`
--

DROP TABLE IF EXISTS `documentstarreds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentstarreds` (
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`DocumentId`,`UserId`),
  KEY `IX_DocumentStarreds_UserId` (`UserId`),
  CONSTRAINT `FK_DocumentStarreds_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DocumentStarreds_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentstarreds`
--

LOCK TABLES `documentstarreds` WRITE;
/*!40000 ALTER TABLE `documentstarreds` DISABLE KEYS */;
/*!40000 ALTER TABLE `documentstarreds` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documenttokens`
--

DROP TABLE IF EXISTS `documenttokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documenttokens` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Token` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CreatedDate` datetime NOT NULL,
  `DocumentVersionId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documenttokens`
--

LOCK TABLES `documenttokens` WRITE;
/*!40000 ALTER TABLE `documenttokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `documenttokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documentuserpermissions`
--

DROP TABLE IF EXISTS `documentuserpermissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentuserpermissions` (
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`DocumentId`,`UserId`),
  KEY `IX_DocumentUserPermissions_DocumentId_UserId` (`DocumentId`,`UserId`),
  KEY `IX_DocumentUserPermissions_UserId` (`UserId`),
  CONSTRAINT `FK_DocumentUserPermissions_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DocumentUserPermissions_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentuserpermissions`
--

LOCK TABLES `documentuserpermissions` WRITE;
/*!40000 ALTER TABLE `documentuserpermissions` DISABLE KEYS */;
/*!40000 ALTER TABLE `documentuserpermissions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documentversions`
--

DROP TABLE IF EXISTS `documentversions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentversions` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Path` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Size` bigint NOT NULL,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DocumentVersions_CreatedBy` (`CreatedBy`),
  KEY `IX_DocumentVersions_DeletedBy` (`DeletedBy`),
  KEY `IX_DocumentVersions_DocumentId` (`DocumentId`),
  KEY `IX_DocumentVersions_ModifiedBy` (`ModifiedBy`),
  CONSTRAINT `FK_DocumentVersions_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DocumentVersions_Users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_DocumentVersions_Users_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_DocumentVersions_Users_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentversions`
--

LOCK TABLES `documentversions` WRITE;
/*!40000 ALTER TABLE `documentversions` DISABLE KEYS */;
/*!40000 ALTER TABLE `documentversions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `emailsmtpsettings`
--

DROP TABLE IF EXISTS `emailsmtpsettings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `emailsmtpsettings` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Host` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsEnableSSL` tinyint(1) NOT NULL,
  `Port` int NOT NULL,
  `IsDefault` tinyint(1) NOT NULL,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_EmailSMTPSettings_CreatedBy` (`CreatedBy`),
  KEY `IX_EmailSMTPSettings_DeletedBy` (`DeletedBy`),
  KEY `IX_EmailSMTPSettings_ModifiedBy` (`ModifiedBy`),
  CONSTRAINT `FK_EmailSMTPSettings_Users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_EmailSMTPSettings_Users_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_EmailSMTPSettings_Users_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `emailsmtpsettings`
--

LOCK TABLES `emailsmtpsettings` WRITE;
/*!40000 ALTER TABLE `emailsmtpsettings` DISABLE KEYS */;
/*!40000 ALTER TABLE `emailsmtpsettings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `emailtemplates`
--

DROP TABLE IF EXISTS `emailtemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `emailtemplates` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Subject` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Body` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `emailtemplates`
--

LOCK TABLES `emailtemplates` WRITE;
/*!40000 ALTER TABLE `emailtemplates` DISABLE KEYS */;
INSERT INTO `emailtemplates` VALUES ('634c2a67-e390-42bc-aaec-68fb0ada7b31','Welcome Email','Welcome to ML Glob Tech','<p>Hi ##UserName##,</p><p>Welcome to ML Glob Tech.</p><p>Please Fill free to contact.</p><p><strong><span style=\"color:#0e8a16;\">Thanks</span></strong></p><p><strong><span style=\"color:#0e8a16;\">Team ML Glob Tech</span></strong></p>','0001-01-01 00:00:00','00000000-0000-0000-0000-000000000000','2021-04-15 12:15:55','4b352b37-332a-40c6-ab05-e38fcf109719',NULL,NULL,0),('bf6bd6f0-75f2-45ee-9dd0-360776fe1bf2','Reset Pasword','Reset Password Request','<p>Hi ##UserName##,</p><p>We got the reset password request from the account please link on below Link to reset your password:</p><p>##link##</p><p><strong><span style=\"color:#0e8a16;\">Thanks,</span></strong></p><p><strong><span style=\"color:#0e8a16;\">Team ML Glob Tech</span></strong></p>','2021-04-15 12:15:38','4b352b37-332a-40c6-ab05-e38fcf109719','2021-04-15 12:15:38','4b352b37-332a-40c6-ab05-e38fcf109719',NULL,NULL,0);
/*!40000 ALTER TABLE `emailtemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `hierarchyfolders`
--

DROP TABLE IF EXISTS `hierarchyfolders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hierarchyfolders` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SystemFolderName` bigint NOT NULL,
  `ParentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Level` int NOT NULL,
  `PhysicalFolderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `IsShared` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `hierarchyfolders`
--

LOCK TABLES `hierarchyfolders` WRITE;
/*!40000 ALTER TABLE `hierarchyfolders` DISABLE KEYS */;
/*!40000 ALTER TABLE `hierarchyfolders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `loginaudits`
--

DROP TABLE IF EXISTS `loginaudits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `loginaudits` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LoginTime` datetime NOT NULL,
  `RemoteIP` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Provider` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Latitude` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Longitude` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `loginaudits`
--

LOCK TABLES `loginaudits` WRITE;
/*!40000 ALTER TABLE `loginaudits` DISABLE KEYS */;
/*!40000 ALTER TABLE `loginaudits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nlog`
--

DROP TABLE IF EXISTS `nlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nlog` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MachineName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Logged` datetime NOT NULL,
  `Level` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Logger` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Properties` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Callsite` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Exception` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Source` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nlog`
--

LOCK TABLES `nlog` WRITE;
/*!40000 ALTER TABLE `nlog` DISABLE KEYS */;
/*!40000 ALTER TABLE `nlog` ENABLE KEYS */;
UNLOCK TABLES;



--
-- Table structure for table `physicalfolderusers`
--

DROP TABLE IF EXISTS `physicalfolderusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `physicalfolderusers` (
  `FolderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`FolderId`,`UserId`),
  KEY `IX_PhysicalFolderUsers_UserId` (`UserId`),
  CONSTRAINT `FK_PhysicalFolderUsers_PhysicalFolders_FolderId` FOREIGN KEY (`FolderId`) REFERENCES `physicalfolders` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PhysicalFolderUsers_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `physicalfolderusers`
--

LOCK TABLES `physicalfolderusers` WRITE;
/*!40000 ALTER TABLE `physicalfolderusers` DISABLE KEYS */;
/*!40000 ALTER TABLE `physicalfolderusers` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Table structure for table `virtualfolders`
--

DROP TABLE IF EXISTS `virtualfolders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `virtualfolders` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ParentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Size` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsShared` tinyint(1) NOT NULL,
  `PhysicalFolderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VirtualFolders_Name_IsDeleted_ParentId_PhysicalFolderId` (`Name`,`IsDeleted`,`ParentId`,`PhysicalFolderId`),
  KEY `IX_VirtualFolders_ParentId` (`ParentId`),
  KEY `IX_VirtualFolders_PhysicalFolderId` (`PhysicalFolderId`),
  CONSTRAINT `FK_VirtualFolders_PhysicalFolders_PhysicalFolderId` FOREIGN KEY (`PhysicalFolderId`) REFERENCES `physicalfolders` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VirtualFolders_VirtualFolders_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `virtualfolders` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `virtualfolders`
--

LOCK TABLES `virtualfolders` WRITE;
/*!40000 ALTER TABLE `virtualfolders` DISABLE KEYS */;
INSERT INTO `virtualfolders` VALUES ('a4d06132-d76c-49b5-8472-2bf78ac4147e','All FIles',NULL,NULL,0,'79073ec1-51e2-4772-95e6-9b06075a174b','2021-06-01 00:00:00','4b352b37-332a-40c6-ab05-e38fcf109719','2021-06-01 00:00:00','4b352b37-332a-40c6-ab05-e38fcf109719',NULL,NULL,0);
/*!40000 ALTER TABLE `virtualfolders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recentactivities`
--

DROP TABLE IF EXISTS `recentactivities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recentactivities` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FolderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CreatedDate` datetime NOT NULL,
  `Action` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RecentActivities_DocumentId` (`DocumentId`),
  KEY `IX_RecentActivities_FolderId` (`FolderId`),
  KEY `IX_RecentActivities_UserId` (`UserId`),
  CONSTRAINT `FK_RecentActivities_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RecentActivities_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RecentActivities_VirtualFolders_FolderId` FOREIGN KEY (`FolderId`) REFERENCES `virtualfolders` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recentactivities`
--

LOCK TABLES `recentactivities` WRITE;
/*!40000 ALTER TABLE `recentactivities` DISABLE KEYS */;
/*!40000 ALTER TABLE `recentactivities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shareddocumentuser`
--

DROP TABLE IF EXISTS `shareddocumentuser`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shareddocumentuser` (
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`UserId`,`DocumentId`),
  KEY `IX_SharedDocumentUser_DocumentId` (`DocumentId`),
  CONSTRAINT `FK_SharedDocumentUser_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_SharedDocumentUser_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shareddocumentuser`
--

LOCK TABLES `shareddocumentuser` WRITE;
/*!40000 ALTER TABLE `shareddocumentuser` DISABLE KEYS */;
/*!40000 ALTER TABLE `shareddocumentuser` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usernotifications`
--

DROP TABLE IF EXISTS `usernotifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usernotifications` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Action` int NOT NULL,
  `DocumentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `FolderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CreatedDate` datetime NOT NULL,
  `ToUserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FromUserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `IsRead` tinyint(1) NOT NULL,
  `Status` int NOT NULL,
  `ErrorMsg` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_UserNotifications_DocumentId` (`DocumentId`),
  KEY `IX_UserNotifications_FolderId` (`FolderId`),
  CONSTRAINT `FK_UserNotifications_Documents_DocumentId` FOREIGN KEY (`DocumentId`) REFERENCES `documents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserNotifications_VirtualFolders_FolderId` FOREIGN KEY (`FolderId`) REFERENCES `virtualfolders` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usernotifications`
--

LOCK TABLES `usernotifications` WRITE;
/*!40000 ALTER TABLE `usernotifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `usernotifications` ENABLE KEYS */;
UNLOCK TABLES;



--
-- Table structure for table `virtualfolderusers`
--

DROP TABLE IF EXISTS `virtualfolderusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `virtualfolderusers` (
  `FolderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `IsStarred` tinyint(1) NOT NULL,
  `CreatedDate` datetime NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `ModifiedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`FolderId`,`UserId`),
  KEY `IX_VirtualFolderUsers_CreatedBy` (`CreatedBy`),
  KEY `IX_VirtualFolderUsers_DeletedBy` (`DeletedBy`),
  KEY `IX_VirtualFolderUsers_ModifiedBy` (`ModifiedBy`),
  KEY `IX_VirtualFolderUsers_UserId` (`UserId`),
  CONSTRAINT `FK_VirtualFolderUsers_Users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_VirtualFolderUsers_Users_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_VirtualFolderUsers_Users_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_VirtualFolderUsers_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VirtualFolderUsers_VirtualFolders_FolderId` FOREIGN KEY (`FolderId`) REFERENCES `virtualfolders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `virtualfolderusers`
--

LOCK TABLES `virtualfolderusers` WRITE;
/*!40000 ALTER TABLE `virtualfolderusers` DISABLE KEYS */;
/*!40000 ALTER TABLE `virtualfolderusers` ENABLE KEYS */;
UNLOCK TABLES;

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

--
-- Dumping routines for database 'YourDrive'
--
/*!50003 DROP PROCEDURE IF EXISTS `getPhysicalFolderChildsHierarchyById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getPhysicalFolderParentsHierarchyById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getSharedChildsHierarchyById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getSharedParentsHierarchyById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getVirtualFolderChildsHierarchyById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getVirtualFolderParentsHierarchyById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `NLog_AddEntry_p` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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
  END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

