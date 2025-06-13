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

## Azure Functions を Docker で実行する

```bash
# API コンテナーをビルド
docker build -t funcapp -f src/Api/Dockerfile src/Api

# ローカルで起動
docker run -p 7071:80 funcapp
```

## テスト実行

リポジトリには Solution ファイル `ServiceStarterKit.sln` が含まれています。まず依存パッケージを復元してビルドを行い、その後テストを実行します。

```bash
dotnet restore ServiceStarterKit.sln
dotnet build --no-restore ServiceStarterKit.sln
dotnet test --no-build --verbosity normal ServiceStarterKit.sln
```
