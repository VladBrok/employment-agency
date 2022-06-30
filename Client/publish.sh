#!/usr/bin/env

sed -i 's/isDev = true/isDev = false/' ./scripts/config.js
yes "" | surge
sed -i 's/isDev = false/isDev = true/' ./scripts/config.js
