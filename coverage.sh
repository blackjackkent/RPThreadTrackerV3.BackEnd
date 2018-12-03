#!/bin/bash
set -ev
dotnet test "RPThreadTrackerV3.BackEnd.Test/RPThreadTrackerV3.BackEnd.Test.csproj" -c Release //p:CollectCoverage=true //p:CoverletOutputFormat=opencover
"$HOME/.nuget/packages/reportgenerator/4.0.4/tools/net47/ReportGenerator.exe" -reports:"RPThreadTrackerV3.BackEnd.Test/coverage.opencover.xml" -targetdir:"Reports" -verbosity:"Info"
