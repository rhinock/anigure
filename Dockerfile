FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY ./anigure/bin/Debug/net7.0/publish /app

ENTRYPOINT ["dotnet", "anigure.dll"]
