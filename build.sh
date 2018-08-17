#!/bin/bash
set -ev
# install
dotnet restore
dotnet build -c Release
# test
dotnet test "RPThreadTrackerV3.BackEnd.Test/RPThreadTrackerV3.BackEnd.Test.csproj" -c Release /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
wget "https://codecov.io/bash" -O codecov.sh
chmod a+x ./codecov.sh
./codecov.sh -f "RPThreadTrackerV3.BackEnd.Test/coverage.opencover.xml" 
