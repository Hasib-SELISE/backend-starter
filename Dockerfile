# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0  AS build-env
WORKDIR /src
ARG git_branch

RUN mkdir -p /root/.nuget/NuGet
COPY ./config/NuGetPackageSource.Config /root/.nuget/NuGet/NuGet.Config

COPY /src/ ./


ENV ASPNETCORE_ENVIRONMENT=$git_branch

#   Copy only .csproj and restore
COPY ./src/API/*.csproj ./webservice/
RUN dotnet restore ./webservice/


#   Copy everything else and build
COPY ./src/API ./webservice/
RUN dotnet build ./webservice/


#   publish
RUN dotnet publish ./webservice/ -o /publish --configuration Release
RUN ls /publish



# Publish Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
ARG git_branch

WORKDIR /app
COPY --from=build-env /publish .
ENV port=80
ARG git_branch

ENV ASPNETCORE_ENVIRONMENT=$git_branch
ENV ASPNETCORE_URLS=http://+:$port

ENTRYPOINT ["dotnet", "API.dll"]
