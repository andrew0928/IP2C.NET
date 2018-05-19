docker rm -f demo
: docker pull wcshub.azurecr.io/ip2c.webapi.selfhost:demo
docker run -d --name demo wcshub.azurecr.io/ip2c.webapi.selfhost:demo

powershell sleep 10

docker logs -t demo
docker stop demo

powershell sleep 5

docker logs -t demo