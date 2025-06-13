# Infrastructure

This folder contains Bicep modules for the template.

```
infra/
  common/       # shared modules (VNet, KeyVault, B2C, OTEL)
  svc-template/ # per-service resources
  svc-foo/
  svc-bar/
```

Deploy a service module with:

```bash
az deployment group create -f infra/svc-foo/main.bicep -g <rg> -p prefix=myapp env=dev
```
