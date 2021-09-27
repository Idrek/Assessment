## Instructions to run/test it
```
docker pull mcr.microsoft.com/dotnet/sdk:3.1

docker run --rm -it -v /home/me/code:/code -v /home/me/datos.zip:/data/datos.zip mcr.microsoft.com/dotnet/sdk:3.1 bash

apt-get update && apt-get install -y unzip nginx

cd /data

cp datos.zip /var/www/html && cd $_ && unzip datos.zip

tar -zxvf archive.tgz

nginx &

cd /code

dotnet test test/AppTest/AppTest.fsproj

dotnet run --project src/App/App.fsproj --dir '/output' --uri 'http://localhost'
```