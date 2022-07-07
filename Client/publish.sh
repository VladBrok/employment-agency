#!/usr/bin/env

file=./scripts/config.js
found=$(grep 'isDev = true' $file)
found=${found:0:1}

sed -i 's/isDev = true/isDev = false/' $file
npm run build
cd dist/
yes "" | surge
cd -

if [ -n "$found" ]
then
  sed -i 's/isDev = false/isDev = true/' $file
fi
