# Infrastructure

This folder contains Bicep modules and helper scripts for deploying Azure resources.

## Azure SQL Database (Serverless)

`sql.bicep` provisions a serverless SQL Database with minimal settings.
Deploy with:

```bash
az deployment group create -f infra/sql.bicep -g <resource-group> -p administratorPassword=<password>
```
