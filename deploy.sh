#!/bin/bash
set -ev

TAG=$1
DOCKER_USERNAME=$2
DOCKER_PASSWORD=$3

echo "$TAG"
echo "$DOCKER_USERNAME"
echo "$DOCKER_PASSWORD"
docker build -t $DOCKER_USERNAME/rpthreadtrackerv3.backend .
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push $DOCKER_USERNAME/rpthreadtrackerv3.backend:latest
