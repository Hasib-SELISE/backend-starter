# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src

RUN mkdir -p /root/.nuget/NuGet
COPY ./config/NuGetPackageSource.Config /root/.nuget/NuGet/NuGet.Config

COPY /src/ ./

COPY ./src/WebHost/*.csproj ./WebHost/
RUN dotnet restore ./WebHost/

#   Copy everything else and build
COPY ./src/WebHost ./WebHost/
RUN ls -lah
RUN dotnet build ./WebHost/


#   publish
RUN dotnet publish ./WebHost/ -o /publish --configuration Release
RUN ls /publish

# Publish Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /publish .
ARG git_branch

ENV ASPNETCORE_ENVIRONMENT=$git_branch

ENTRYPOINT ["dotnet", "WebHost.dll"]