# Service Starter Kit Template
開発環境のテンプレートです。

> **Spec** → see [/spec/dev-environment.md](spec/dev-environment.md)

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
