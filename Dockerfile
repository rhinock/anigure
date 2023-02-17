# 1. Prepare server side build image

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
COPY ./anigure/*.csproj ./anigure/
COPY *.sln ./
RUN dotnet restore

# 2. Build client-side artifacts

FROM node AS clientBuild
WORKDIR /ClientApp
COPY ./anigure/ClientApp/package*.json ./
RUN npm install
COPY ./anigure/ClientApp/ .
RUN npm run build

# 3. Publish final output by merging server-side and client-side artifacts

FROM build-env AS publish
COPY . .
RUN dotnet publish -c Release -o out
COPY --from=clientBuild ./ClientApp/build ./anigure/out/ClientApp/build

# Build runtime image

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY anigure.pfx ./
ENV Kestrel__Certificates__Default__Path=/app/anigure.pfx
ENV Kestrel__Certificates__Default__Password="P@ssw0rd"
COPY --from=publish /app/anigure/out/ .
EXPOSE 80
EXPOSE 443
EXPOSE 442
EXPOSE 44494
ENTRYPOINT ["dotnet", "anigure.dll"]
