param prefix string
param location string = resourceGroup().location

resource workspace 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
  name: '${prefix}-log'
  location: location
  properties: {}
}

resource collector 'Microsoft.Insights/dataCollectionEndpoints@2021-09-01-preview' = {
  name: '${prefix}-otel'
  location: location
  properties: {
    networkAcls: {
      publicNetworkAccess: 'Enabled'
    }
  }
}
