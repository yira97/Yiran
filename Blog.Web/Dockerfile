FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Blog.Web/Blog.Web.csproj", "Blog.Web/"]
RUN dotnet restore "Blog.Web/Blog.Web.csproj"
COPY . .
WORKDIR "/src/Blog.Web"
RUN dotnet build "Blog.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blog.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog.Web.dll"]
