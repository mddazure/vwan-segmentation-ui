param location string
param count int
param ipbase string


resource vnet 'Microsoft.Network/virtualNetworks@2022-07-01' =  [for i in range(1,count):{
  name:'vnet-${location}${i}'
  location: location
  properties:{
    addressSpace:{
      addressPrefixes:[
        '${ipbase}.${i}.0/24'
      ]
    }
    subnets:[
      {
        name: 'default'
        properties:{
          addressPrefix:'${ipbase}.${i}.0/25'
        }
      }
    ]
  }
}]

output vnetId array = [for i in range(0, count-1): vnet[i].id]
