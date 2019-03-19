#FROM microsoft/windowsservercore
FROM mcr.microsoft.com/windows/servercore:ltsc2019

COPY nginx           /nginx
COPY start-nginx.cmd /
COPY reload.cmd      /

EXPOSE 80

ENTRYPOINT start-nginx.cmd
