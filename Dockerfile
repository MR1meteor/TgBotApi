FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update; apt-get clean
RUN apt-get install -y wget
RUN apt-get install -y gnupg
RUN apt-get update && apt-get -y install libglib2.0-0 libxi6 libnss3
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list
RUN apt-get update && apt-get -y install google-chrome-stable

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "TgBotApi/TgBotApi.csproj"
WORKDIR "/src/TgBotApi"
RUN dotnet build "TgBotApi.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/TgBotApi"
RUN dotnet publish "TgBotApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TgBotApi.dll"]