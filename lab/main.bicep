param rgName string = 'vwan-seg-lab'

param location_hub1 string = 'westeurope'
param location_hub2 string = 'northeurope'
param location_hub3 string = 'swedencentral'
param location_hub4 string = 'francecentral'
param location_hub5 string = 'uksouth'

var count = 5

targetScope = 'subscription'

resource labRg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location_hub1
}



module hub1_vnets 'vnets.bicep' = {
  name : 'hub1_vnets'
  scope : labRg
  params: {
    location: location_hub1
    count: count
    ipbase: '10.1'
  }
}


module hub2_vnets 'vnets.bicep' = {
  name : 'hub2_vnets'
  scope : labRg
  params: {
    location: location_hub2
    count: count
    ipbase: '10.2'
  }
}


module hub3_vnets 'vnets.bicep' = {
  name : 'hub3_vnets'
  scope : labRg
  params: {
    location: location_hub3
    count: count
    ipbase: '10.3'
  }
}

module hub4_vnets 'vnets.bicep' = {
  name : 'hub4_vnets'
  scope : labRg
  params: {
    location: location_hub4
    count: count
    ipbase: '10.4'
  }
}
module hub5_vnets 'vnets.bicep' = {
  name : 'hub5_vnets'
  scope : labRg

  params: {
    location: location_hub5
    count: count
    ipbase: '10.5'
  }
}
module vwan 'vwan.bicep'= {
  name: 'vwan'
  scope: labRg
  dependsOn:[
    hub1_vnets
    hub2_vnets
    hub3_vnets
    hub4_vnets
    hub5_vnets
  ]
  params: {
    vwan_name: 'vwan'
    vwan_location: location_hub1
    hub1_location:location_hub1
    hub2_location:location_hub2 
    hub3_location:location_hub3
    hub4_location:location_hub4
    hub5_location:location_hub5

    base_ip:'10.'

    count: count

    hub1_vnet_ids: hub1_vnets.outputs.vnetId
    hub2_vnet_ids: hub2_vnets.outputs.vnetId
    hub3_vnet_ids: hub3_vnets.outputs.vnetId
    hub4_vnet_ids: hub4_vnets.outputs.vnetId
    hub5_vnet_ids: hub5_vnets.outputs.vnetId

  }
}
