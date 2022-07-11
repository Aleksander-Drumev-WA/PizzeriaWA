
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WA.Pizza.Web/WA.Pizza.Web.csproj", "WA.Pizza.Web/"]
COPY ["WA.Pizza.Infrastructure/WA.Pizza.Infrastructure.csproj", "WA.Pizza.Infrastructure/"]
COPY ["WA.Pizza.Core/WA.Pizza.Core.csproj", "WA.Pizza.Core/"]
COPY ["Pizzeria.Tests/Pizzeria.Tests.csproj", "Pizzeria.Tests/"]
RUN dotnet restore "WA.Pizza.Web/WA.Pizza.Web.csproj"
COPY . .
WORKDIR "/src/WA.Pizza.Web"
RUN dotnet build "WA.Pizza.Web.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "WA.Pizza.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WA.Pizza.Web.dll"]
