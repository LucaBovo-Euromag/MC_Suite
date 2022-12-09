using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Services.Printing
{
    /// <summary>
    /// Page content to send to the printer
    /// </summary>
    public sealed partial class PageToPrint : Page
    {
        public RichTextBlock TextContentBlock { get; set; }
        public string TestoAggiornabile { get; set; }

        public PageToPrint(string Title)
        {
            this.InitializeComponent();
            TextContentBlock = TextContent;
            TestoAggiornabile = Title;
        }
    }
}
