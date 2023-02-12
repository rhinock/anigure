FROM mcr.microsoft.com/dotnet/sdk:7.0 AS dotnet-build
WORKDIR /src/anigure
COPY ./anigure/*.csproj ./
WORKDIR /src
COPY *.sln ./
RUN dotnet restore
COPY . ./
RUN dotnet build -c Release -o /app/build

FROM dotnet-build AS dotnet-publish
RUN dotnet publish -c Release -o /app/publish

FROM node AS node-builder
WORKDIR /node
COPY ./anigure/ClientApp/package*.json ./
RUN npm ci
COPY ./anigure/ClientApp ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY anigure.pfx ./
ENV Kestrel__Certificates__Default__Path=/app/anigure.pfx
ENV Kestrel__Certificates__Default__Password="P@ssw0rd"
EXPOSE 80
EXPOSE 443
EXPOSE 442
EXPOSE 5050
COPY --from=dotnet-publish /app/publish .
COPY --from=node-builder /node/build ./wwwroot
ENTRYPOINT ["dotnet", "anigure.dll"]
