FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MSRD.Identity/MSRD.Identity.csproj", "MSRD.Identity/"]
RUN dotnet restore "MSRD.Identity/MSRD.Identity.csproj"
COPY . .
WORKDIR "/src/MSRD.Identity"
RUN dotnet build "MSRD.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSRD.Identity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSRD.Identity.dll"]