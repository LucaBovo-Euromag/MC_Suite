using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MC_Suite.Properties;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Services.Printing
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class ReportVerificator : Page
    {
        public RichTextBlock TextContentBlock { get; set; }
        public string Linea1 { get; set; }

        public ReportVerificator()
        {
            this.InitializeComponent();
            TextContentBlock = TextContent;

            MainInfo.Text =     "Date:\t\t\t" + DataToPrint.Data_Test + "\n" +
                                "Operator:\t\t\t" + DataToPrint.OperatoreTest + "\n" +
                                "Company:\t\t\t" + DataToPrint.Company + " - " + DataToPrint.CompanyLocation + "\n" +
                                "Customer:\t\t\t" + DataToPrint.Customer + " - " + DataToPrint.CustomerLocation + "\n\r";
            FlowmeterInfoInfo.Text =
                                "Model:\t\t\t" + DataToPrint.Modello_Convertitore + "\n" +
                                "Part N:\t\t\t" + DataToPrint.Matricola_Convertitore + "\n" +
                                "Calibration Factor:\t\t" + DataToPrint.KA + "\n" +
                                "Full Scale:\t\t\t" + DataToPrint.FondoScala + "m/s\n" +
                                "Pulse Output:\t\t" + DataToPrint.Impulsi + "\n\r";

            ConverterInfoInfo.Text =
                                "Model:\t\t\t" + DataToPrint.Modello_Sensore + "\n" +
                                "Part N:\t\t\t" + DataToPrint.Matricola_Sensore + "\n\r";

            FlowmeterResults.Text =
                                "Test 4/20 mA:\t\t" + DataToPrint.AnalogOut + "\n" +
                                "Simulation:\t\t\t" + DataToPrint.Simulation + "\n" +
                                "Empty Pipe:\t\t\t" + DataToPrint.EmptyPype + "\n" +                                
                                "Energy Coil:\t\t" + DataToPrint.EnergyCoil + "\n" +
                                "I/O:\t\t\t" + DataToPrint.IO + "\n";

            ConverterResults.Text =
                                "RL AB:\t\t\t" + DataToPrint.CoilResistance + "\n" +
                                "Isolation A-C:\t\t" + DataToPrint.IsolationAC + "\n" +
                                "Isolation T-C:\t\t" + DataToPrint.IsolationTC + "\n" +
                                "Isolation D-C:\t\t" + DataToPrint.IsolationDC + "\n" +
                                "Isolation E-C:\t\t" + DataToPrint.IsolationEC + "\n\r";

            ResultDisclaimer.Text =
                                "N.B.:\t\t" + "The above test and results verify that the flowmeter is functioning within\n" +
                                "\t\t normal working limits and is within + 1% of original Calibration certificate\n\r";

            ReportNote.Text =   "NOTE:\t" + DataToPrint.Note + "\n\r";

            VerificatorInfo.Text =
                                "SN:\t\t\t" + DataToPrint.SN_Verificator + "\n" +
                                "Software Version:\t\t" + DataToPrint.SW_Ver_Verificator + "\n" +
                                "Last Calibration Date:\t\t" + DataToPrint.DataCalibrazione + "\n" +
                                "Next Calibration Date:\t\t" + DataToPrint.NuovaCalibrazione + "\n\r";

            Signature.Text =    "Signature   ___________________\n";

            FooterLine1.Text =  "Euromag International Srl\t" + "web: www.euromag.com";
            FooterLine2.Text =  "Via della Tecnica, 20 -35035\t" + "e-mail: info@euromag.com";
            FooterLine3.Text =  "Mestrino\t\t\t" + "phone: +39/049.9005064";
            FooterLine4.Text =  "PADOVA-ITALY\t\t" + "fax: +39/049.9007764\t\t" + "Rev 2.0";
        }

        private ReportLine DataToPrint
        {
            get { return Settings.Instance.ReportToPrint; }
        }

    }
}
