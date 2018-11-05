#!/bin/bash
set -ev

DOCKER_USERNAME=$1
DOCKER_PASSWORD=$2
TAG=$3
if [ $# -eq 2 ]
  then
    TAG="latest"
fi

docker build -t blackjacksoftware/rpthreadtrackerv3.backend .
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push $DOCKER_USERNAME/rpthreadtrackerv3.backend
docker image tag $DOCKER_USERNAME/rpthreadtrackerv3.backend $DOCKER_USERNAME/rpthreadtrackerv3.backend:$TAG
docker push $DOCKER_USERNAME/rpthreadtrackerv3.backend:$TAG
