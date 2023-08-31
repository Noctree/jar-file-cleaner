@echo off
dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -p:PublishReadyToRunShowWarnings=true -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained
dotnet publish -c Release -r linux-x64 -p:PublishReadyToRun=true -p:PublishReadyToRunShowWarnings=true -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained
echo Done!