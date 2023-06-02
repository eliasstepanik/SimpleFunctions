#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#WORKDIR /src
#COPY ["Functions/Functions.csproj", "Functions/"]
#RUN dotnet restore "Functions/Functions.csproj"
#COPY . .
#WORKDIR "/src/Functions"
#RUN dotnet build "Functions.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "Functions.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Functions.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Functions.csproj", "./"]
RUN dotnet restore "./Functions.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Functions.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "Functions.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Functions.dll"]
