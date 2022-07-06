#!/usr/bin/env

file=./scripts/config.js
found=$(grep 'isDev = true' $file)
found=${found:0:1}

sed -i 's/isDev = true/isDev = false/' $file
yes "" | surge

if [ -n "$found" ]
then
  sed -i 's/isDev = false/isDev = true/' $file
fi
