#FROM microsoft/aspnet:4.7.1-windowsservercore-1709
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019

WORKDIR c:/inetpub/wwwroot/
COPY . .

EXPOSE 80
VOLUME ["c:/inetpub/wwwroot/App_Data"]