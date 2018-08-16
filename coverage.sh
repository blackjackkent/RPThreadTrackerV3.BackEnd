#!/bin/bash
set -ev
"$HOME/.nuget/packages/opencover/4.6.519/tools/OpenCover.Console.exe" \
  -register:user \
  -output:coverage.xml \
  -target:"dotnet.exe" \
  -targetargs:"test RPThreadTrackerV3.BackEnd.Test/RPThreadTrackerV3.BackEnd.Test.csproj -c Release" \
  -filter:"+[RPThreadTrackerV3.BackEnd*]* -[*Test]*" \
  -excludebyattribute:System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute \
  -hideskipped:Attribute \
  -oldstyle
rm -rf reports
mkdir reports
"$HOME/.nuget/packages/reportgenerator/3.1.2/tools/ReportGenerator.exe" -reports:"coverage.xml" -targetdir:"Reports" -verbosity:"Info"
