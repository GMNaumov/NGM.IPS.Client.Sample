# Этот скрипт компилирует одноименный solution, используя конфигурацию IPS SDK из 
# файла ConfigureIPSSDK.ps1 в папке с файлом solution.

# Внимание!!! Не редактировать!!!
# Содержимое этого файла создается автоматически и может быть изменено в любой момент.


$sdkDir = $env:IPS_SDK_DIR
if (!$sdkDir -or !(Test-Path $sdkDir)) { throw "IPS SDK is not found. Please install the IPS SDK and configure it at first." }
$sdkCommandsDir = Join-Path $sdkDir "Automation\v1\Commands"


# подключаем библиотеки общих функций (dot sourcing)
. (Join-Path $sdkCommandsDir "Lib.ps1")


# определим имя солюшена - оно соответствует имени этого скрипта (без приставки Build и с расширением .sln)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$solutionFileName = (Set-Extension (Split-Path -Leaf $MyInvocation.MyCommand.Definition) ".sln") -replace @("^Build", "")
$solutionPath = Join-Path $scriptDir $solutionFileName


# компиляция солюшена
if (!(Test-Path $solutionPath)) { throw "The solution $solutionPath is not found." }
& (Join-Path $sdkCommandsDir "BuildSolution.ps1") $solutionPath @args
