# Dorisoy.Pan API Linux 部署手册

本手册介绍如何在 Linux 服务器上部署 Dorisoy.Pan API 服务，涵盖 **Docker 部署** 和 **手动部署** 两种方式。

---

## 一、环境要求

| 组件 | 版本要求 |
|---|---|
| 操作系统 | Ubuntu 22.04+ / CentOS 8+ / Debian 11+ 等主流 Linux 发行版 |
| .NET 10 SDK/Runtime | 10.0 或以上 |
| MySQL | 8.0 或以上（也可使用 SQL Server 2012+） |
| Redis | 可选 —— 未安装时系统自动降级为无锁模式 |
| Docker | 20.10+（仅 Docker 部署方式需要） |
| Docker Compose | v2.0+（可选，推荐） |

---

## 二、方式一：Docker 部署（推荐）

### 2.1 安装 Docker

```bash
# Ubuntu / Debian
sudo apt update
sudo apt install -y docker.io docker-compose-plugin
sudo systemctl enable docker
sudo systemctl start docker

# CentOS / RHEL
sudo yum install -y docker docker-compose-plugin
sudo systemctl enable docker
sudo systemctl start docker
```

验证安装：

```bash
docker --version
docker compose version
```

### 2.2 准备数据库

在 MySQL 服务器上创建数据库并导入初始数据：

```bash
# 登录 MySQL
mysql -u root -p

# 创建数据库
CREATE DATABASE dorisoy_pan CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# 退出后导入 SQL 脚本
mysql -u root -p dorisoy_pan < SQL/MySQL.sql
mysql -u root -p dorisoy_pan < SQL/PROCEDURE-MySQL.sql
```

> 如果使用 SQL Server，请执行 `SQL/MsSQL.sql`。

### 2.3 构建 Docker 镜像

将项目源码上传到 Linux 服务器，进入 `Web` 目录：

```bash
cd /opt/dorisoy-pan/Web
```

使用 Dockerfile 构建镜像：

```bash
docker build -t dorisoy-pan-api:latest -f Dorisoy.Pan.API/Dockerfile .
```

> **注意**：构建上下文为 `Web/` 目录（包含所有子项目），Dockerfile 路径为 `Dorisoy.Pan.API/Dockerfile`。

### 2.4 修改配置文件

在宿主机上创建配置文件目录，并准备 `appsettings.json`：

```bash
sudo mkdir -p /opt/dorisoy-pan/config
```

创建 `/opt/dorisoy-pan/config/appsettings.json`：

```json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "connectionStrings": {
    "DocumentDbConnectionString": "server='你的MySQL地址';Port=3306;user='用户名';password='密码';database=dorisoy_pan;Allow User Variables=True",
    "Redis": "127.0.0.1:6379"
  },
  "JwtSettings": {
    "key": "This*Is&A!Long)Keysdfsdsdsfd(For%Creating@A$SymmetricKey",
    "issuer": "http://你的服务器IP:5000",
    "audience": "PTCUsers",
    "minutesToExpiration": "720"
  },
  "UserProfilePath": "Users",
  "WebApplicationUrl": "http://你的前端地址",
  "DocumentPath": "Documents",
  "DefaultUserImage": "user-profile.jpg",
  "ExecutableFileTypes": ".bat,.bin,.cmd,.com,.cpl,.exe,.gadget,.inf1,.ins,.inx,.isu,.job,.jse,.lnk,.msc,.msi,.msp,.action,.apk,.app,.command,.csh,.ipa,.ksh,.mst,.osx,.out,.paf,.pif,.run",
  "EncryptionKey": "你的加密密钥",
  "ContentRootPath": ""
}
```

**关键配置说明：**

| 配置项 | 说明 |
|---|---|
| `DocumentDbConnectionString` | MySQL 数据库连接字符串，请替换为实际地址、用户名和密码 |
| `Redis` | Redis 连接地址。未安装 Redis 可保留默认值，系统自动降级 |
| `JwtSettings:key` | JWT 签名密钥，**生产环境务必修改为高强度随机字符串** |
| `JwtSettings:issuer` | JWT 签发者地址，改为实际 API 访问地址 |
| `WebApplicationUrl` | 前端 URL，用于 CORS 配置，多个地址用逗号分隔 |
| `EncryptionKey` | 文件 AES 加密密钥，**生产环境务必修改** |

### 2.5 运行容器

```bash
docker run -d \
  --name dorisoy-pan-api \
  --restart always \
  -p 5000:5000 \
  -p 5001:5001 \
  -v /opt/dorisoy-pan/config/appsettings.json:/app/appsettings.json \
  -v /opt/dorisoy-pan/data/Documents:/app/Documents \
  -v /opt/dorisoy-pan/data/Users:/app/Users \
  -v /opt/dorisoy-pan/logs:/app/Logs \
  dorisoy-pan-api:latest
```

