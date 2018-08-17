#!/bin/bash
set -ev
dotnet test "RPThreadTrackerV3.BackEnd.Test/RPThreadTrackerV3.BackEnd.Test.csproj" -c Release //p:CollectCoverage=true //p:CoverletOutputFormat=opencover
"$HOME/.nuget/packages/reportgenerator/3.1.2/tools/ReportGenerator.exe" -reports:"RPThreadTrackerV3.BackEnd.Test/coverage.opencover.xml" -targetdir:"Reports" -verbosity:"Info"
