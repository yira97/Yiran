# Blog.Api

## 生成数据表定义

### 0.准备

确保已经安装 [dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
dotnet ef --help
```

若未安装执行：

```bash
dotnet tool install --global dotnet-ef
```

### 1. 生成class定义

```bash
python3 ./Blog.Api/Scripts/database.py update-scheme --name InitialCreate
```

### 2. 导出 SQL语句

```bash
python3 ./Blog.Api/Scripts/database.py generate-script

cat ./Blog.Api/Migrations/Blog.Api.sql

```

## 本地运行

### 需要保密的配置项
[user-secrets 文档](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=linux)

密钥存储位置：

```txt
~/.microsoft/usersecrets/Yiran-Blog.Api/secrets.json
```

添加AWS密钥

```bash
cd Blog.Api
dotnet user-secrets set "AwsSettings:AccessKeyId" "123456"
dotnet user-secrets set "AwsSettings:SecretAccessKey" "123456"
```

### 无需保密的配置项

```txt
Blog.Api/Properties/launchSettings.json
```

