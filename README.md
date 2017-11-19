# IP2C.NET

Use single C# class to implement IP address to country search based on  http://software77.net/geo-ip/ database.

No database is required.  The CountryFinder class uses in-memory array and binary search algorithm to provide fast search(0.01ms/request). 







----

```text

docker swarm join --token SWMTKN-1-38j61cp6gj679asz71gdsgucs3oneoz74oxuhwqv3g68lrr0b1-cvkl8ilz7h37y3dy93pq9exsd 10.0.0.4:2377



registry access key:
wcshub
FrePdljk=YhZjEgwbuefC2yDJtFAmekG


node.labels.os == windows



powershell Install-Module DockerProvider -Force
powershell Install-Package -Name docker -ProviderName DockerProvider -Update -Force


#https://blog.docker.com/2017/09/docker-windows-server-1709/
Install-Module DockerProvider
Install-Package Docker -ProviderName DockerProvider -RequiredVersion preview -Update -Force




powershell Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled false

docker swarm join --token SWMTKN-1-1wsdfp2ucx723rug61b8f2ki4wcb5ndn8yv11ptajhhr08y9z7-80hlg3sy4hdhs3ijvqzenx3io 10.0.0.9:2377
docker login wcshub.azurecr.io -u wcshub -p FrePdljk=YhZjEgwbuefC2yDJtFAmekG
docker pull wcshub.azurecr.io/ip2c.webapi:latest






docker service create --name=webapi -p 8000:80 --replicas 3 --with-registry-auth -d wcshub.azurecr.io/ip2c.webapi:latest



    docker swarm join --token SWMTKN-1-1lfq1qz55kg4w1tg1tadi146wl7uiyfrzrjb2hvb8iyzrhdsor-cys7240ys3fb1j3ptu9mddglr 10.0.0.4:2377





http://blog.airdesk.com/2017/10/windows-containers-portainer-gui.html

http://blog.airdesk.com/2017/10/docker-swarm-on-windows.html

https://rock-it.pl/tips-for-using-docker-swarm-mode-in-production/


```