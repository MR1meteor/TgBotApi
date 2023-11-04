FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt update && apt install -y  \
 apt-transport-https \
 ca-certificates \
 curl \
 gnupg \
 hicolor-icon-theme \
 libcanberra-gtk* \
 libgl1-mesa-dri \
 libgl1-mesa-glx \
 libpango1.0-0 \
 libpulse0 \
 libv4l-0 \
 fonts-symbola \
 --no-install-recommends \
&& curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
&& echo "deb [arch=amd64] https://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list \
&& apt-get update && apt-get install -y \
google-chrome-stable \
--no-install-recommends \
&& apt-get purge --auto-remove -y curl \
&& rm -rf /var/lib/apt/lists/*

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