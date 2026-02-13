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

### 功能详细设计

#### 一、用户与权限系统

| 功能 | 说明 |
|---|---|
| 用户注册 / 登录 | JWT 令牌认证，登录后返回 Bearer Token，支持自定义过期时间 |
| 管理员/普通用户角色 | Admin 路由通过 `AdminAuthGuard` 守卫，普通用户通过 `AuthGuard` |
| 用户管理 (CRUD) | 创建、查询、更新、删除用户；分页查询支持 `X-Pagination` 响应头 |
| 个人资料 | 修改个人信息、上传/更换头像、修改密码 |
| 密码重置 | 管理员可重置用户密码 |
| 在线用户检测 | 通过 SignalR Hub（`/userHub`）实时追踪在线用户，管理员可强制踢人下线 |
| 登录审计 | 记录每次登录的 IP、时间、浏览器等信息，支持分页查询 |

#### 二、文件夹管理

| 功能 | 说明 |
|---|---|
| 虚拟文件夹树 | 物理文件夹 + 虚拟文件夹双层结构，每个用户拥有独立的虚拟根目录 |
| 创建子文件夹 | 在任意层级创建子文件夹，自动创建对应物理文件夹 |
| 文件夹重命名 | 就地重命名文件夹 |
| 文件夹删除 / 回收站 | 软删除文件夹移入回收站，支持恢复或永久删除 |
| 清空回收站 | 一键永久删除回收站中所有文件和文件夹 |
| 文件夹移动 | 将文件夹移动到其他目录，含共享层级冲突检测 |
| 文件夹复制 | 深度复制文件夹及其子目录和文档 |
| 文件夹下载 | 打包文件夹为 ZIP 下载，自动解密加密文件 |
| 批量下载 | 同时选中多个文件夹和文件，打包 ZIP 下载 |
| 面包屑导航 | 获取文件夹父级链路，展示完整路径导航 |
| 文件夹排序 | 按名称、创建时间排序（升序/降序） |

#### 三、文档管理

| 功能 | 说明 |
|---|---|
| 分片上传 | 前端计算 MD5 → 分片切割 → 逐片上传 → 后端合并，支持大文件 |
| MD5 秒传 | 上传前校验 MD5，若服务器已存在相同文件则直接秒传引用 |
| 并发上传锁 | 通过 Redis（或降级为内存）管理多用户同时上传同一文件的并发控制 |
| AES 加密存储 | 所有文件存储到磁盘时经过 AES 加密，下载/预览时实时解密 |
| 缩略图生成 | 上传时自动生成图片/文档缩略图 |
| 文档下载 | 支持流式解密下载，兼容 Firefox/Chrome 文件名编码 |
| 文档重命名 | 修改文档显示名称 |
| 文档删除 / 回收站 | 软删除移入回收站，支持恢复或永久删除 |
| 文档移动 | 跨文件夹移动文档，含共享状态检查 |
| 文档复制 | 将文档复制到目标文件夹 |
| 全局搜索 | 按名称搜索文件夹和文档，前端使用 debounce 防抖 |
| 可执行文件过滤 | 禁止上传 `.exe`、`.bat`、`.cmd` 等可执行文件类型 |
| 文件夹上传 | 支持拖拽上传整个文件夹，自动创建子目录结构 |
| 上传进度条 | 浮动面板实时显示每个文件的上传百分比 |

#### 四、文档预览

| 功能 | 说明 |
|---|---|
| 图片预览 | 支持 JPG/PNG/GIF/BMP/SVG 等格式在线预览 |
| PDF 预览 | 内嵌 PDF 阅读器 |
| Office 预览 | 通过 Token 机制调用 Office Viewer 预览 Word/Excel/PPT |
| 文本预览 | 在线查看 TXT/CSV/JSON/XML 等文本文件内容（AES 解密后展示） |
| 视频预览 | 支持 MP4/WebM 等格式在线播放 |
| 音频预览 | 支持 MP3/WAV 等格式在线播放 |
| 无预览提示 | 不支持的文件类型显示"无法预览"界面并提供下载选项 |

#### 五、版本控制

| 功能 | 说明 |
|---|---|
| 文档版本历史 | 每次上传同名文件自动创建新版本，保留历史版本记录 |
| 查看历史版本 | 查看每个版本的上传时间、大小等信息 |
| 恢复历史版本 | 可将文档回退到任意历史版本 |
| 版本下载 | 支持下载指定历史版本的文件 |

