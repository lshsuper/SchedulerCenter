
#基础镜像（用来构建镜像）
FROM mcr.microsoft.com/dotnet/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 6000
RUN cp  /usr/share/zoneinfo/Asia/Shanghai /etc/localtime \
         && echo "Asia/Shanghai" > /etc/timezone

#编译（临时镜像，主要用来编译发布项目）
FROM mcr.microsoft.com/dotnet/sdk:3.1-buster-slim AS build
WORKDIR /src
COPY . .
WORKDIR /src/SchedulerCenter
RUN dotnet publish -c Release -o /app

#构建镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ["dotnet", "SchedulerCenter.Host.dll"]