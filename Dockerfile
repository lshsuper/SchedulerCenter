
#基础镜像（用来构建镜像）
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 9000
COPY . .
CMD ["dotnet", "SchedulerCenter.Host.dll"]