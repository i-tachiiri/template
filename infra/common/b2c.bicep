param prefix string
param location string = resourceGroup().location

resource tenant 'Microsoft.AzureActiveDirectory/b2cDirectories@2019-01-01-preview' = {
  name: '${prefix}-b2c'
  location: location
  properties: {
    createTenant: true
    displayName: '${prefix}-b2c'
  }
}
