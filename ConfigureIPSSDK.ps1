#Версия IPS
$env:IPS_VERSION = 8

#Путь к серверной части IPS. Если серверная часть установлена где-то в дебрях сети - НАСТОЯТЕЛЬНО рекомендую примапить путь как сетевой диск
$env:IPS_SERVER_DIR = Resolve-Path "C:\Program Files\IPS\Server"

#Путь к клиентской части IPS
$env:IPS_CLIENT_DIR = Resolve-Path "C:\Program Files\IPS\Client"

#Путь к папке с установленной Visual Studio 2022
$env:DEVENV_DIR = Resolve-Path "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE"
