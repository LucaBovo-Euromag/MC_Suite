using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MC_Suite.Properties;
using MC_Suite.Services;
using MC_Suite.Euromag.Protocols.StdCommands;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class DataLogPage : Page
    {
        public DataLogPage()
        {
            this.InitializeComponent();
            this.DataContext = new DataLogPageViewModel();            
        }

        private void DataLogGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Ascending)
            {
                //Use the Tag property to pass the bound column name for the sorting implementation 
                if (e.Column.Tag.ToString() == "Row N.")
                {
                    e.Column.SortDirection = DataGridSortDirection.Descending;                    
                }
            }
            else
            {
                //Use the Tag property to pass the bound column name for the sorting implementation 
                if (e.Column.Tag.ToString() == "Row N.")
                {
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
        }
    }
}
