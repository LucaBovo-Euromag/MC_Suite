using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using System.Windows.Input;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using MC_Suite.Services;
using MC_Suite.Properties;
using MC_Suite.Euromag.Devices;
using MC_Suite.Euromag.Protocols;
using MC_Suite.Euromag.Protocols.StdCommands;
using Microsoft.Toolkit.Uwp;
using Telerik;
using Windows.UI.Xaml.Navigation;


// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class GraphPage : Page
    {
        public GraphPage()
        {
            this.InitializeComponent();
            Settings.Instance.UpdateRunning = true;
            this.DataContext = new GraphViewModel();
            this.GraphChart.DataContext = new GraphViewModel();
        }
    }
}
