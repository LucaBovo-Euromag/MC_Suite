using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using MC_Suite.Services;
using MC_Suite.Services.Printing;
using MC_Suite.Properties;
using MC_Suite.Euromag.Protocols.StdCommands;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class ReportDatabase : Page
    {
        public ReportDatabase()
        {
            this.InitializeComponent();

            Printer = new PrintHelper(this);

            if (Printer.IsSupported)
            { 
                // Initialize print content for this scenario
                PrintReportBtn.Visibility = Visibility.Visible;
            }
            else
                PrintReportBtn.Visibility = Visibility.Collapsed;

            ReporViewer.Visibility = Visibility.Collapsed;
            ExportReportBtn.Visibility = Visibility.Collapsed;
        }

        public DataAccess Verificator
        {
            get
            {
                return DataAccess.Instance;
            }
        }

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
            }
        }

        private ICommand _delReportCmd;
        public ICommand DelReportCmd
        {
            get
            {
                if (_delReportCmd == null)
                {
                    _delReportCmd = new RelayCommand(
                        param => DelReport()
                            );
                }
                return _delReportCmd;
            }
        }

        private async void DelReport()
        {
            if (ReportDB.SelectedItem != null)
            {
                var dialog = new MessageDialog("Are you sure?");
                dialog.Title = "Delete Report";
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
                var res = await dialog.ShowAsync();

                if ((int)res.Id == 0)
                {
                    ReportLine ReportToDelete = ReportDB.SelectedItem as ReportLine;

                    DataAccess.DeleteData("MC_Suite_DataBase.db", ReportToDelete.ID);

                    Verificator.ReportList = DataAccess.GetData("MC_Suite_DataBase.db");

                    Verificator.ReportView.Clear();
                    Verificator.ReportList.ForEach(p => Verificator.ReportView.Add(p));
                }
            }
        }

        private ICommand _openReportCmd;
        public ICommand OpenReportCmd
        {
            get
            {
                if (_openReportCmd == null)
                {
                    _openReportCmd = new RelayCommand(
                        param => OpenReport()
                            );
                }
                return _openReportCmd;
            }
        }

        private bool ShowDisclaimer;

        private void OpenReport()
        {
            if (ReportDB.SelectedItem != null)
            {
                ReporViewer.Visibility = Visibility.Visible;
                ExportReportBtn.Visibility = Visibility.Visible;

                ReportLine ReportToShow = ReportDB.SelectedItem as ReportLine;

                MainInfo.Text =     "Date:\t\t\t" + ReportToShow.Data_Test + "\n" +
                                    "Operator:\t\t\t" + ReportToShow.OperatoreTest + "\n" +
                                    "Company:\t\t\t" + ReportToShow.Company + " - " + ReportToShow.CompanyLocation + "\n" +
                                    "Customer:\t\t\t" + ReportToShow.Customer + " - " + ReportToShow.CustomerLocation + "\n\r";
                FlowmeterInfoInfo.Text =
                                    "Model:\t\t\t" + ReportToShow.Modello_Convertitore + "\n" +
                                    "Part N:\t\t\t" + ReportToShow.Matricola_Convertitore + "\n" +
                                    "Calibration Factor:\t\t" + ReportToShow.KA + "\n" +
                                    "Full Scale:\t\t\t" + ReportToShow.FondoScala + "\n" +
                                    "Pulse Output:\t\t" + ReportToShow.Impulsi + "\n\r";

                SensorInfo.Text =
                                    "Model:\t\t\t" + ReportToShow.Modello_Sensore + "\n" +
                                    "Part N:\t\t\t" + ReportToShow.Matricola_Sensore + "\n\r";

                FlowmeterResults.Text =
                                    "Test 4/20 mA:\t\t" + ReportToShow.AnalogOut + "\n" +
                                    "Simulation:\t\t\t" + ReportToShow.Simulation + "\n" +
                                    "Empty Pipe:\t\t" + ReportToShow.EmptyPype + "\n" +
                                    "Energy Coil:\t\t" + ReportToShow.EnergyCoil + "\n" +
                                    "I/O:\t\t\t" + ReportToShow.IO + "\n";

                ShowDisclaimer = true;

                if (FlowmeterResults.Text.Contains("FAIL"))
                    ShowDisclaimer = false;

                SensorResults.Text =
                                    "RL AB:\t\t\t" + ReportToShow.CoilResistance + "\n" +
                                    "Isolation A-C:\t\t" + ReportToShow.IsolationAC + "\n" +
                                    "Isolation T-C:\t\t" + ReportToShow.IsolationTC + "\n" +
                                    "Isolation D-C:\t\t" + ReportToShow.IsolationDC + "\n" +
                                    "Isolation E-C:\t\t" + ReportToShow.IsolationEC + "\n" +
                                    "Test Mode:\t\t\t" + ReportToShow.TestType + "\n\r";

                if (SensorResults.Text.Contains("FAIL"))
                    ShowDisclaimer = false;

                if (ShowDisclaimer)
                {
                    ResultDisclaimer.Text =
                                        //"N.B.:\t\t" + "The above test and results verify that the flowmeter is functioning within\n" +
                                        "\t The above test and results verify that the flowmeter is functioning within\n" +
                                        "\t normal working limits and is within + 1% of original Calibration certificate\n\r";
                }
                else
                    ResultDisclaimer.Text = "";

                ReportNote.Text = "NOTE:\t" + ReportToShow.Note + "\n\r";

                VerificatorInfo.Text =
                                    "SN:\t\t\t" + ReportToShow.SN_Verificator + "\n" +
                                    "Software Version:\t\t" + ReportToShow.SW_Ver_Verificator + "\n" +
                                    "Last Calibration Date:\t\t" + ReportToShow.DataCalibrazione + "\n" +
                                    "Next Calibration Date:\t\t" + ReportToShow.NuovaCalibrazione + "\n\r";

                Signature.Text = "Signature   ___________________\n";

                FooterLine1.Text = "Euromag International Srl\t" + "web: www.euromag.com";
                FooterLine2.Text = "Via della Tecnica, 20 -35035\t" + "e-mail: euromag@euromag.com";
                FooterLine3.Text = "Mestrino\t\t\t" + "phone: +39/049.9005064";
                FooterLine4.Text = "PADOVA-ITALY\t\t" + "fax: +39/049.9007764\t\t" + "Rev 2.0";
            }                  
        }

        public RichTextBlock TextContentBlock { get; set; }

        private string _linea1;
        public string Linea1
        {
            get { return _linea1; }
            set
            {
                if (value != _linea1)
                {
                    _linea1 = value;
                    OnPropertyChanged("Linea1");
                }
            }
        }

        private ICommand _saveReportCmd;
        public ICommand SaveReportCmd
        {
            get
            {
                if (_saveReportCmd == null)
                {
                    _saveReportCmd = new RelayCommand(
                        param => SaveReport()
                            );
                }
                return _saveReportCmd;
            }
        }

        private async void SaveReport()
        {
            if (ReportDB.SelectedItem != null)
            {                
                ReportLine ReportToSave = ReportDB.SelectedItem as ReportLine;

                string ReportLine = ReportToSave.ID.ToString() + ";" +
                                    ReportToSave.Data_Test + ";" +
                                    ReportToSave.OperatoreTest.ToString() + ";" +
                                    ReportToSave.Modello_Sensore.ToString() + ";" +
                                    ReportToSave.Matricola_Sensore.ToString() + ";" +
                                    ReportToSave.Modello_Convertitore.ToString() + ";" +
                                    ReportToSave.Matricola_Convertitore.ToString() + ";" +
                                    ReportToSave.KA.ToString() + ";" +
                                    ReportToSave.FondoScala.ToString() + ";" +
                                    ReportToSave.Impulsi.ToString() + ";" +
                                    //Convertitore******************************************
                                    ReportToSave.AnalogOut.ToString() + ";" +
                                    ReportToSave.Simulation.ToString() + ";" +
                                    ReportToSave.Zero_read.ToString("#.000") + ";" +
                                    ReportToSave.Hi_read.ToString("#.000") + ";" +
                                    ReportToSave.LO_read.ToString("#.000") + ";" +
                                    ReportToSave.EmptyPype.ToString() + ";" +
                                    ReportToSave.EnergyCoil + ";" +
                                    ReportToSave.ICoil_Read.ToString() + ";" +
                                    ReportToSave.IO.ToString() + ";" +
                                    ReportToSave.TempPCB.ToString() + ";" +
                                    //Sensore***********************************************
                                    ReportToSave.CoilResistance.ToString() + ";" +
                                    ReportToSave.IsolationAC.ToString() + ";" +
                                    ReportToSave.IsolationTC.ToString() + ";" +
                                    ReportToSave.IsolationDC.ToString() + ";" +
                                    ReportToSave.IsolationEC.ToString() + ";" +
                                    ReportToSave.TestType.ToString() + ";" +
                                    //******************************************************
                                    ReportToSave.Company.ToString() + ";" +
                                    ReportToSave.CompanyLocation.ToString() + ";" +
                                    ReportToSave.Customer.ToString() + ";" +
                                    ReportToSave.CustomerLocation.ToString() + ";" +
                                    ReportToSave.Note.ToString() + ";" +
                                    ReportToSave.SW_Ver_Verificator.ToString() + ";" +
                                    ReportToSave.SN_Verificator.ToString() + ";" +
                                    ReportToSave.DataCalibrazione.ToString() + ";" +
                                    ReportToSave.NuovaCalibrazione.ToString() + "\n";
                char[] ReportLineArray = ReportLine.ToCharArray();

                FileManager.SavedFileName = "Report_" + ReportToSave.Customer.ToString() + "_" +
                                            ReportToSave.Matricola_Convertitore + "_" +
                                            ReportToSave.Matricola_Sensore + "_" +
                                            DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + ".tmp";

                await BinaryStorage.Append(FileManager.SavedFileName, FileManager.CurrentFolder.Path, ReportLineArray);                
            }

            //Encripta il file
            string SavedReport = FileCrypt.Instance.EncryptFile(FileManager.SavedFileName, ".vrp");
            int startFileName = SavedReport.LastIndexOf("\\") + 1;
            // Change the file's extension to ".enc"
            SavedReport = SavedReport.Substring(startFileName, FileManager.SavedFileName.Length);
            //Cancella il file in chiaro
            await SerializableStorage<VariableImage>.Delete(FileManager.SavedFileName, FileManager.CurrentFolder.Path, false);

            var dialog = new MessageDialog(SavedReport + " Saved");
            var res = await dialog.ShowAsync();
        }

        private ICommand _saveReportDatabaseCmd;
        public ICommand SaveReportDatabaseCmd
        {
            get
            {
                if (_saveReportDatabaseCmd == null)
                {
                    _saveReportDatabaseCmd = new RelayCommand(
                        param => SaveReportDatabase()
                            );
                }
                return _saveReportDatabaseCmd;
            }
        }

        private async void SaveReportDatabase()
        {
            FileManager.SavedFileName = "DataBase_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm") + ".tmp";

            Verificator.ReportList = DataAccess.GetData("MC_Suite_DataBase.db");

            for(int i = 0; i < Verificator.ReportList.Count; i++)
            {
                string ReportLine = Verificator.ReportList[i].ID.ToString() + ";" +
                                    Verificator.ReportList[i].Data_Test + ";" +
                                    Verificator.ReportList[i].OperatoreTest.ToString() + ";" +
                                    Verificator.ReportList[i].Modello_Sensore.ToString() + ";" +
                                    Verificator.ReportList[i].Matricola_Sensore.ToString() + ";" +
                                    Verificator.ReportList[i].Modello_Convertitore.ToString() + ";" +
                                    Verificator.ReportList[i].Matricola_Convertitore.ToString() + ";" +
                                    Verificator.ReportList[i].KA.ToString() + ";" +
                                    Verificator.ReportList[i].FondoScala.ToString() + ";" +
                                    Verificator.ReportList[i].Impulsi.ToString() + ";" +
                                    //Convertitore******************************************
                                    Verificator.ReportList[i].AnalogOut.ToString() + ";" +
                                    Verificator.ReportList[i].Simulation.ToString() + ";" +
                                    Verificator.ReportList[i].Zero_read.ToString("#.000") + ";" +
                                    Verificator.ReportList[i].Hi_read.ToString("#.000") + ";" +
                                    Verificator.ReportList[i].LO_read.ToString("#.000") + ";" +
                                    Verificator.ReportList[i].EmptyPype.ToString() + ";" +
                                    Verificator.ReportList[i].EnergyCoil + ";" +
                                    Verificator.ReportList[i].ICoil_Read.ToString() + ";" +
                                    Verificator.ReportList[i].IO.ToString() + ";" +
                                    Verificator.ReportList[i].TempPCB.ToString() + ";" +
                                    //Sensore***********************************************
                                    Verificator.ReportList[i].CoilResistance.ToString() + ";" +
                                    Verificator.ReportList[i].IsolationAC.ToString() + ";" +
                                    Verificator.ReportList[i].IsolationTC.ToString() + ";" +
                                    Verificator.ReportList[i].IsolationDC.ToString() + ";" +
                                    Verificator.ReportList[i].IsolationEC.ToString() + ";" +
                                    Verificator.ReportList[i].TestType.ToString() + ";" +
                                    //******************************************************
                                    Verificator.ReportList[i].Company.ToString() + ";" +
                                    Verificator.ReportList[i].CompanyLocation.ToString() + ";" +
                                    Verificator.ReportList[i].Customer.ToString() + ";" +
                                    Verificator.ReportList[i].CustomerLocation.ToString() + ";" +
                                    Verificator.ReportList[i].Note.ToString() + ";" +
                                    Verificator.ReportList[i].SW_Ver_Verificator.ToString() + ";" +
                                    Verificator.ReportList[i].SN_Verificator.ToString() + ";" +
                                    Verificator.ReportList[i].DataCalibrazione.ToString() + ";" +
                                    Verificator.ReportList[i].NuovaCalibrazione.ToString() + "\n";
                char[] ReportLineArray = ReportLine.ToCharArray();
                await BinaryStorage.Append(FileManager.SavedFileName, FileManager.CurrentFolder.Path, ReportLineArray);
            }

            //Encripta il file
            FileCrypt.Instance.EncryptFile(FileManager.SavedFileName, ".vdb");
            //Cancella il file in chiaro
            await SerializableStorage<VariableImage>.Delete(FileManager.SavedFileName, FileManager.CurrentFolder.Path, false);

            var dialog = new MessageDialog("Database Saved");
            var res = await dialog.ShowAsync();
        }

        #region ObservableObject

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private async void PrintReportBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ReportDB.SelectedItem != null)
            {
                if (Printer != null)
                {
                    Printer.UnregisterForPrinting();
                }

                Printer.RegisterForPrinting();

                Settings.Instance.ReportToPrint = ReportDB.SelectedItem as ReportLine;

                Printer.PreparePrintContent(new ReportVerificator());

                await Printer.ShowPrintUIAsync();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Printer != null)
            {
                Printer.UnregisterForPrinting();
            }
        }

        private PrintHelper Printer;
    }
}
