# Service Starter Kit Template
開発環境のテンプレートです。

> **Spec** → see [/spec/template-spec.md](spec/template-spec.md)

## Azure AD B2C 設定

本番およびローカル Docker 開発で使用する Azure AD B2C の設定テンプレートを `env/` 配下に用意しています。

```text
env/
  dev/
    .env              # Docker 用環境変数サンプル
    appsettings.json  # Functions などで利用する構成値
  stg/
    .env              # Staging 用
    appsettings.json
  prod/
    .env
    appsettings.json
```

必要に応じてクライアント ID やシークレットを上書きしてください。Secrets 管理は Key Vault を利用する想定です。
`KeyVault:Url` キーでボールトの URL を指定します。Docker で実行する際は
`KeyVault__Url=https://<your-vault>.vault.azure.net/` の形式で環境変数を渡してください。

## Azure Functions を Docker で実行する

```bash
# API コンテナーをビルド
docker build -t funcapp -f src/Api/Dockerfile src/Api

# ローカルで起動
docker run -p 7071:80 funcapp
```
### ローカル Blob Storage

SQL Server と Azurite (Blob エミュレーター) を起動するには `docker-compose up -d` を実行します。Functions からは `AZURE_STORAGE_CONNECTION_STRING=UseDevelopmentStorage=true` を設定してアクセスします。

### OpenTelemetry Collector

`docker-compose` には Azure Monitor へ転送する OpenTelemetry Collector コンテナー `otelcollector` が含まれます。環境変数 `AZURE_MONITOR_CONNECTION_STRING` を指定すると、トレースとメトリックが Application Insights へ送信されます。
=======
## docker-compose で依存サービスを起動する

SQL Server と Azurite をまとめて起動できます。

```bash
docker-compose up -d
```

## Dev Container を利用する

VS Code の Remote Containers 拡張からこのリポジトリを開くと、`.devcontainer` の設定に基づいた開発環境が自動で構築されます。初回セットアップ時に `docker-compose` で SQL Server や Azurite などの依存サービスが起動します。


## テスト実行

リポジトリには Solution ファイル `ServiceStarterKit.sln` が含まれています。まず依存パッケージを復元してビルドを行い、その後テストを実行します。

```bash
dotnet restore ServiceStarterKit.sln
dotnet build --no-restore ServiceStarterKit.sln
dotnet test --no-build --verbosity normal ServiceStarterKit.sln
```
