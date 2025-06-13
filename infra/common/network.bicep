param prefix string
param location string = resourceGroup().location

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: '${prefix}-vnet'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
  }
}
