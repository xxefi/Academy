
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Academy.Presentation/Academy.Presentation.csproj", "Academy.Presentation/"]
COPY ["Academy.Application/Academy.Application.csproj", "Academy.Application/"]
COPY ["Academy.Infrastructure/Academy.Infrastructure.csproj", "Academy.Infrastructure/"]
COPY ["Academy.Domain/Academy.Domain.csproj", "Academy.Domain/"]
RUN dotnet restore "Academy.Presentation/Academy.Presentation.csproj"

COPY . .
WORKDIR "/src/Academy.Presentation"
RUN dotnet build "Academy.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Academy.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Academy.Presentation.dll"]