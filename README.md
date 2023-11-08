
##  Dorisoy.Pan v1.0 ##

Dorisoy.Pan 是基于.net core5的跨平台文档管理系统，使用 MS SQL 2012 / MySql8.0（或更高版本）后端数据库，您可以在Windows、Linux 或Mac上运行它,项目中的所有方法都是异步的,支持令牌基身份验证,项目体系结构遵循著名的软件模式和最佳安全实践。源代码是完全可定制的,热插拔且清晰的体系结构,使开发定制功能和遵循任何业务需求变得容易。
系统使用最新的Microsoft技术，高性能稳定性和安全性。

### 先决条件 ###

.NET 5.0 SDK and VISUAL STUDIO 2019, SQL SERVER, MySQL 8.0 

### 安装步骤 ###

1.使用 visual studio 2019+，打开解决方案文件 "Dorisoy.Pan.sln"。

2.右键单击解决方案资源管理器并还原 nuget 软件包。

3.在"Dorisoy.Pan.API 项目中更改“appsettings”中的数据库连接字符串。

4.从“visual studio菜单-->工具-->nuget软件包管理器-->软件包管理器控制台”打开软件包管理器控制台。

5.在package manager控制台中，选择默认项目为 “Dorisoy.Pan.Domain”。

6.在package manager控制台中运行“Update-Database”命令，创建数据库并插入初始数据。

7.在解决方案资源管理器中，右键单击“Dorisoy.Pan.API" 然后从菜单中单击 `设置为启动项`。

8.按F5键运行项目。


###   项目结构 ### 

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

### 截图 ###

## Desktop 客户端示例

<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop1.png.png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop2.png.png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/desktop3.png.png"/>

## Web客户端示例

<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(1).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(2).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(3).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(4).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(5).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(6).png"/>
<img src="https://github.com/dorisoy/Dorisoy.Pan/blob/main/Screen/s%20(7).png"/>

                        

 