**挂载卷说明：**

| 宿主机路径 | 容器路径 | 用途 |
|---|---|---|
| `/opt/dorisoy-pan/config/appsettings.json` | `/app/appsettings.json` | 覆盖应用配置 |
| `/opt/dorisoy-pan/data/Documents` | `/app/Documents` | 上传文件持久化存储 |
| `/opt/dorisoy-pan/data/Users` | `/app/Users` | 用户头像持久化存储 |
| `/opt/dorisoy-pan/logs` | `/app/Logs` | 日志文件持久化 |

### 2.6 使用 Docker Compose（推荐）

创建 `/opt/dorisoy-pan/docker-compose.yml`：

```yaml
version: "3.8"

services:
  mysql:
    image: mysql:8.0
    container_name: dorisoy-pan-mysql
    restart: always
    environment:
      MYSQL_ROOT_PASSWORD: your_root_password
      MYSQL_DATABASE: dorisoy_pan
      MYSQL_USER: dorisoy
      MYSQL_PASSWORD: your_password
    ports:
      - "3306:3306"
    volumes:
      - mysql_data:/var/lib/mysql
      - ./SQL/MySQL.sql:/docker-entrypoint-initdb.d/01-schema.sql
      - ./SQL/PROCEDURE-MySQL.sql:/docker-entrypoint-initdb.d/02-procedures.sql
    command: --character-set-server=utf8mb4 --collation-server=utf8mb4_unicode_ci

  redis:
    image: redis:7-alpine
    container_name: dorisoy-pan-redis
    restart: always
    ports:
      - "6379:6379"

  api:
    build:
      context: ./Web
      dockerfile: Dorisoy.Pan.API/Dockerfile
    image: dorisoy-pan-api:latest
    container_name: dorisoy-pan-api
    restart: always
    ports:
      - "5000:5000"
      - "5001:5001"
    volumes:
      - ./config/appsettings.json:/app/appsettings.json
      - api_documents:/app/Documents
      - api_users:/app/Users
      - api_logs:/app/Logs
    depends_on:
      - mysql
      - redis

volumes:
  mysql_data:
  api_documents:
  api_users:
  api_logs:
```

> **提示**：使用 Docker Compose 时，`appsettings.json` 中的数据库地址应使用服务名 `mysql`，Redis 地址使用 `redis:6379`。

启动所有服务：

```bash
cd /opt/dorisoy-pan
docker compose up -d
```

查看运行状态：

```bash
docker compose ps
docker compose logs -f api
```

---

## 三、方式二：手动部署

### 3.1 安装 .NET 10 Runtime

```bash
# Ubuntu 22.04+
sudo apt update
sudo apt install -y dotnet-sdk-10.0

# 或仅安装运行时（不需要编译时）
sudo apt install -y aspnetcore-runtime-10.0
```

> 其他发行版请参考微软官方文档：https://learn.microsoft.com/dotnet/core/install/linux

验证安装：

```bash
dotnet --version
```

### 3.2 安装 MySQL 8.0

```bash
# Ubuntu
sudo apt install -y mysql-server
sudo systemctl enable mysql
sudo systemctl start mysql

# 安全初始化
sudo mysql_secure_installation
```

创建数据库并导入数据：

```bash
mysql -u root -p
```

```sql
CREATE DATABASE dorisoy_pan CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'dorisoy'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON dorisoy_pan.* TO 'dorisoy'@'%';
FLUSH PRIVILEGES;
EXIT;
```

```bash
mysql -u dorisoy -p dorisoy_pan < SQL/MySQL.sql
mysql -u dorisoy -p dorisoy_pan < SQL/PROCEDURE-MySQL.sql
```

### 3.3 安装 Redis（可选）

```bash
sudo apt install -y redis-server
sudo systemctl enable redis-server
sudo systemctl start redis-server
```

> 不安装 Redis 也可正常运行，系统会自动降级为无锁模式。

### 3.4 发布项目

在开发机器或 Linux 服务器上（需要 .NET 10 SDK）执行：

```bash
cd /path/to/source/Web
dotnet restore Dorisoy.Pan.sln
dotnet publish Dorisoy.Pan.API/Dorisoy.Pan.API.csproj -c Release -o /opt/dorisoy-pan/publish
```

### 3.5 配置应用

编辑 `/opt/dorisoy-pan/publish/appsettings.json`，修改数据库连接字符串和其他配置（参考 2.4 节中的配置说明）。

### 3.6 创建数据目录

