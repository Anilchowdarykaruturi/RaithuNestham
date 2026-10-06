# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["RaithuNestham/RaithuNestham.csproj", "RaithuNestham/"]
RUN dotnet restore "RaithuNestham/RaithuNestham.csproj"

COPY . .
WORKDIR "/src/RaithuNestham"

RUN dotnet publish "RaithuNestham.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "RaithuNestham.dll"]