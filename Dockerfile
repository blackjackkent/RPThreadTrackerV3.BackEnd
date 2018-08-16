FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

COPY RPThreadTrackerV3.BackEnd/RPThreadTrackerV3.BackEnd.csproj ./
RUN dotnet --version

COPY RPThreadTrackerV3.BackEnd/ ./
RUN dotnet publish -c Release -o out -r debian-x64

# Build runtime image
FROM microsoft/dotnet:2.1-runtime
WORKDIR /app
COPY --from=build-env /app/out .
RUN apt-get update
RUN apt-get install libgdiplus -y
RUN apt-get install libc6-dev -y
CMD dotnet RPThreadTrackerV3.BackEnd.dll