#### 六、共享与协作

| 功能 | 说明 |
|---|---|
| 文件夹共享 | 将文件夹共享给指定用户，被共享者可访问和编辑 |
| 文档共享 | 将单个文档共享给指定用户 |
| 移除共享权限 | 文件夹/文档所有者可移除指定用户的访问权限 |
| 共享文件列表 | 独立页面查看所有"被共享给我的"文件 |
| 共享层级检测 | 移动/复制操作时自动检测父子级共享冲突并提示 |
| 外部分享链接 | 生成公开分享链接，可设置：有效期、密码保护、是否允许下载 |
| 链接密码验证 | 访问链接前需输入密码验证 |
| 链接预览 | 未登录用户通过分享链接可在线预览文档 |

#### 七、星标收藏

| 功能 | 说明 |
|---|---|
| 文档星标 | 一键收藏/取消收藏文档 |
| 文件夹星标 | 一键收藏/取消收藏文件夹 |
| 星标文件列表 | 独立页面查看所有已收藏的文件和文件夹 |

#### 八、评论系统

| 功能 | 说明 |
|---|---|
| 文档评论 | 对文档添加评论 |
| 评论列表 | 查看文档的所有评论记录 |

#### 九、通知系统

| 功能 | 说明 |
|---|---|
| 实时通知 | SignalR 推送文件夹变更通知（新增、删除文件等） |
| 通知铃铛 | 顶部导航栏显示未读通知数量和最新 10 条消息 |
| 通知列表 | 独立页面查看所有通知，支持分页 |
| 标记已读 | 点击通知自动标记为已读 |
| 通知跳转 | 点击通知直接跳转到对应文件夹或打开文档预览 |

#### 十、最近活动

| 功能 | 说明 |
|---|---|
| 活动记录 | 自动记录文件的创建（CREATED）和查看（VIEWED）操作 |
| 最近活动列表 | 独立页面查看用户的最近文件活动 |

#### 十一、邮件系统

| 功能 | 说明 |
|---|---|
| SMTP 配置 | 管理员配置邮件发送服务器（主机、端口、SSL、认证信息） |
| 邮件模板 | 管理邮件模板（HTML 模板内容） |
| 发送邮件 | 向指定收件人发送自定义邮件 |
| 发送文档 | 将文档作为附件通过邮件发送 |
| 发送文件夹 | 将文件夹打包后通过邮件发送 |

#### 十二、管理后台（Admin 面板）

| 功能 | 说明 |
|---|---|
| 仪表盘 | 统计总用户数、活跃用户数、非活跃用户数、在线用户列表 |
| 用户管理 | 查看、新建、编辑、删除用户，分页+搜索 |
| 登录审计 | 查看所有用户登录历史记录 |
| 系统日志 (NLog) | 查看系统运行日志，支持按级别、时间、来源筛选 |
| 邮件 SMTP 配置 | 配置邮件发送服务器参数 |
| 邮件模板管理 | 创建和编辑邮件模板 |
| 邮件发送 | 管理员发送系统邮件 |

#### 十三、实时通信

| 功能 | 说明 |
|---|---|
| SignalR Hub | `/userHub` 端点，支持 WebSocket 长连接 |
| 用户上线/下线 | `join` / `logout` 事件自动广播在线状态变化 |
| 文件夹变更通知 | 共享文件夹内有变更时实时推送到相关用户 |
| 强制登出 | 管理员可通过 `forceLogout` 事件强制踢出指定用户 |
| 在线状态持久化 | 在线用户列表缓存到 localStorage，刷新页面后恢复 |

#### 十四、前端 UI 特性

| 功能 | 说明 |
|---|---|
| Angular Material UI | 基于 Material Design 组件库 |
| 列表/网格视图切换 | 文件列表支持列表模式和网格模式切换 |
| 右键上下文菜单 | 右键文件/文件夹弹出操作菜单（下载、共享、移动、复制、删除等） |
| 拖放上传 | 支持拖放文件/文件夹直接上传 |
| 响应式布局 | 适配桌面和移动端，侧边栏可折叠 |
| 懒加载模块 | 各功能模块使用 `loadChildren` 按需加载，优化首屏速度 |
| 左侧文件夹树 | 树形导航展示用户文件夹结构 |
| 全选 / 批量操作 | 勾选多个文件/文件夹后批量下载 |

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
