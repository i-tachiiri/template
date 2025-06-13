#!/bin/bash
SERVICE=$1
export AZURE_STORAGE_CONNECTION_STRING=UseDevelopmentStorage=true
export ConnectionStrings__Default="Server=sqlserver,1433;Database=${SERVICE};User Id=sa;Password=Your_password123;Encrypt=False"
export SWA_CLI_APP_ARTIFACT_LOCATION=src/${SERVICE}/WebApp/wwwroot
swa start http://localhost:7071 --api-location http://localhost:7071
