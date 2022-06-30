#!/usr/bin/env

dotnet publish -c Release -o build/
cp -r media/ build/
cp -r sql/ build/
mv appsettings.Release.json build/appsettings.json
powershell Compress-Archive -Path 'build\*' -DestinationPath 'Server.zip'
