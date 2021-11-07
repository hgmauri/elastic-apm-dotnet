FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Sample.ElasticApm.WebApi/Sample.ElasticApm.WebApi.csproj", "src/Sample.ElasticApm.WebApi/"]
COPY ["src/Sample.ElasticApm.WebApi.Core/Sample.ElasticApm.WebApi.Core.csproj", "src/Sample.ElasticApm.WebApi.Core/"]
COPY ["src/Sample.ElasticApm.Persistence/Sample.ElasticApm.Persistence.csproj", "src/Sample.ElasticApm.Persistence/"]
COPY ["src/Sample.ElasticApm.Domain/Sample.ElasticApm.Domain.csproj", "src/Sample.ElasticApm.Domain/"]
RUN dotnet restore "src/Sample.ElasticApm.WebApi/Sample.ElasticApm.WebApi.csproj"
COPY . .

WORKDIR "/src/src/Sample.ElasticApm.WebApi"
RUN dotnet build "Sample.ElasticApm.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.ElasticApm.WebApi.csproj" -c Release -o /app/publish

ENV TZ=America/Sao_Paulo
ENV LANG pt-BR
ENV LANGUAGE pt-BR
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.ElasticApm.WebApi.dll"]