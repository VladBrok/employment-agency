#!/usr/bin/env

dotnet publish -c Release -o bin/build/
cp -r media/ bin/build/
cp -r sql/ bin/build/
rm -rf Server.zip
powershell Compress-Archive -Path 'bin\build\*' -DestinationPath 'Server.zip' -Force
