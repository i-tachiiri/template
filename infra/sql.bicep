param location string = resourceGroup().location
param administratorLogin string = 'sqladmin'
@secure()
param administratorPassword string
param sqlDbName string = 'appdb'

resource sqlServer 'Microsoft.Sql/servers@2022-02-01-preview' = {
  name: '${sqlDbName}srv'
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-02-01-preview' = {
  parent: sqlServer
  name: sqlDbName
  sku: {
    name: 'GP_S_Gen5_1'
    tier: 'GeneralPurpose'
  }
  properties: {
    autoPauseDelay: 60
    minCapacity: 0.5
    requestedServiceObjectiveName: 'GP_S_Gen5_1'
  }
}

output connectionString string = 'Server=tcp:${sqlServer.name}.database.windows.net,1433;Initial Catalog=${sqlDbName};User ID=${administratorLogin};Password=${administratorPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
