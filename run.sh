#!/bin/bash
set -ev
SCRIPTPATH="$( cd "$(dirname "$0")" ; pwd -P )"
docker run --rm -it -p 29564:80 --name dev --mount type=bind,source=$SCRIPTPATH,target=/app microsoft/dotnet:2.1-sdk
