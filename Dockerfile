#基础镜像（用来构建镜像）
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 9000
RUN cp  /usr/share/zoneinfo/Asia/Shanghai /etc/localtime \
         && echo "Asia/Shanghai" > /etc/timezone

#编译（临时镜像，主要用来编译发布项目）
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

#切换到具体源码路径
WORKDIR /src
COPY  src/ /src

#切换到具体的启动项目路径
WORKDIR /src/SchedulerCenter.Host
RUN dotnet publish -c Release -o /app

#构建镜像
FROM base AS final
WORKDIR /app
CMD ["dotnet", "SchedulerCenter.Host.dll"]