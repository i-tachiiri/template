# 🌐 C# + Azure "Service Starter Kit" — Template Repository Specification (AI‑Friendly)

> **Purpose** – Single source of truth for humans *and* AI agents that scaffold, reason over, or validate this template.
> **Style** – Flat Markdown, deterministic headings, short bullets, fenced code blocks.
> **Status** – Includes all review feedback up to 2025‑06‑13.

---

## 1. Overview

* **Goal** – Ship production‑grade services faster by sharing boiler‑plate for auth, data, CI/CD, observability and desktop distribution.
* **Core Stack**

  | Layer          | Choice                                                                           |
  | -------------- | -------------------------------------------------------------------------------- |
  | Platform       | **.NET 8**                                                                       |
  | Auth           | **Azure AD B2C**                                                                 |
  | API            | **Azure Functions** (Isolated Worker)                                            |
  | Web Front‑end  | **Azure Static Web Apps** (Blazor WASM)                                          |
  | Desktop        | **Avalonia UI** (MVVM, CommunityToolkit)                                         |
  | DB             | **Azure SQL Database (Serverless)**                                              |
  | Object Storage | **Azure Blob Storage**                                                           |
  | Secrets        | **Azure Key Vault**                                                              |
  | Observability  | **OpenTelemetry → Azure Monitor** (Managed Collector) + **Application Insights** |
  | CI/CD          | **GitHub Actions**                                                               |
  | Local Dev      | **Docker Compose** + **devcontainer**                                            |

---

## 2. High‑Level Architecture

### 2.1 Cloud

1. **Auth flow** – SWA or Desktop → B2C login → JWT issued.
2. **API call** – JWT → Azure Functions → Token validated → EF Core repo calls Azure SQL.
3. **Asset access** – API / Front‑end fetch dynamic files from **Blob Storage** (via SAS token or Managed Identity).
4. **Observability** – Auto‑instrumented traces/metrics/logs flow through **Managed OTEL Collector** into Azure Monitor; Workbooks visualize.
5. **Secrets** – Functions & SWA resolve settings from Key Vault via Managed Identity.

### 2.2 Local

* `docker‑compose.yml` launches **SQL Server (Linux)** & **Azurite** (Blob emulator).
  Run `docker-compose up -d` to start and `docker-compose down` to stop.
* Projects run in VS Code / VS with hot‑reload, hitting container endpoints.
* **Dev Container** locks .NET SDK, Node, Azure CLI, Bicep versions.
* **Ubuntu package source** stored at `packages-microsoft-prod.deb` for setting up the SDK locally.
* **Dockerfile** in `src/Api/` builds the Functions app for container runs.
  Build image via `docker build -t func-api -f src/Api/Dockerfile .`.
* **Static Web Apps CLI** (`swa start`) serves the Blazor front‑end and proxies
  the Functions API for full‑stack local testing.

---

## 3. Repository Layout

```text
/  (root)
├─ infra/
│   ├─ common/              # Shared modules (VNet, KeyVault, Log, AD B2C, OTEL Collector)
│   ├─ svc-foo/             # Service‑specific IaC (Functions, SWA, SQL, Blob, App Insights)
│   └─ svc-bar/
├─ src/
│   ├─ foo/                 # Web/API/Desktop for foo
│   └─ bar/
…
```

### 3.1 Naming Conventions

* Prefix every cloud resource with `${svcName}-${env}` (eg. `foo-dev-func`).
* Environment diffs reside in `infra/svc-*/<env>.params.json`.
* Global/shared resources live in `infra/common` and are **referenced** (Bicep `existing`) from service modules.

---

## 4. Component Specifications

### 4.1 Layer Responsibilities

| Layer              | Purpose                                                           | Notes                                                                           |
| ------------------ | ----------------------------------------------------------------- | ------------------------------------------------------------------------------- |
| **Domain**         | Aggregate roots, entities, value objects                          | No external references.                                                         |
| **Application**    | MediatR CQRS handlers, business use‑cases                         | Relies on **Domain** & *interfaces* defined in **Infrastructure.Abstractions**. |
| **Infrastructure** | Implementations for external services (SQL, Blob, Key Vault, B2C) | Exposes interfaces consumed by Application; replaceable for tests.              |
| **Contracts**      | DTOs, request / response shapes                                   | Shared across UI & API.                                                         |
| **Shared**         | Cross‑cutting stateless helpers                                   | e.g., DateTime provider, RetryPolicy.                                           |

### 4.2 Infrastructure Project – Adapter Examples

```csharp
public interface IBlobStorageService
{
    Task UploadAsync(string container, string blobName, Stream content, CancellationToken ct = default);
    Task<Uri>  GetReadUriAsync(string container, string blobName, TimeSpan ttl, CancellationToken ct = default);
}

public sealed class AzureBlobStorageService : IBlobStorageService
{
    // wraps Azure.Storage.Blobs SDK – injected via DI
}
```

* Similar adapters: `IAuthorizationService` (B2C/MSAL), `IKeyVaultService`, `ISqlUnitOfWork`.

### 4.3 Api (Azure Functions, Isolated Worker)

