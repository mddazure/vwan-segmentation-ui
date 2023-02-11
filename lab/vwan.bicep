param vwan_name string 
param vwan_location string
param hub1_location string
param hub2_location string
param hub3_location string
param hub4_location string
param hub5_location string

param hub1_vnet_ids array
param hub2_vnet_ids array
param hub3_vnet_ids array
param hub4_vnet_ids array
param hub5_vnet_ids array

param base_ip string 

param count int



resource vwan 'Microsoft.Network/virtualWans@2022-07-01'= {
  name: vwan_name
  location: vwan_location
  properties:{
  }
}

resource hub1 'Microsoft.Network/virtualHubs@2022-07-01' = {
  name: 'hub1'
  location: hub1_location
  properties:{
    addressPrefix: '${base_ip}1.0.0/24'
    sku:'Standard'
    virtualWan:{
      id:vwan.id
    }
  }
}
resource hub1_conns 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections@2022-07-01' = [for i in range(0,count-1): {
  name: 'hub1_con${i}'
  parent:hub1
  properties:{
    remoteVirtualNetwork:{
      id:hub1_vnet_ids[i]
    }
    routingConfiguration:{
      associatedRouteTable:{
        id:resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub1.name,'defaultRouteTable')
      }
      propagatedRouteTables:{
        ids:[{
          id: resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub1.name,'defaultRouteTable')
        }
        ]
      }
    }
  }
}]


resource hub2 'Microsoft.Network/virtualHubs@2022-07-01' = {
  name: 'hub2'
  location: hub2_location
  properties:{
    addressPrefix: '${base_ip}2.0.0/24'
    sku:'Standard'
    virtualWan:{
      id:vwan.id
    }
  }
}
resource hub2_conns 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections@2022-07-01' = [for i in range(0,count-1): {
  name: 'hub2_con${i}'
  parent:hub2
  properties:{
    remoteVirtualNetwork:{
      id:hub2_vnet_ids[i]
    }
    routingConfiguration:{
      associatedRouteTable:{
        id:resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub2.name,'defaultRouteTable')
      }
      propagatedRouteTables:{
        ids:[{
          id: resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub2.name,'defaultRouteTable')
        }
        ]
      }
    }
  }
}]

resource hub3 'Microsoft.Network/virtualHubs@2022-07-01' = {
  name: 'hub3'
  location: hub3_location
  properties:{
    addressPrefix: '${base_ip}3.0.0/24'
    sku:'Standard'
    virtualWan:{
      id:vwan.id
    }
  }
}
resource hub3_conns 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections@2022-07-01' = [for i in range(0,count-1): {
  name: 'hub3_con${i}'
  parent:hub3
  properties:{
    remoteVirtualNetwork:{
      id:hub3_vnet_ids[i]
    }
    routingConfiguration:{
      associatedRouteTable:{
        id:resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub3.name,'defaultRouteTable')
      }
      propagatedRouteTables:{
        ids:[{
          id: resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub3.name,'defaultRouteTable')
        }
        ]
      }
    }
  }
}]

resource hub4 'Microsoft.Network/virtualHubs@2022-07-01' = {
  name: 'hub4'
  location: hub4_location
  properties:{
    addressPrefix: '${base_ip}4.0.0/24'
    sku:'Standard'
    virtualWan:{
      id:vwan.id
    }
  }
}
resource hub4_conns 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections@2022-07-01' = [for i in range(0,count-1): {
  name: 'hub4_con${i}'
  parent:hub4
  properties:{
    remoteVirtualNetwork:{
      id:hub4_vnet_ids[i]
    }
    routingConfiguration:{
      associatedRouteTable:{
        id:resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub4.name,'defaultRouteTable')
      }
      propagatedRouteTables:{
        ids:[{
          id: resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub4.name,'defaultRouteTable')
        }
        ]
      }
    }
  }
}]



resource hub5 'Microsoft.Network/virtualHubs@2022-07-01' = {
  name: 'hub5'
  location: hub5_location
  properties:{
    addressPrefix: '${base_ip}5.0.0/24'
    sku:'Standard'
    virtualWan:{
      id:vwan.id
    }
  }
}
resource hub5_conns 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections@2022-07-01' = [for i in range(0,count-1): {
  name: 'hub5_con${i}'
  parent:hub5
  properties:{
    remoteVirtualNetwork:{
      id:hub5_vnet_ids[i]
    }
    routingConfiguration:{
      associatedRouteTable:{
        id:resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub5.name,'defaultRouteTable')
      }
      propagatedRouteTables:{
        ids:[{
          id: resourceId('Microsoft.Network/virtualHubs/hubRouteTables',hub5.name,'defaultRouteTable')
        }
        ]
      }
    }
  }
}]

