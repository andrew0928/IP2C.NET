pushd .
cd IP2C.WebAPI.SelfHost\bin\debug


start /min IP2C.WebAPI.SelfHost.exe -url http://localhost:%RANDOM%/

popd