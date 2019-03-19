#FROM microsoft/dotnet-framework:4.7.2-runtime-windowsservercore-1709
#FROM microsoft/dotnet-framework:4.7.2-runtime-windowsservercore-1803
FROM microsoft/dotnet-framework:4.7.2-runtime-windowsservercore-ltsc2019

WORKDIR c:/selfhost
COPY . .


ENV CONSUL_URL=http://consul:8500
ENV NETWORK=0.0.0.0/0

EXPOSE 80
ENTRYPOINT IP2C.WebAPI.SelfHost.exe -network %NETWORK% -consul %CONSUL_URL%