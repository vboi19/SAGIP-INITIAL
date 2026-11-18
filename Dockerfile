# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SAGIP/SAGIP.csproj", "SAGIP/"]
RUN dotnet restore "SAGIP/SAGIP.csproj"
COPY . .
WORKDIR "/src/SAGIP"
RUN dotnet publish "SAGIP.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SAGIP.dll"]