```bash
sudo mkdir -p /opt/dorisoy-pan/publish/Documents
sudo mkdir -p /opt/dorisoy-pan/publish/Users
sudo mkdir -p /opt/dorisoy-pan/publish/Logs
```

### 3.7 测试运行

```bash
cd /opt/dorisoy-pan/publish
dotnet Dorisoy.Pan.API.dll
```

访问 `http://服务器IP:5000/swagger` 确认 API 正常启动。

### 3.8 配置 Systemd 服务（守护进程）

创建服务文件 `/etc/systemd/system/dorisoy-pan-api.service`：

```ini
[Unit]
Description=Dorisoy.Pan API Service
After=network.target mysql.service

[Service]
Type=notify
User=www-data
Group=www-data
WorkingDirectory=/opt/dorisoy-pan/publish
ExecStart=/usr/bin/dotnet /opt/dorisoy-pan/publish/Dorisoy.Pan.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dorisoy-pan-api

Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

设置目录权限并启动服务：

```bash
sudo chown -R www-data:www-data /opt/dorisoy-pan/publish
sudo systemctl daemon-reload
sudo systemctl enable dorisoy-pan-api
sudo systemctl start dorisoy-pan-api
```

查看服务状态和日志：

```bash
sudo systemctl status dorisoy-pan-api
sudo journalctl -u dorisoy-pan-api -f
```

---

## 四、Nginx 反向代理配置

建议使用 Nginx 作为反向代理，提供 HTTPS 支持和静态资源服务。

### 4.1 安装 Nginx

```bash
sudo apt install -y nginx
sudo systemctl enable nginx
```

### 4.2 配置站点

创建 `/etc/nginx/sites-available/dorisoy-pan`：

```nginx
server {
    listen 80;
    server_name your-domain.com;

    # 可选：HTTP 重定向到 HTTPS
    # return 301 https://$host$request_uri;

    location / {
        proxy_pass http://127.0.0.1:5000;
        proxy_http_version 1.1;

        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;

        # SignalR WebSocket 支持
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";

        # 大文件上传支持
        client_max_body_size 500M;
        proxy_read_timeout 600s;
        proxy_send_timeout 600s;
    }
}
```

启用站点：

```bash
sudo ln -s /etc/nginx/sites-available/dorisoy-pan /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### 4.3 配置 HTTPS（推荐）

使用 Let's Encrypt 免费证书：

```bash
sudo apt install -y certbot python3-certbot-nginx
sudo certbot --nginx -d your-domain.com
```

证书会自动续期。启用 HTTPS 后，请同步更新 `appsettings.json` 中的 `JwtSettings:issuer` 和 `WebApplicationUrl` 为 `https://` 开头的地址。

---

## 五、常见问题

### 5.1 端口被占用

```bash
# 检查端口占用
sudo ss -tlnp | grep 5000

# 修改监听端口（Systemd 方式）
# 编辑 /etc/systemd/system/dorisoy-pan-api.service
# 修改 ASPNETCORE_URLS=http://0.0.0.0:你的端口
```

### 5.2 数据库连接失败

- 确认 MySQL 允许远程连接：检查 `/etc/mysql/mysql.conf.d/mysqld.cnf` 中 `bind-address = 0.0.0.0`
- 确认防火墙开放了 3306 端口
- 确认数据库用户有远程访问权限

### 5.3 文件上传失败

- 确认 `Documents` 目录有写入权限：`sudo chown -R www-data:www-data /opt/dorisoy-pan/publish/Documents`
- 如使用 Nginx 反向代理，确认 `client_max_body_size` 设置足够大
- 检查磁盘空间是否充足

### 5.4 SignalR 连接失败

- 确认 Nginx 配置了 WebSocket 代理（`Upgrade` 和 `Connection` 头）
- 确认防火墙未拦截 WebSocket 连接

### 5.5 Redis 连接警告

Redis 为可选组件。如果日志中出现 Redis 连接警告但功能正常，可忽略。系统已自动降级为无锁模式。如需启用 Redis 并发控制：

```bash
# 确认 Redis 运行中
redis-cli ping
# 应返回 PONG
```

---

## 六、防火墙配置

```bash
# UFW（Ubuntu）
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 5000/tcp    # 如果不使用 Nginx 反向代理

# firewalld（CentOS）
sudo firewall-cmd --permanent --add-port=80/tcp
sudo firewall-cmd --permanent --add-port=443/tcp
sudo firewall-cmd --reload
```

---

## 七、默认登录信息

| 项目 | 值 |
|---|---|
| Swagger 地址 | `http://服务器IP:5000/swagger` |
| 默认管理员账号 | `admin@gmail.com` |
| 默认密码 | `admin@123` |

> **安全提示**：部署到生产环境后，请立即修改管理员密码、`JwtSettings:key` 和 `EncryptionKey`。
