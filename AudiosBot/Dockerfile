FROM mcr.microsoft.com/dotnet/sdk:5.0-focal
WORKDIR /app
EXPOSE 80

ARG build_mode=Release

COPY *.sln ./
COPY AudiosBot.API/*.csproj ./AudiosBot.API/
COPY AudiosBot.Domain/*.csproj ./AudiosBot.Domain/
COPY AudiosBot.Infra/*.csproj ./AudiosBot.Infra/
COPY AudiosBot.Test/*.csproj ./AudiosBot.Test/

RUN dotnet restore AudiosBot.API.sln

COPY . ./

RUN dotnet publish -c $build_mode -o out

CMD ASPNETCORE_URLS=http://*:$PORT dotnet out/AudiosBot.API.dll