#FROM microsoft/windowsservercore:1709
#FROM microsoft/windowsservercore:1803
FROM mcr.microsoft.com/windows/servercore:ltsc2019

ENV CONSUL_UI_BETA=1

WORKDIR consul

COPY . .

EXPOSE 8500 8600 8600/udp

ENTRYPOINT consul.exe agent -dev -client 0.0.0.0