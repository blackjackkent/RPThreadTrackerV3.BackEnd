#!/bin/bash
set -ev
# install
dotnet restore
dotnet build -c Release
# test
dotnet test "RPThreadTrackerV3.BackEnd.Test/RPThreadTrackerV3.BackEnd.Test.csproj" /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
wget "https://codecov.io/bash" -O codecov.sh
./codecov.sh -f "coverage.opencover.xml"
