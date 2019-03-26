: configuration
SET BUILD_VERSION=3.2.2


: init
Tools\nuget.exe restore


: build solutions in release mode
"c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" /p:Configuration=Release /p:DeployOnBuild=true


: build consul docker image
pushd .
cd Tools

docker build -t 91app/demo.ip2c.consul:latest -t 91app/demo.ip2c.consul:%BUILD_VERSION% .
docker push 91app/demo.ip2c.consul:%BUILD_VERSION%
docker push 91app/demo.ip2c.consul:latest

popd


: build webapi docker image
pushd .
:: cd IP2C.WebAPI\obj\Release\Package\PackageTmp
cd IP2C.WebAPI

docker build -t 91app/demo.ip2c.webapi:latest -t 91app/demo.ip2c.webapi:%BUILD_VERSION% .
docker push 91app/demo.ip2c.webapi:%BUILD_VERSION%
docker push 91app/demo.ip2c.webapi:latest

popd




: build webapi (selfhost) docker image
pushd .
cd IP2C.WebAPI.SelfHost\bin\Release

docker build -t 91app/demo.ip2c.webapi.selfhost:latest -t 91app/demo.ip2c.webapi.selfhost:%BUILD_VERSION% .
docker push 91app/demo.ip2c.webapi.selfhost:%BUILD_VERSION%
docker push 91app/demo.ip2c.webapi.selfhost:latest


popd


: build worker docker image
pushd .
cd IP2C.Worker\bin\Release

docker build -t 91app/demo.ip2c.worker:latest -t 91app/demo.ip2c.worker:%BUILD_VERSION% .
docker push 91app/demo.ip2c.worker:%BUILD_VERSION%
docker push 91app/demo.ip2c.worker:latest

popd






: build test console docker image
pushd .
cd IP2CTest.Console\bin\Release
docker build -t 91app/demo.ip2c.console:latest -t 91app/demo.ip2c.console:%BUILD_VERSION% .
docker push 91app/demo.ip2c.console:%BUILD_VERSION%
docker push 91app/demo.ip2c.console:latest
popd


: build reverse proxy
pushd .
cd IP2C.ReverseProxy
docker build -t 91app/demo.ip2c.proxy:latest -t 91app/demo.ip2c.proxy:%BUILD_VERSION% .
docker push 91app/demo.ip2c.proxy:%BUILD_VERSION%
docker push 91app/demo.ip2c.proxy:latest
popd

: build & push nuget package
Tools\nuget.exe pack IP2C.SDK\IP2C.SDK.csproj -Properties Configuration=Release -Version %BUILD_VERSION%


: build complete
