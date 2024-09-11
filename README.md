## Dorisoy.Pan

Dorisoy.Pan 是基于.net core8 的跨平台文档管理系统，使用 MS SQL 2012 / MySql8.0（或更高版本）后端数据库，您可以在 Windows、Linux 或 Mac 上运行它,项目中的所有方法都是异步的,支持令牌基身份验证,项目体系结构遵循著名的软件模式和最佳安全实践。源代码是完全可定制的,热插拔且清晰的体系结构,使开发定制功能和遵循任何业务需求变得容易。
系统使用最新的 Microsoft 技术，高性能稳定性和安全性。

### 先决条件

.NET 7.0+ SDK and VISUAL STUDIO 2019, SQL SERVER, MySQL 8.0

### 服务端启动步骤

1.使用 visual studio 2019+，打开解决方案文件 "Dorisoy.Pan.sln"。

2.右键单击解决方案资源管理器并还原 nuget 软件包。

3.在"Dorisoy.Pan.API 项目中更改“appsettings”中的数据库连接字符串。

4.从“visual studio 菜单-->工具-->nuget 软件包管理器-->软件包管理器控制台”打开软件包管理器控制台。

5.在 package manager 控制台中，选择默认项目为 “Dorisoy.Pan.Domain”。

6.在 package manager 控制台中运行“Update-Database”命令，创建数据库并插入初始数据。

7.如果你的数据库为MySQL（示例），请附加SQL/下的SQL脚步 PROCEDURE-MySQL.sql 并重建存储过程。

8.在解决方案资源管理器中，右键单击“Dorisoy.Pan.API" 然后从菜单中单击 `设置为启动项`。

9.按 F5 键运行项目。

### 前端启动步骤

如果您还没有安装 nodejs，请下载并全局安装 nodejs: https://nodejs.org ,确保您的 nodejs 版本>= 4.0 且 NPM >= 3 同时全局安装 TypeScript。

全局安装 Angular-CLI 命令："npm install -g @angular/cli"

1.使用 visual code 打开项目目录 "\UI"。

2.新建：终端-> "npm install" 初始化安装依赖。

3.终端-> "npm run start" 启动 Angular Server。

4.当 ** Angular Live Development Server is listening on localhost:4200, open your browser on http://localhost:4200/ ** 监听时在浏览器运行。

要在生产模式下运行本地副本并构建源，请执行 "ng build --prod" ，这将生成应用程序的生产版本，所有 html，css 和 js 代码都被缩小并放入 dist 文件夹。此文件夹可以在发布应用程序时放入生产服务器。

### Demo

演示地址：[http://pan.dorisoy.com/](http://pan.dorisoy.com/)

默认账号：admin@gmail.com  密码：admin@123

### 项目结构

<pre class="prettyprint">

├──Dorisoy.Pan.sln/                     * 解决方案
│   │
│   ├──Dorisoy.Pan.API                  * REST API Controller, Dependancy configuration, Auto mapper profile 
│   │
│   ├──Dorisoy.Pan.MediatR              * Command handler, Query handler, Fluent API validation
│   │
│   ├──Dorisoy.Pan.Repository           * Each entity repository
│   │
│   ├──Dorisoy.Pan.Domain               * Entity framework dbContext 
|   |
│   ├──Dorisoy.Pan.Common               * Generic repository and Unit of work patterns
│   │ 
│   ├──Dorisoy.Pan.Data                 * Entity classes and DTO classes
│   │
│   ├──Dorisoy.Pan.Helper               * Utility classes

</pre>

### 截图

## Desktop 客户端示例

<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop1.png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop2.png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop3.png"/>

## Web 客户端示例

<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(1).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(2).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(3).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(4).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(5).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(6).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(7).png"/>
