## 本地开发

Generate a self-signed certificate.
```bash
cd certs
openssl req -x509 -newkey rsa:4096 -keyout evrane-local.key -out evrane-local.crt -nodes -subj "/CN=local.evrane.com" -addext "subjectAltName=DNS:admin.blog.local.evrane.com,DNS:admin.pine-hamster.local.evrane.com,DNS:api.pine-hamster.local.evrane.com,DNS:currency.pine-hamster.local.evrane.com,DNS:identity.local.evrane.com,DNS:report.pine-hamster.local.evrane.com,DNS:yiran.local.evrane.com,DNS:evrane-identity-dev-identity-server"
openssl pkcs12 -export -in evrane-local.crt -inkey evrane-local.key -out evrane-local.pfx -name "Evrane"
# Enter Export Password:password
# Verifying - Enter Export Password:password
```

/etc/hosts 文件
```hosts
# 除了 .local 的部分外，与正式域名设置相同
127.0.0.1 admin.blog.local.evrane.com
127.0.0.1 admin.pine-hamster.local.evrane.com
127.0.0.1 api.pine-hamster.local.evrane.com
127.0.0.1 currency.pine-hamster.local.evrane.com
127.0.0.1 report.pine-hamster.local.evrane.com
127.0.0.1 identity.local.evrane.com
127.0.0.1 yiran.local.evrane.com
# docker-compose
127.0.0.1 identity-server
```