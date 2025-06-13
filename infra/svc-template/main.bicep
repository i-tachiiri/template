param prefix string
param env string
param location string = resourceGroup().location

var svcPrefix = '${prefix}-${env}'

resource kv 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: '${prefix}-${env}-kv'
}

resource func 'Microsoft.Web/sites@2022-09-01' = {
  name: '${svcPrefix}-func'
  location: location
  kind: 'functionapp'
  properties: {}
}

resource swa 'Microsoft.Web/staticSites@2022-09-01' = {
  name: '${svcPrefix}-swa'
  location: location
  properties: {}
}

resource sqlServer 'Microsoft.Sql/servers@2022-02-02-preview' = {
  name: '${svcPrefix}-sqlsrv'
  location: location
  properties: {
    administratorLogin: 'sqladmin'
    administratorLoginPassword: 'ChangeM3!'  // placeholder
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2022-02-02-preview' = {
  name: '${sqlServer.name}/appdb'
  location: location
  sku: {
    name: 'GP_S_Gen5'
  }
  properties: {
    autoPauseDelay: 60
    maxSizeBytes: 268435456000
  }
  dependsOn: [ sqlServer ]
}

resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: toLower('${svcPrefix}st')
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {}
}

resource insights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${svcPrefix}-appins'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}
