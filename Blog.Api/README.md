# Blog.Api

## 配置

[Entity Framework Core tools reference - .NET Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
dotnet tool install --global dotnet-ef
```

---

[user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=linux)

存储位置：

```txt
~/.microsoft/usersecrets/Yiran-Blog.Api/secrets.json
```

设置步骤：

```bash
dotnet user-secrets set "AwsSettings:AccessKeyId" "123456"
dotnet user-secrets set "AwsSettings:SecretAccessKey" "123456"
```