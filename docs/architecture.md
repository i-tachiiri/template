# Architecture

```
Users ─┐
       ├─> Azure AD B2C
       └─> Azure Static Web Apps ─┐
                                   ├─> Azure Functions (foo)
                                   ├─> Azure Functions (bar)
                                   └─> Storage / SQL
```

Each service has its own Functions app, Static Web App and database while
sharing a single Virtual Network, Key Vault and OpenTelemetry collector.
