# AI Developer Guidelines

This repository contains a C#/.NET 8 template for Azure based services. See [spec/template-spec.md](spec/template-spec.md) for detailed architecture and component guidance.

## Local Development
- Use `.devcontainer/devcontainer.json` to spin up a development container. This automatically runs `docker-compose up -d sqlserver azurite otelcollector` after creation.
- Manually start dependencies with `docker-compose up -d` when needed. SQL Server and Azurite (Blob emulator) will be started, and an OpenTelemetry Collector container is included.
- To run the Functions app in Docker:

```bash
docker build -t funcapp -f src/Api/Dockerfile src/Api
docker run -p 7071:80 funcapp
```

## Dev Container
VS Codeの Remote Containers 拡張でリポジトリを開くと `.devcontainer` に基づく環境が自動構築され、初回に `docker-compose` が起動します。

## Quick Start (新しいサービス)
1. `cp -r src/template src/<name>`
2. `cp -r infra/svc-template infra/svc-<name>`
3. `.github/workflows/*` の `matrix.service` に `<name>` を追加
4. `git push` すれば GitHub Actions がデプロイします。

## テスト実行
`ServiceStarterKit.sln` を対象に次のコマンドでテストします。

```bash
dotnet restore ServiceStarterKit.sln
dotnet build --no-restore ServiceStarterKit.sln
dotnet test --no-build --verbosity normal ServiceStarterKit.sln
```
The final command expects the build artifacts from the previous steps.
If you omit `restore` or `build`, tests may fail with `VSTestTask returned false`.

## Coding Style
- `.editorconfig` で4スペースインデント、UTF-8、LF改行を指定しています。
- `bin/` や `obj/` などのビルド生成物は `.gitignore` で除外されています。

## その他
- リポジトリ構成や命名規則は `spec/template-spec.md` のセクション2〜3を参照してください。
- InfrastructureのBicepモジュールは `infra/` 配下にあります。共有モジュールは `infra/common`、サービス固有は `infra/svc-*` に配置されています。
