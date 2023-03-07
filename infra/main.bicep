targetScope = 'subscription'

param location string
param coreName string
param imageTag string

var resourceToken = toLower(uniqueString(subscription().id, coreName, location))

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${resourceToken}-rg'
  location: location
}

module app './app.bicep' = {
  name: 'app'
  scope: rg
  params: {
    location: location
    coreName: resourceToken
    imageTag: imageTag
  }
}
