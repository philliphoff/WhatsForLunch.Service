param location string = resourceGroup().location
param coreName string
param externalIngress bool = true
param targetIngressPort int = 80
param imageTag string

@description('CPU cores allocated to a single container instance, e.g. 0.5')
param containerCpuCoreCount string = '0.5'

@description('Memory allocated to a single container instance, e.g. 1Gi')
param containerMemory string = '1.0Gi'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: '${coreName}-log-analytics'
  location: location
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}

resource appEnv 'Microsoft.App/managedEnvironments@2022-10-01' = {
  name: '${coreName}-app-env'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

resource app 'Microsoft.App/containerApps@2022-10-01' = {
  name: '${coreName}-app'
  location: location
  properties: {
    managedEnvironmentId: appEnv.id
    configuration: {
      activeRevisionsMode: 'single'
      ingress: {
        external: externalIngress
        targetPort: targetIngressPort
        transport: 'auto'
      }
    }
    template: {
      containers: [
        {
          image: 'philliphoff/whats-for-lunch-service:${imageTag}'
          name: 'whats-for-lunch-service'
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:${targetIngressPort}'
            }
            {
              name: 'TITAN_MENU_ENDPOINT'
              value: 'https://family.titank12.com/api/'
            }
            {
              name: 'TITAN_MENU_IDENTIFIER'
              value: 'LPX5UV'
            }
          ]
          resources: {
            cpu: json(containerCpuCoreCount)
            memory: containerMemory
          }
        }
      ]
    }
  }
}
