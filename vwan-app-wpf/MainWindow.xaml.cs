using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Azure.Identity;
using Azure.ResourceManager;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources;
using System.Collections.ObjectModel;
using Azure.ResourceManager.Network;
using Azure.Core;
using Azure.ResourceManager.Network.Models;
using System.Windows.Ink;
using Azure.ResourceManager.Resources.Models;

namespace vwan_app_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class SegmentationDomain
    {
        public string Name { get; set; }

        private ObservableCollection<HubVirtualNetworkConnectionResource> members;
        private HubVirtualNetworkConnectionResource member;
        public SegmentationDomain(string name)
        {
            Name = name;
            members = new ObservableCollection<HubVirtualNetworkConnectionResource>();
        }        

        public ObservableCollection<HubVirtualNetworkConnectionResource> Members
        {
            get { return members; }

            set => members.Add(member);
        }
    }

    public class LinkGroup
    { 
        public string Name { get; set; }
        public string GroupName { get; set; }

        private ObservableCollection<SegmentationDomain> members;
        private SegmentationDomain member;

        public LinkGroup(string name)
        {
            Name = name;
            members = new ObservableCollection<SegmentationDomain>();
        }
        public ObservableCollection<SegmentationDomain> Members
        {
            get { return members; }
            set => members.Add(member);
        }
    }

    public partial class MainWindow : Window
    {
        ArmClient armClient; //https://learn.microsoft.com/en-us/dotnet/api/azure.resourcemanager.armclient?view=azure-dotnet

        SubscriptionResource defaultsubscription;
        SubscriptionResource subscriptionResource;

        SubscriptionCollection subscriptionCollection;
        ObservableCollection<SubscriptionResource> subscriptionResources;

        ResourceGroupCollection rgCollection;
        ResourceGroupResource resourceGroup;
        ObservableCollection<ResourceGroupResource> resourceGroupResources;

        VirtualWanCollection virtualWanCollection;  //https://learn.microsoft.com/en-us/dotnet/api/azure.resourcemanager.network.virtualwancollection?view=azure-dotnet-preview
        ObservableCollection<VirtualWanResource> virtualWanResources;
        VirtualWanResource virtualWan;

        VirtualHubCollection virtualHubCollection;
        ObservableCollection<VirtualHubResource> virtualHubSelectedResources; //https://learn.microsoft.com/en-us/dotnet/api/azure.resourcemanager.network.virtualhubcollection?view=azure-dotnet-preview
        ObservableCollection<VirtualHubResource> virtualHubResources;

        HubVirtualNetworkConnectionCollection hubVirtualNetworkConnectionCollection;
        //ObservableCollection<ObservableCollection<HubVirtualNetworkConnectionResource>> hubVirtualNetworkConnectionResources; //https://learn.microsoft.com/en-us/dotnet/api/azure.resourcemanager.network.hubvirtualnetworkconnectionresource?view=azure-dotnet-preview
        // there is a HubVirtualNetworkConnectionCollection per Hub, so need nested ObservableCollection as collection of all spoke connections in VWAN
        ObservableCollection<HubVirtualNetworkConnectionResource> hubVirtualNetworkConnectionResources;

        ObservableCollection<SegmentationDomain> segmentationDomains;
        ObservableCollection<LinkGroup> linkGroups; // collection of all Link Groups
        ObservableCollection<LinkGroup> linkedGroups; //collection of connected Link Groups

        HubRouteTableResource hubRouteTable;

        RoutingConfiguration routingConfiguration;

        public MainWindow()
        {
            InitializeComponent();
            segmentationDomains = new ObservableCollection<SegmentationDomain>();
            linkedGroups = new ObservableCollection<LinkGroup>();


        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //armClient = new ArmClient(new DefaultAzureCredential(true));
            armClient = new ArmClient(new InteractiveBrowserCredential());    //https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication-additional-methods?view=azure-dotnet#interactive-browser-authentication
            subscriptionCollection = armClient.GetSubscriptions();      //https://learn.microsoft.com/en-us/dotnet/api/azure.resourcemanager.armclient.getsubscriptions?view=azure-dotnet#azure-resourcemanager-armclient-getsubscriptions
            defaultsubscription = armClient.GetDefaultSubscription();
            subscriptionResources = new ObservableCollection<SubscriptionResource>();
            subscriptionResources.Clear();
            foreach (SubscriptionResource subscriptionResource in subscriptionCollection) //https://learn.microsoft.com/en-us/dotnet/api/azure.resourcemanager.resources.subscriptionresource?view=azure-dotnet
            {
                if (subscriptionResource.HasData)
                {
                    this.subscriptionResources.Add(subscriptionResource);
                }
            }
            SubscriptionsBox.ItemsSource = subscriptionResources;      //https://learn.microsoft.com/en-us/dotnet/api/azure.resourcemanager.resources.subscriptioncollection?view=azure-dotnet
            SubscriptionsBox.DisplayMemberPath = "Data.DisplayName";
            SubscriptionsBox.SelectedValuePath = "Data.SubscriptionId";
        }

        private void SubscriptionsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndexSubs = SubscriptionsBox.SelectedIndex;
            if (selectedIndexSubs < 0) { return; };
            subscriptionResource = subscriptionResources[selectedIndexSubs];
            rgCollection = subscriptionResource.GetResourceGroups();
            resourceGroupResources = new ObservableCollection<ResourceGroupResource>();
            foreach (ResourceGroupResource resourceGroup in rgCollection)
            {
                this.resourceGroupResources.Add(resourceGroup);
            }
            ResourcegroupsBox.ItemsSource = resourceGroupResources;
            ResourcegroupsBox.DisplayMemberPath = "Data.Name";
            ResourcegroupsBox.SelectedValuePath = "Data.Id";
        }


        private void ResourcegroupsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VirtualWANsBox.SelectedIndex = 0;

            int selectedIndexRGs = ResourcegroupsBox.SelectedIndex;
            if (selectedIndexRGs < 0) { return; };
            resourceGroup = resourceGroupResources[selectedIndexRGs];
            virtualWanCollection = resourceGroup.GetVirtualWans();
            virtualWanResources = new ObservableCollection<VirtualWanResource>();
            foreach (VirtualWanResource virtualWan in virtualWanCollection)
            {
                if (virtualWan.HasData)
                {
                    this.virtualWanResources.Add(virtualWan);
                }
            }
            VirtualWANsBox.ItemsSource = virtualWanResources;
            VirtualWANsBox.DisplayMemberPath = "Data.Name"; //https://learn.microsoft.com/en-us/dotnet/api/azure.resourcemanager.network.virtualwandata?view=azure-dotnet-preview NB: Name property not documented
        }

        private void VirtualWANsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = VirtualWANsBox.SelectedIndex;
            if (selectedIndex < 0) { return; };
            virtualWan = virtualWanResources[selectedIndex];
            virtualHubCollection = resourceGroup.GetVirtualHubs(); //VirtualHubs are organized under ResourceGroup, not under VWAN, so hub of multiple VWANs in same RG are in same collection
            virtualHubResources = new ObservableCollection<VirtualHubResource>();
            VirtualHubsBox.ItemsSource = virtualHubResources;
            VirtualHubsBox.DisplayMemberPath = "Data.Name";
            foreach (VirtualHubResource virtualHubResource in virtualHubCollection)
            {
                if (virtualHubResource.HasData)
                {
                    if (virtualHubResource.Data.VirtualWanId == virtualWan.Data.Id)
                    {
                        this.virtualHubResources.Add(virtualHubResource);
                    }
                }
            }
            VirtualHubsBox.ItemsSource = virtualHubResources;
            VirtualHubsBox.DisplayMemberPath = "Data.Name";
        }

        private void VirtualHubsBox_SelectionChanged(object sender, RoutedEventArgs e)
        {



            if (VirtualHubsBox.SelectedItem != null)
            {
                VirtualHubResource virtualHubResource = (VirtualHubResource)VirtualHubsBox.SelectedItem;
                hubVirtualNetworkConnectionCollection = virtualHubResource.GetHubVirtualNetworkConnections();
            }
            SpokeConnectionsBox.ItemsSource = hubVirtualNetworkConnectionCollection;
            SpokeConnectionsBox.DisplayMemberPath = "Data.Name";

        }


        private void AddSegmentationDomain_Click(object sender, RoutedEventArgs e)
        {
            if (SegmentationDomainTextBox.Text != "")
            {
                SegmentationDomain segmentationDomain = new SegmentationDomain(SegmentationDomainTextBox.Text);
                segmentationDomains.Add(segmentationDomain);
                SegmentationDomainsBox.ItemsSource = segmentationDomains;
                SegmentationDomainsBox.DisplayMemberPath = "Name";
                SegmentationDomainTextBox.Clear();
            }
        }

        private void AddSpokesToDomain_Click(object sender, RoutedEventArgs e)
        {
            if (SegmentationDomainsBox.SelectedItem != null)
            {
                SegmentationDomain segmentationDomain = (SegmentationDomain)SegmentationDomainsBox.SelectedItem;
                if (SpokeConnectionsBox.SelectedItems.Count > 0)
                {
                    foreach (HubVirtualNetworkConnectionResource selecteditem in SpokeConnectionsBox.SelectedItems)
                    {
                        segmentationDomain.Members.Add(selecteditem);
                    }
                }
            }
        }
        private void AddLinkGroup_Click(object sender, RoutedEventArgs e)
        {
            if (LinkGroupTextBox.Text != "")
            {
                LinkGroup linkGroup = new LinkGroup(LinkGroupTextBox.Text);
                linkGroups.Add(linkGroup);
                LinkGroupsBox.ItemsSource = linkGroups;
                LinkGroupsBox.DisplayMemberPath = "Name";
                LinkedGroupsBox.ItemsSource = linkGroups;
                LinkedGroupsBox.DisplayMemberPath = "Name";
                LinkGroupTextBox.Clear();
            }
        }
        private void AddDomainToGroup_Click(object sender, RoutedEventArgs e)
        {
            if (SegmentationDomainsBox.SelectedItem != null && LinkGroupsBox.SelectedItem != null)
            {
                SegmentationDomain segmentationDomain = (SegmentationDomain)SegmentationDomainsBox.SelectedItem;
                LinkGroup linkedGroup = (LinkGroup)LinkGroupsBox.SelectedItem;
                linkedGroup.Members.Add(segmentationDomain);
            }
        }
        private void ConnectLinkGroup_Click(object sender, RoutedEventArgs e)
        {
            if (LinkedGroupsBox.SelectedItems.Count > 1)
            {
                linkedGroups = new ObservableCollection<LinkGroup>();
                foreach (LinkGroup linkGroup in LinkedGroupsBox.SelectedItems)
                {
                    linkedGroups.Add(linkGroup);
                }
            }
        }

        private void ListSpokesInDomain_Click(object sender, RoutedEventArgs e)
        {
            if (SegmentationDomainsBox.SelectedItem != null)
            {
                SegmentationDomain selectedDomain = (SegmentationDomain)SegmentationDomainsBox.SelectedItem;
                SpokesInDomainBox.ItemsSource = selectedDomain.Members;
                SpokesInDomainBox.DisplayMemberPath = "Data.Name";
            }
        }
        private void ListDomainsInGroup_Click(object sender, RoutedEventArgs e)
        {
            if (LinkGroupsBox.SelectedItem != null)
            {
                LinkGroup linkedGroup = (LinkGroup)LinkGroupsBox.SelectedItem;
                DomainsInGroupBox.ItemsSource = linkedGroup.Members;
                DomainsInGroupBox.DisplayMemberPath = "Name";
            }
        }
        private void ListRTsInHub_Click(object sender, RoutedEventArgs e)
        {
            if (VirtualHubsBox != null)
            {
                VirtualHubResource virtualHubResource = (VirtualHubResource)VirtualHubsBox.SelectedItem;
                RouteTablesInVirtualHubsBox.ItemsSource = null;
                RouteTablesInVirtualHubsBox.ItemsSource = virtualHubResource.GetHubRouteTables();
                RouteTablesInVirtualHubsBox.DisplayMemberPath = "Data.Name";
            }
        }
        private async void ProgramRTsInHubs_Click(object sender, RoutedEventArgs e)
        {
            ProgramRTsInHubs.IsEnabled = false;
            string progress = "";
            foreach (SegmentationDomain segmentationDomain in segmentationDomains)
            {
                HubRouteTableData hubRouteTableData = new HubRouteTableData();
                hubRouteTableData.Name = segmentationDomain.Name;
                foreach (VirtualHubResource virtualHubResource in virtualHubResources)
                {
                    
                    string subscrId = subscriptionResource.Id.ToString()[15..]; //subscriptionResource.Id returns "/subscriptions/<id>, needs to be truncated to <id>
                    ResourceIdentifier hubRTResourceIdentifier = HubRouteTableResource.CreateResourceIdentifier(subscrId, resourceGroup.Data.Name, virtualHubResource.Data.Name, hubRouteTableData.Name);//CreateResourceIdentifier is a static method on class HubRouteTableResource so can be called on class directly without object instance
                    hubRouteTableData.Id = hubRTResourceIdentifier;
                    hubRouteTable = armClient.GetHubRouteTableResource(hubRTResourceIdentifier);//gets a new HubRouteTableResource object
                    progress = progress + $"updating hub {virtualHubResource.Data.Name} with route table {hubRouteTableData.Name} \n";
                    ProgressBox.Text = progress;
                    await hubRouteTable.UpdateAsync(Azure.WaitUntil.Started, hubRouteTableData);//calls API to (create or update) route table on the hub
                }
                PropagatedRouteTable propagatedRouteTable = new PropagatedRouteTable();

                foreach (HubVirtualNetworkConnectionResource hubVirtualNetworkConnection in segmentationDomain.Members)
                {
                    HubVirtualNetworkConnectionData hubVirtualNetworkConnectionData = hubVirtualNetworkConnection.Data;
                    WritableSubResource writableSubResource = new WritableSubResource();
                    writableSubResource.Id = hubRouteTable.Id;
                    routingConfiguration = new RoutingConfiguration();
                    routingConfiguration.AssociatedRouteTableId = hubRouteTable.Id;
                    propagatedRouteTable.Ids.Add(writableSubResource);
                    hubVirtualNetworkConnectionData.RoutingConfiguration.AssociatedRouteTableId = hubRouteTableData.Id;
                    hubVirtualNetworkConnectionData.RoutingConfiguration.PropagatedRouteTables = propagatedRouteTable;
                    progress = progress + $"updating hubVirtualNetworkConnection {hubVirtualNetworkConnection.Data.Name} with associated route table {hubRouteTableData.Name} and propagating route tables {propagatedRouteTable.Ids} \n";
                    ProgressBox.Text = progress;
                    await hubVirtualNetworkConnection.UpdateAsync(Azure.WaitUntil.Started, hubVirtualNetworkConnectionData);
                }
            }
            ProgramRTsInHubs.IsEnabled = true;
        }
        private async void RemoveRTsFromHubs_Click(object sender, RoutedEventArgs e)
        {
            RemoveRTsFromHubs.IsEnabled = false;
            string progress = "";
            foreach (VirtualHubResource virtualHubResource in virtualHubResources)
            {
                HubRouteTableCollection hubRouteTables = virtualHubResource.GetHubRouteTables();
                foreach (HubRouteTableResource hubRouteTable in hubRouteTables)
                {
                    if (hubRouteTable.Data.Name != "defaultRouteTable" && hubRouteTable.Data.Name != "noneRouteTable")
                    {
                        await hubRouteTable.DeleteAsync(Azure.WaitUntil.Completed);
                        progress = progress + $"removing routetable {hubRouteTable.Data.Name} from hub {virtualHubResource.Data.Name} \n";
                        ProgressBox.Text = progress;
                    }
                }
            }
            RemoveRTsFromHubs.IsEnabled = true;
        }
    }
}
