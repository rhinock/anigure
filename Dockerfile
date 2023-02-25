FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

WORKDIR /App
RUN mkdir Anigure

# Copy sln with csproj and restore as distinct layers
COPY *.sln ./
COPY Anigure/*.csproj ./Anigure/
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Build a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 44494
ENTRYPOINT ["dotnet", "Anigure.dll"]
