#!/bin/bash
# Helper script to upload artifacts to Azure Blob Storage
az storage blob upload-batch -s "$1" -d "$2" --account-name "$AZURE_STORAGE_ACCOUNT"
