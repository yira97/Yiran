# Blog.Api

## 配置

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