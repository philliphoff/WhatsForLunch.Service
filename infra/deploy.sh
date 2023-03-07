#!/bin/bash

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

# Usage: ./deploy.sh <subscriptionId> <resourceGroupLocation> <coreName> <imageTag>

subscriptionId=$1
resourceGroupLocation=$2
coreName=$3
imageTag=$4

az deployment sub create \
  --subscription $subscriptionId \
  --location $resourceGroupLocation \
  --template-file $SCRIPT_DIR/main.bicep \
  --parameters location=$resourceGroupLocation coreName=$coreName imageTag=$imageTag