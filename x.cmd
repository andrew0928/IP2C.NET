pushd .
cd tools
set CONSUL_UI_BETA=true
start /min consul.exe agent --dev
:: start /min consul.exe agent --dev --config-file consul-conf.json
cd ..
popd




pause

pushd .
cd IP2C.WebAPI.SelfHost\bin\debug


start /min IP2C.WebAPI.SelfHost.exe -url http://localhost:%RANDOM%/
start /min IP2C.WebAPI.SelfHost.exe -url http://localhost:%RANDOM%/
start /min IP2C.WebAPI.SelfHost.exe -url http://localhost:%RANDOM%/
start /min IP2C.WebAPI.SelfHost.exe -url http://localhost:%RANDOM%/
start /min IP2C.WebAPI.SelfHost.exe -url http://localhost:%RANDOM%/


popd

powershell sleep 5
pushd .
cd IP2CTest.Console\bin\debug
IP2CTest.Console.exe
popd