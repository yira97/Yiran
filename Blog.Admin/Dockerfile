FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Blog.Admin/Blog.Admin.csproj", "Blog.Admin/"]
RUN dotnet restore "Blog.Admin/Blog.Admin.csproj"
COPY . .
WORKDIR "/src/Blog.Admin"
RUN dotnet build "Blog.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blog.Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog.Admin.dll"]
