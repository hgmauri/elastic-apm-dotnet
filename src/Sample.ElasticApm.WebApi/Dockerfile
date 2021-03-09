FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Sample.ElasticApm.WebApi/Sample.ElasticApm.WebApi.csproj", "Sample.ElasticApm.WebApi/"]
COPY ["Sample.ElasticApm.WebApi.Core/Sample.ElasticApm.WebApi.Core.csproj", "Sample.ElasticApm.WebApi.Core/"]
COPY ["Sample.ElasticApm.Domain/Sample.ElasticApm.Domain.csproj", "Sample.ElasticApm.Domain/"]
RUN dotnet restore "Sample.ElasticApm.WebApi/Sample.ElasticApm.WebApi.csproj"
COPY . .
WORKDIR "/src/Sample.ElasticApm.WebApi"
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