: build solutions in release mode
"c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" /p:Configuration=Release /p:DeployOnBuild=true

: build webapi docker image
pushd .
cd IP2C.WebAPI\obj\Release\Package\PackageTmp
docker build -t ip2c/webapi:latest .
popd

: build worker docker image
pushd .
cd IP2C.Worker\bin\Release
docker build -t ip2c/worker:latest .
popd

: build complete
