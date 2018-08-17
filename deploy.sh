#!/bin/bash
set -ev

TAG=$1
DOCKER_USERNAME=$2
DOCKER_PASSWORD=$3

docker build -t blackjacksoftware/rpthreadtrackerv3.backend:$TAG .
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push $DOCKER_USERNAME/rpthreadtrackerv3.backend:$TAG
