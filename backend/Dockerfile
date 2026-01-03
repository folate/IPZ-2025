FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ipz-marketplace/ipz-marketplace/ipz-marketplace.csproj", "ipz-marketplace/ipz-marketplace/"]
RUN dotnet restore "ipz-marketplace/ipz-marketplace/ipz-marketplace.csproj"

COPY . .
WORKDIR "/src/ipz-marketplace/ipz-marketplace"
RUN dotnet publish "ipz-marketplace.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ipz-marketplace.dll"]