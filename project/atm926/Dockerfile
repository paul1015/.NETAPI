FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY atm/*.csproj ./atm/
RUN dotnet restore

# copy everything else and build app
COPY atm/. ./atm/
WORKDIR /app/atm
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS runtime
WORKDIR /app
COPY --from=build /app/atm/out ./
ENTRYPOINT ["dotnet", "atm.dll"]
