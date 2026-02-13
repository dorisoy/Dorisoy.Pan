## Dorisoy.Pan

Dorisoy.Pan 是基于 **.NET 10** 的跨平台文档管理系统，使用 MS SQL 2012 / MySQL 8.0（或更高版本）后端数据库，您可以在 Windows、Linux 或 Mac 上运行它。项目中的所有方法都是异步的，支持 JWT 令牌身份验证，项目体系结构遵循 CQRS + MediatR 模式和最佳安全实践。源代码完全可定制，热插拔且清晰的体系结构，使开发定制功能和遵循任何业务需求变得容易。

商用版本下载：[http://shop.unitos.cn/item/6](http://shop.unitos.cn/item/6)

---

### 技术栈

| 分类 | 技术 / 版本 |
|---|---|
| 运行时框架 | .NET 10 (`net10.0`) |
| ORM | Entity Framework Core 10.0.3 |
| MySQL 驱动 | MySql.EntityFrameworkCore 10.0.1（Oracle 官方驱动） |
| SQL Server | Microsoft.EntityFrameworkCore.SqlServer 10.0.3 |
| 身份认证 | ASP.NET Core Identity + JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer 10.0.3`) |
| CQRS / 中介者 | MediatR 14.0.0 |
| 对象映射 | AutoMapper 16.0.0 |
| 参数验证 | FluentValidation 12.1.1 |
| API 文档 | Swashbuckle.AspNetCore 10.1.2 |
| 缓存 | NewLife.Redis 6.4.x（可选，不安装 Redis 时自动降级） |
| 日志 | NLog 6.1.0 |
| JSON | Newtonsoft.Json 13.0.4 |
| 图片处理 | SixLabors.ImageSharp 3.1.12 |

---

### 先决条件

- **.NET 10.0 SDK**（Preview / RC）及以上
- **Visual Studio 2022 17.14+**（或 VS Code + C# Dev Kit）
- **MySQL 8.0+** 或 **SQL Server 2012+**
- **Redis**（可选）—— 用于文件上传并发锁控制，未安装时系统自动降级为无锁模式
- **Node.js >= 14** 及 **npm >= 6**（仅前端 Angular UI 需要）

---

### 服务端启动步骤（Web 目录）

1. 使用 Visual Studio 2022 或命令行打开解决方案 `Web/Dorisoy.Pan.sln`。

2. 还原 NuGet 包：
   ```bash
   dotnet restore Web/Dorisoy.Pan.sln
   ```

3. 修改 `Web/Dorisoy.Pan.API/appsettings.json` 中的数据库连接字符串：
   ```json
   "connectionStrings": {
     "DocumentDbConnectionString": "server='你的MySQL地址';Port=3306;user='用户名';password='密码';database=dorisoy_pan;Allow User Variables=True",
     "Redis": "127.0.0.1:6379"
   }
   ```
   > **注意**：如果本地未安装 Redis，保留默认配置即可。系统会自动降级，上传等功能不受影响。

4. 初始化数据库（任选一种方式）：

   **方式一：使用 EF Core Migration**
   ```bash
   # 在 Package Manager Console 中，默认项目选择 Dorisoy.Pan.Domain
   Update-Database
   ```

   **方式二：直接执行 SQL 脚本**
   - MySQL：执行 `SQL/MySQL.sql`
   - SQL Server：执行 `SQL/MsSQL.sql`
   - MySQL 存储过程：执行 `SQL/PROCEDURE-MySQL.sql`

5. 启动项目：
   ```bash
   dotnet run --project Web/Dorisoy.Pan.API
   ```
   或在 Visual Studio 中将 `Dorisoy.Pan.API` 设为启动项目，按 F5 运行。

6. 打开浏览器访问 Swagger：`http://localhost:5000/swagger`

---

### 前端启动步骤（UI 目录）

1. 全局安装 Angular CLI：
   ```bash
   npm install -g @angular/cli
   ```

2. 安装依赖：
   ```bash
   cd UI
   npm install
   ```

3. 启动开发服务器：
   ```bash
   npm run start
   ```

4. 浏览器访问：`http://localhost:4200`

5. 生产构建：
   ```bash
   ng build --configuration production
   ```
   构建产物位于 `UI/dist/` 目录。

---

### Demo

演示地址：[http://pan.doriso.cn/](http://pan.doriso.cn/)

默认账号：`admin@gmail.com`　密码：`admin@123`

---

### 项目结构

```
Web/
├── Dorisoy.Pan.API            # REST API 控制器、DI 配置、Swagger、启动入口
├── Dorisoy.Pan.MediatR        # CQRS Command/Query Handler、FluentValidation 验证
├── Dorisoy.Pan.Repository     # 实体仓储层（AutoMapper Profile）
├── Dorisoy.Pan.Domain         # EF Core DbContext、数据库迁移
├── Dorisoy.Pan.Common         # 通用仓储、UnitOfWork、查询扩展方法
├── Dorisoy.Pan.Data           # 实体类（Entity）、DTO 类
└── Dorisoy.Pan.Helper         # 工具类（加密、缩略图、路径等）

Client/                        # Avalonia 跨平台桌面/移动客户端
├── Dorisoy.Pan                # 主 UI 项目（Avalonia）
├── Dorisoy.Pan.Desktop        # 桌面端入口
├── Dorisoy.Pan.Android        # Android 端入口
├── Dorisoy.Pan.iOS            # iOS 端入口
└── Dorisoy.Pan.Web            # WebAssembly 端入口

UI/                            # Angular Web 前端
SQL/                           # 数据库初始化脚本
Screen/                        # 截图资源
```

---

### .NET 10 迁移说明

本项目已从 .NET 8 完整迁移至 .NET 10。以下是迁移过程中的关键变更和注意事项：

#### 1. 目标框架升级
所有 Web 项目的 `TargetFramework` 已从 `net8.0` 更新为 `net10.0`。

#### 2. MySQL 驱动更换
| 变更前 | 变更后 |
|---|---|
| Pomelo.EntityFrameworkCore.MySql 9.0.0 | **MySql.EntityFrameworkCore 10.0.1**（Oracle 官方） |

**原因**：Pomelo 9.0.0 不兼容 EF Core 10，Oracle 官方驱动已支持 .NET 10。

**影响**：
- `MySqlValueGenerationStrategy` 枚举值大小写变更（如 `IdentityColumn` → `IdentityColumn`）
- 命名空间从 `MySqlConnector` 变更为 `MySql.Data.MySqlClient`
- `MySqlParameter` 引用需更新

#### 3. EF Core 10 Contains 查询破坏性变更
EF Core 10 对本地集合的 `Contains()` SQL 翻译方式发生了重大变化，官方建议使用 `EF.Constant()` 包裹。但 **MySql.EntityFrameworkCore 不支持 `EF.Constant()` + `Contains()` 组合**，会抛出：
```
System.InvalidOperationException: Expression '@xxx' in the SQL tree does not have a type mapping assigned.
```

**解决方案**：项目中创建了 `WhereContains` 扩展方法（位于 `Dorisoy.Pan.Common/QueryableContainsExtensions.cs`），用 Expression Tree 动态构建 OR 谓词替代 IN 子句：
```csharp
// 修改前（EF Core 10 + MySQL 会报错）：
.Where(c => allUserIds.Contains(c.Id))

// 修改后：
.WhereContains(c => c.Id, allUserIds)

// 生成 SQL: WHERE Id = @p0 OR Id = @p1 OR Id = @p2 ...
```

> **注意**：仅 EF Core IQueryable 查询需要使用 `WhereContains`，内存中的 LINQ-to-Objects 操作继续使用普通 `.Contains()`。

#### 4. AutoMapper 16.0 构造函数变更
AutoMapper 16.0 移除了接受 `Action<IMapperConfigurationExpression>` 的构造函数，需改用 `MapperConfiguration` 方式创建。

#### 5. Swashbuckle / OpenApi 2.x 破坏性变更
Swashbuckle 10.1.2 依赖 Microsoft.OpenApi 2.x，API 有重大变化：
- `OpenApiSecurityScheme` 构造方式变更
- `AddSecurityRequirement` 签名变更，需使用 lambda 表达式 + `OpenApiSecuritySchemeReference`

#### 6. 数据库自动补列
实体新增了 `Md5` 字段但历史数据库可能没有此列，系统在 `Startup.cs` 中自动检测并执行 `ALTER TABLE` 添加缺失列。

#### 7. Redis 可选降级
文件上传模块使用 Redis 做并发锁控制。当 Redis 不可用时（未安装或连接失败），系统自动降级为无锁模式，不影响正常的文件上传功能。仅在多用户并发上传同一文件时可能丢失锁控制。

---

### 截图

#### Desktop 客户端

<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop1.png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop2.png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop3.png"/>

#### Web 客户端

<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(1).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(2).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(3).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(4).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(5).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(6).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(7).png"/>

---

### 更新日志

#### v2.0.0（2026-02）

- 目标框架从 .NET 9 升级至 **.NET 10**（`net10.0`）
- 所有 NuGet 依赖升级至最新版本
- MySQL 驱动从 Pomelo.EntityFrameworkCore.MySql 9.0.0 更换为 **MySql.EntityFrameworkCore 10.0.1**（Oracle 官方驱动）
- 新增 `WhereContains` 扩展方法，解决 EF Core 10 + MySql.EntityFrameworkCore 的 `Contains()` 类型映射异常
- 修复 AutoMapper 16.0 构造函数破坏性变更
- 修复 Swashbuckle 10.1.2 + Microsoft.OpenApi 2.x 破坏性变更
- 新增数据库自动补列机制（启动时检测并添加缺失的 `Md5` 列）
- Redis 改为可选依赖，不可用时自动降级为无锁模式
- `appsettings.json` 新增 `ConnectionStrings:Redis` 配置项
- EF Core Migration 适配 Oracle MySQL 驱动（`MySQLValueGenerationStrategy` 枚举、命名空间等）
- 修复 `MySqlConnector` → `MySql.Data.MySqlClient` 命名空间迁移

#### v1.3.0（2025-10）

- 目标框架升级至 **.NET 9**
- 同步升级 EF Core 9、ASP.NET Core 9 等依赖

#### v1.2.0（2025）

- 目标框架升级至 **.NET 8**（LTS）
- 同步升级 EF Core 8、ASP.NET Core 8 等依赖

#### v1.1.0（2024）

- 目标框架升级至 **.NET 7**
- 同步升级 EF Core 7、ASP.NET Core 7 等依赖

#### v1.0.0（2024）

- 首个开源版本发布
- 基于 **.NET 6**（LTS）构建
- 支持 MySQL 8.0 / MS SQL Server 2012+
- 使用 Pomelo.EntityFrameworkCore.MySql 作为 MySQL 驱动
- Angular 前端 + ASP.NET Core Web API 后端
- Avalonia 跨平台桌面客户端（Windows / macOS / Linux）
- JWT 令牌身份验证
- CQRS + MediatR 架构模式
- 文件加密存储、缩略图生成、文件夹共享、星标收藏等核心功能

---

### License

[MIT](LICENSE)
