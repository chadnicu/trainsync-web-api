#!/usr/bin/env bash

set -e

# Variables (edit as needed)
RESOURCE_GROUP="TrainSyncResourceGroup"
WEBAPP_NAME="trainsyncwebnicu456"

echo "Building and publishing..."
dotnet publish -c Release -o ./publish

echo "Zipping publish output..."
cd publish
zip -r ../publish.zip .
cd ..

echo "Deploying to Azure Web App..."
az webapp deploy \
  --resource-group "$RESOURCE_GROUP" \
  --name "$WEBAPP_NAME" \
  --src-path ./publish.zip

echo "Deployment complete!"