| Concern               | Design                                                                             |
| --------------------- | ---------------------------------------------------------------------------------- |
| **Auth**              | Middleware (`Microsoft.Identity.Web`) validates JWT ➜ sets `ClaimsPrincipal`.      |
| **Data**              | Application‐layer MediatR handler calls `ISqlUnitOfWork`.                          |
| **Blob**              | Handlers inject `IBlobStorageService` to generate SAS URIs.                        |
| **Logging & Tracing** | OTEL auto‑instrumentation + custom spans; global exception handler records events. |
| **Endpoints**         | `GET /api/health` • `GET /api/me` • `POST /api/assets` (upload sample).            |

### 4.4 WebApp (Blazor WASM)

* **Auth** – `Microsoft.Authentication.WebAssembly.Msal` w/ B2C.
* **Typed API Client** – `Refit` wrappers over **Contracts**.
* **Blob Download** – Use SAS URI returned by API for direct browser download.
* **SWA Configuration** – `staticwebapp.config.json` routes `/api/*` to
  Functions and falls back to `index.html` for client routing.
* **Local dev** – run `swa start` with `dotnet watch` for hot‑reload during
  development.

### 4.5 DesktopApp (Avalonia UI)

* **Auto‑Update** – `Squirrel.Azure` reads feed JSON hosted in *public* Blob container (or GitHub Release for small scale).
* **Installer hosting** – Option flag to push installers to Blob via CI.

### 4.6 docker‑compose (excerpt)

```yaml
services:
  sqlserver: { ... }
  azurite:
    image: mcr.microsoft.com/azure-storage/azurite
    environment:
      AZURITE_ACCOUNTS: "devstoreaccount1:${AZURITE_KEY}"
```

---

## 5. Infrastructure as Code

* **Blob Storage** – Creates Storage Account + private & public containers.
* **Staging slot** – SWA & Functions get `staging` slot for develop deploys.
* **Scripts** – `infra/scripts/` contains helper for `az storage blob upload-batch`.

---

## 6. CI/CD Pipelines (revised)

| Workflow                  | Trigger          | Environment      | Steps (key)                                                 |
| ------------------------- | ---------------- | ---------------- | ----------------------------------------------------------- |
| **test-and-build.yml**    | PR → any         | –                | Lint → `dotnet test` → Build                                 |
| **deploy-infra.yml**      | Manual / Tag     | `prod`           | Azure login → IaC deploy                                    |
| **deploy-to-staging.yml** | Push → `develop` | `stg` RG & slots | Build Web+API → SWA *staging* slot deploy                   |
| **deploy-to-azure.yml**   | Push → `main`    | `prod`           | Promote SWA slot → production                               |
| **release-desktop.yml**   | Release draft    | –                | Publish → Sign → Notarize → Upload → (optional) Blob upload |

All workflows run automatically via **GitHub Actions**. `test-and-build.yml` ensures
unit tests pass before merging, while the deploy pipelines push to Azure on branch
updates.

---

## 7 Service Scale‑Out Guidelines

* **Modular IaC** – `infra/common` provisions once‑per‑subscription items:
  * VNet + Private DNS
  * Azure AD B2C tenant & app registrations
  * OTEL Managed Collector + Log Analytics Workspace
  * Shared Key Vault (per env)
* **Service Modules** – `infra/svc-*` provisions per‑service items:
  * Azure Functions (Isolated Worker, .NET 8)
  * Azure Static Web Apps + staging slot
  * Azure SQL DB (Serverless tier)
  * Storage Account + containers (private + public)
  * Application Insights instance (linked to OTEL)
* **One Workflow, Many Services** – GitHub Actions `matrix.service` loops through `foo`, `bar`, …
* **Copy‑&‑Go** – To add a service: 1️⃣ copy `src/template` ➜ `src/<new>`; 2️⃣ copy `infra/svc-template` ➜ `infra/svc-<new>`; 3️⃣ add `<new>` to matrix list; 4️⃣ push.

---
## 8. Desktop Distribution Options

| Scale                | Channel                             | Notes                                              |
| -------------------- | ----------------------------------- | -------------------------------------------------- |
| Small / internal     | **GitHub Release**                  | Zero extra cost; rate‑limited.                     |
| Public / high volume | **Blob Storage + Azure Front Door** | Global CDN; feed URL unchanged for Squirrel.Azure. |

CI step:

```yaml
- name: Upload installers to Blob
  run: az storage blob upload-batch -s artifacts/ -d \$web --account-name ${{ secrets.STORAGE_ACC }}
```

---

## 9. Collaboration Guidelines (unchanged)

*See earlier section – branching / commits / docs.*

---

## 10. README Skeleton (excerpt)

Add quick link to spec so AI tools pick it up:

```markdown
> **Spec** → see [/spec/template-spec.md](spec/template-spec.md)
```

---

## 11. FAQ (addition)

* **"Where is the installer feed?"** – `https://<storage>.blob.core.windows.net/public/updates`
* **"Blob 403 locally"** – Run Azurite and set `AZURE_STORAGE_CONNECTION_STRING=UseDevelopmentStorage=true`.

---

> **EOF** – PRs welcome 😊
