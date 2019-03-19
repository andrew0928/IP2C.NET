FROM microsoft/dotnet-framework:4.7.2-runtime-windowsservercore-ltsc2019

WORKDIR   c:/IP2C.Worker
COPY . .


VOLUME ["c:/IP2C.Worker/data"]

ENTRYPOINT IP2C.Worker.exe --watch c:\IP2C.Worker\data\ipdb.csv