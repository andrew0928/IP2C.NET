FROM microsoft/dotnet-framework:4.7.2-runtime-windowsservercore-ltsc2019

WORKDIR   c:/IP2C.Console
COPY . .


ENV CONSUL_URL=http://consul:8500

ENTRYPOINT IP2CTest.Console.exe -consul %CONSUL_URL%