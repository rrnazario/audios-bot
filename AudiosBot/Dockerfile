FROM mcr.microsoft.com/dotnet/sdk:5.0-focal
WORKDIR /app
EXPOSE 80

ARG build_mode=Release

#Arguments passed by .yml
ARG TelegramBotToken
ARG AdminChatId
ARG AdminMail
ARG Users

#env vars used on whole process
ENV TelegramBotToken=${TelegramBotToken}
ENV AdminChatId=${AdminChatId}
ENV AdminMail=${AdminMail}
ENV Users=${Users}

COPY *.sln ./
COPY AudiosBot/*.csproj ./AudiosBot/
COPY AudiosBot.Domain/*.csproj ./AudiosBot.Domain/
COPY AudiosBot.Infra/*.csproj ./AudiosBot.Infra/
COPY AudiosBot.Test/*.csproj ./AudiosBot.Test/

RUN dotnet restore AudiosBot.sln

COPY . ./

RUN dotnet publish -c $build_mode -o out

CMD ASPNETCORE_URLS=http://*:$PORT dotnet out/AudiosBot.API.dll