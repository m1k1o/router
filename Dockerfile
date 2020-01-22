FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Router/*.csproj ./Router/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/Router
RUN dotnet build

FROM build AS publish
WORKDIR /app/Router
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/framework/runtime:4.8 AS runtime
WORKDIR /app
COPY --from=publish /app/Router/out ./
ENTRYPOINT ["Router.exe"]
