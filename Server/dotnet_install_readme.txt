
Sur Debian Jessie :
___________________________________

$ sudo apt-get update

$ sudo apt-get install curl libunwind8 gettext

$ curl -sSL -o dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz
$ sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet
$ sudo ln -s /opt/dotnet/dotnet /usr/local/bin

$ dotnet --info
$ rm dotnet.tar.gz
