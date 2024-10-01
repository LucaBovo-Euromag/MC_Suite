﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using MC_Suite.Helpers;
using MC_Suite.Services;
using MC_Suite.Properties;

using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Core;
using System.Windows.Input;

using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.UI.Popups;
using MC_Suite.Modbus;

namespace MC_Suite.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        private NavigationViewItem _selected;

        public NavigationViewItem Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ShellPage()
        {
            InitializeComponent();
            HideNavViewBackButton();
            DataContext = this;
            Initialize();
        }


        private async void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.Navigated += Frame_Navigated;
            KeyboardAccelerators.Add(ActivationService.AltLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(ActivationService.BackKeyboardAccelerator);

            CustomDictionary.Instance.InitDictionaries();

            PowerMessage.Visibility = Visibility.Collapsed;

            LoadVerifConfigFile();

            //Interface*******************************************************************
            ComSetup.ConfigurationPageReady    = false;
            ComSetup.Verificator406PageReady   = false;
            ComSetup.Verificator608PageReady   = false;
            InitConverterSelection();
            ComSetup.SensorOnly                = false;

            if(RAM_VerifConfiguration.TarMode)
                Calibration_Tab.Visibility     = Visibility.Visible;
            else
                Calibration_Tab.Visibility     = Visibility.Collapsed;

            IO_Tab.Visibility                  = Visibility.Collapsed;

            CommResources.DryTestVisibility    = Visibility.Collapsed;

            DispatcherTimer RefreshInterface = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            RefreshInterface.Tick += RefreshInterface_Tick;
            RefreshInterface.Start();

            //Connection******************************************************************
            ConnectionStateVis.Fill = new SolidColorBrush(Colors.Red);
            ConnectionState = "Offline";
            IrConnection.PropertyChanged += IrConnection_PropertyChanged;
            MbConnection.PropertyChanged += MbConnection_PropertyChanged;
            ComSetup.PropertyChanged += ComSetup_PropertyChanged;

            ComSetup.getPortNames();

            //****************************************************************************            

            //DataBase********************************************************************
            InitDatabase();

            //****************************************************************************

            //File System*****************************************************************
            FileManager.ViewFileFilter = Storage.ViewFilesTypes.User;

            FileManager.CurrentFolder = FileManager.MainFolder;
            await FileManager.UpdateFileList(FileManager.CurrentFolder);

            //USB Device Watcher**********************************************************

            FileManager.UsbDriveChanged = await FileManager.GetUSBDrives();;
            Watcher = DeviceInformation.CreateWatcher();
            Watcher.Updated += Watcher_Updated;
            //Watcher.Added += Watcher_Added;
            Watcher.Start();

            //****************************************************************************

            //Sys Clock Refresh***********************************************************
            DispatcherTimer RefreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            RefreshTimer.Tick += RefreshTimer_Tick;
            RefreshTimer.Start();
            SysDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            //*****************************************************************************

            Settings.Instance.VerificatorRunning = false;

            RunningConfiguration.Instance.QuickTarEnabled = false;

            navigationView.IsPaneOpen = false;

            LoadConfigFile();

            FileCrypt.Instance.InitCryptography();

            AnalogMeasures.PowerMonitorOn = true;

            InitVpowerMonitor();

            BatteryChartViewModel.Instance.BatteryChartInit();

            ShowInitMessage();
        }

        private void RefreshInterface_Tick(object sender, object e)
        {
            if ((RAM_VerifConfiguration.TarMode == true) || (RunningConfiguration.Instance.QuickTarEnabled == true))
            {
                CalibrationIcon.Visibility = Visibility.Visible;
                Calibration_Tab.Visibility = Visibility.Visible;
                IO_Tab.Visibility = Visibility.Visible;
                DryCalibration_Tab.Visibility = Visibility.Visible;
            }
            else
            {
                CalibrationIcon.Visibility = Visibility.Collapsed;
                Calibration_Tab.Visibility = Visibility.Collapsed;
                IO_Tab.Visibility = Visibility.Collapsed;
                DryCalibration_Tab.Visibility = Visibility.Collapsed;
            }

            if(AnalogMeasures.PowerMonitorOn == true)
            { 
                if(RAM_Configuration.ViewValim == true)
                    BatteryPercBlock.Visibility = Visibility.Visible;
                else
                    BatteryPercBlock.Visibility = Visibility.Collapsed;
            }

        }

        public GPIO_Device GPIO_Control
        {
            get
            {
                return GPIO_Device.Instance;
            }
        }

        public AnalogsService AnalogMeasures
        {
            get
            {
                return AnalogsService.Instance;
            }
        }

        public GraphData BatteryData
        {
            get
            {
                return GraphData.Instance;
            }
        }

        #region VAlim Monitor

        private string _valimText;
        public string VAlimText
        {
            get { return _valimText; }
            set
            {
                if (value != _valimText)
                {
                    _valimText = value;
                    OnPropertyChanged("VAlimText");
                }
            }
        }

        private string _batteryPercText;
        public string BatteryPercText
        {
            get { return _batteryPercText; }
            set
            {
                if (value != _batteryPercText)
                {
                    _batteryPercText = value;
                    OnPropertyChanged("BatteryPercText");
                }
            }
        }

        private async void InitVpowerMonitor()
        {
            if (AnalogMeasures.PowerMonitorOn)
            {
                GPIO_Control.InitGPIO();

                if (AnalogMeasures.ADC_ModuleIsReady == false)
                    await AnalogMeasures.ADC_Module_Open();

                if (AnalogMeasures.ADC_Module.Ready)
                {
                    AnalogMeasures.ADC_MeasuresStart();
                    RefreshValimStart();
                    ChargingTimerInit();
                }
                else
                    BatteryIcon.Visibility = Visibility.Collapsed;

                GPIO_Control.ResLowBatteryOut();
            }
            else
            {
                BatteryIcon.Visibility = Visibility.Collapsed;
                VAlimBlock.Visibility = Visibility.Collapsed;
                BatteryPercBlock.Visibility = Visibility.Collapsed;
                AnalogMeasures.VAlimLow = false;
            }
        }

        private DispatcherTimer RefreshValimTimer;
        private void RefreshValimStart()
        {
            RefreshValimTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            RefreshValimTimer.Tick += RefreshValimTimer_Tick;
            RefreshValimTimer.Start();
        }
        //private float memVAlim;

        /*
        private async void RefreshValimTimer_Tick(object sender, object e)
        {
            AnalogMeasures.AggiornaMisure();

            if( GPIO_Control.GetChargingIn().value == true)
            {
                ChargingOn = true;
            }
            else
            {
                ChargingOn = false;
            }

            if(AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura > 0)
            {
                VAlimText = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura.ToString("#.0") + "V";

                string ValimTxt = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura.ToString("#.0");
                float ValimTxtFloat = (float)Convert.ToDouble(ValimTxt);
                float BatteryPerc = (ValimTxtFloat - RAM_VerifConfiguration.Vbattery0) * (100f / (RAM_VerifConfiguration.Vbattery100 - RAM_VerifConfiguration.Vbattery0));

                if (BatteryPerc > 0)
                {
                    if (BatteryPerc > 100f)
                        BatteryPerc = 100f;
                    if ((BatteryPerc < 10f) && (BatteryPerc >= 5f))
                        BatteryPerc = 10f;

                    string tmp = BatteryPerc.ToString("#");
                    BatteryPercText = tmp.Substring(0, tmp.Length - 1) + "0%";
                }
                else
                    BatteryPercText = "0%";

                if (memVAlim > 0)
                {
                    //Rilevazione attivazione carica
                    if (AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura > memVAlim)
                    {
                        if((AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura - memVAlim) >= 0.2f)
                        {
                            if (Charging == false)
                            {
                                ChargingTimerStart();
                                Charging = true;
                                //BatteryPercBlock.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    //Rilevazione disattivazione carica
                    if (AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura < memVAlim)
                    {
                        if ((memVAlim - AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura) >= 0.1f)
                        {
                            if(Charging == true )
                            { 
                                ChargingTimer.Stop();
                                Charging = false;
                                //BatteryPercBlock.Visibility = Visibility.Visible;
                                //UpdateVbattery100();
                            }
                        }
                    }
                }

                if ( AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura >= RAM_VerifConfiguration.Vbattery100)
                {
                    //Charging
                    AnalogMeasures.VAlimLow = false;
                    if(Charging == false)
                    {
                        ChargingTimerStart();
                        Charging = true;
                        //BatteryPercBlock.Visibility = Visibility.Collapsed;
                    }                
                }
                else if (BatteryPerc <= 10.0f)
                {
                    if (AnalogMeasures.VAlimLow == false)
                    {
                        var messageDialog = new MessageDialog("Please connect the external power supply (110/240VAC)", "Battery Low");
                        await messageDialog.ShowAsync();
                        AnalogMeasures.VAlimLow = true;
                    }
                    BatteryIcon.Glyph = ((char)0xEBA0).ToString();
                }
                else
                {
                    //BatteryPercBlock.Visibility = Visibility.Visible;
                    ChargingTimer.Stop();
                    Charging = false;                    

                    if(BatteryPerc >= 100f)
                        BatteryIcon.Glyph = ((char)0xEBAA).ToString();
                    else if (BatteryPerc >= 90f)
                        BatteryIcon.Glyph = ((char)0xEBA9).ToString();
                    else if (BatteryPerc >= 80f)
                        BatteryIcon.Glyph = ((char)0xEBA8).ToString();
                    else if (BatteryPerc >= 70f)
                        BatteryIcon.Glyph = ((char)0xEBA7).ToString();
                    else if (BatteryPerc >= 60f)
                        BatteryIcon.Glyph = ((char)0xEBA6).ToString();
                    else if (BatteryPerc >= 50f)
                        BatteryIcon.Glyph = ((char)0xEBA5).ToString();
                    else if (BatteryPerc >= 40f)
                        BatteryIcon.Glyph = ((char)0xEBA4).ToString();
                    else if (BatteryPerc >= 30f)
                        BatteryIcon.Glyph = ((char)0xEBA3).ToString();
                    else if (BatteryPerc >= 20f)
                        BatteryIcon.Glyph = ((char)0xEBA2).ToString();
                    else if (BatteryPerc >= 10f)
                        BatteryIcon.Glyph = ((char)0xEBA1).ToString();
                    else
                        BatteryIcon.Glyph = ((char)0xEBA0).ToString();
                }

                memVAlim = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura;
            }
            else
                VAlimText = "--";
        }
        */

        private async void RefreshValimTimer_Tick(object sender, object e)
        {
            AnalogMeasures.AggiornaMisure();

            if (AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura > 0)
            {
                VAlimText = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura.ToString("#.0") + "V";

                string ValimTxt = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura.ToString("#.0");
                float ValimTxtFloat = (float)Convert.ToDouble(ValimTxt);
                float BatteryPerc = (AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura - RAM_VerifConfiguration.Vbattery0) * (100f / (RAM_VerifConfiguration.Vbattery100 - RAM_VerifConfiguration.Vbattery0));

                if (BatteryData.Charging == false)
                {
                    if (BatteryPerc > 0)
                    {
                        if (BatteryPerc >= 95f)
                            BatteryPerc = 100f;
                        if ((BatteryPerc < 10f) && (BatteryPerc >= 5f))
                            BatteryPerc = 10f;

                        string tmp = BatteryPerc.ToString("#");
                        BatteryPercText = tmp.Substring(0, tmp.Length - 1) + "0";
                        BatteryData.BatteryPercValue = Convert.ToInt32(BatteryPercText);
                        BatteryPercText += "%";
                    }
                    else
                    { 
                        BatteryData.BatteryPercValue = 0;
                        BatteryPercText = "0%";
                    }
                }
                else
                {
                    BatteryData.BatteryPercValue = 100;
                    BatteryPercText = "";
                }
                    
                // Attivazione/Disattivazione carica
                if (GPIO_Control.GetChargingIn().value == true)
                {
                    //Charging
                    AnalogMeasures.VAlimLow = false;
                    if (BatteryData.Charging == false)
                    {
                        ChargingTimerStart();
                        BatteryData.Charging = true;
                        GPIO_Control.ResLowBatteryOut();
                        BatteryIcon.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (BatteryData.Charging == true)
                    {
                        ChargingTimer.Stop();
                        //UpdateVbattery100();
                        BatteryData.Charging = false;
                    }
                }

                if(RAM_VerifConfiguration.TarMode == false)
                { 
                    if ((BatteryPerc <= 20.0f) && (BatteryData.Charging == false))
                    {
                        if (AnalogMeasures.VAlimLow == false)
                        {
                            AnalogMeasures.VAlimLow = true;
                            GPIO_Control.SetLowBatteryOut();

                            ContentDialog messageDialog = new ContentDialog()
                            {
                                Title = "Battery Low",
                                Content = "Please connect the external power supply",
                                CloseButtonText = "OK",
                            };
                            await messageDialog.ShowAsync();
                        }
                        BatteryIcon.Glyph = ((char)0xEBA0).ToString();
                    }
                    else
                    {
                        if(BatteryData.Charging == false)
                        {
                            if(AnalogMeasures.VAlimLow)
                            {
                                AnalogMeasures.VAlimLow = false;
                                GPIO_Control.ResLowBatteryOut();
                            }
                        }
                    }
                }

                if (BatteryData.Charging == false)
                {
                    if (BatteryPerc >= 100f)
                        BatteryIcon.Glyph = ((char)0xEBAA).ToString();
                    else if (BatteryPerc >= 90f)
                        BatteryIcon.Glyph = ((char)0xEBA9).ToString();
                    else if (BatteryPerc >= 80f)
                        BatteryIcon.Glyph = ((char)0xEBA8).ToString();
                    else if (BatteryPerc >= 70f)
                        BatteryIcon.Glyph = ((char)0xEBA7).ToString();
                    else if (BatteryPerc >= 60f)
                        BatteryIcon.Glyph = ((char)0xEBA6).ToString();
                    else if (BatteryPerc >= 50f)
                        BatteryIcon.Glyph = ((char)0xEBA5).ToString();
                    else if (BatteryPerc >= 40f)
                        BatteryIcon.Glyph = ((char)0xEBA4).ToString();
                    else if (BatteryPerc >= 30f)
                        BatteryIcon.Glyph = ((char)0xEBA3).ToString();
                    else if (BatteryPerc >= 20f)
                        BatteryIcon.Glyph = ((char)0xEBA2).ToString();
                    else if (BatteryPerc >= 10f)
                        BatteryIcon.Glyph = ((char)0xEBA1).ToString();
                    else
                        BatteryIcon.Glyph = ((char)0xEBA0).ToString();

                    BatteryBlinking(BatteryPerc);
                }
            }
            else
                VAlimText = "--";
        }

        private void BatteryBlinking(float perc)
        {
            if(AnalogMeasures.PowerMonitorOn)
            { 
                if(perc <= 20.0f)
                { 
                    if (BatteryIcon.Visibility == Visibility.Visible)
                        BatteryIcon.Visibility = Visibility.Collapsed;
                    else
                        BatteryIcon.Visibility = Visibility.Visible;
                }
                else
                    BatteryIcon.Visibility = Visibility.Visible;
            }
        }

        void UpdateVbattery100()
        {
            RAM_VerifConfiguration.Vbattery100 = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura;
            List<VerificatorConfig> ConfigList = new List<VerificatorConfig>();
            ConfigList.Add(RAM_VerifConfiguration);

            SerializableStorage<VerificatorConfig>.Save(FileManager.VerificatorConfigFile, FileManager.MainFolder.Path, ConfigList);
        }

        private DispatcherTimer ChargingTimer;
        private byte ChargingState = 0;

        private void ChargingTimerInit()
        {
            ChargingTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            ChargingTimer.Tick += ChargingTimer_Tick;
        }

        private void ChargingTimerStart()
        {
            ChargingTimer.Start();
            ChargingState = 0;
        }

        private void ChargingTimer_Tick(object sender, object e)
        {
            switch(ChargingState)
            {
                case 0:
                    BatteryIcon.Glyph = ((char)0xEBAB).ToString();
                    break;
                case 1:
                    BatteryIcon.Glyph = ((char)0xEBAC).ToString();
                    break;
                case 2:
                    BatteryIcon.Glyph = ((char)0xEBAD).ToString();   //20% >= 12.1
                    break;
                case 3:
                    BatteryIcon.Glyph = ((char)0xEBAE).ToString();
                    break;
                case 4:
                    BatteryIcon.Glyph = ((char)0xEBAF).ToString();   //40% >= 12.2
                    break;
                case 5:
                    BatteryIcon.Glyph = ((char)0xEBB0).ToString();
                    break;
                case 6:
                    BatteryIcon.Glyph = ((char)0xEBB1).ToString();   //60% >= 12.3
                    break;
                case 7:
                    BatteryIcon.Glyph = ((char)0xEBB2).ToString();
                    break;
                case 8:
                    BatteryIcon.Glyph = ((char)0xEBB3).ToString();   //80% >= 12.4
                    break;
                case 9:
                    BatteryIcon.Glyph = ((char)0xEBB4).ToString();
                    break;
                case 10:
                    BatteryIcon.Glyph = ((char)0xEBB5).ToString();   //100% >= 12.5
                    break;
            }

            if (++ChargingState > 10)
                ChargingState = 0;
        }

#endregion

        private async void ShowInitMessage()
        {
            if(FileCrypt.Instance.CryptEnabled == false)
            { 
                ContentDialog cryptMessag = new ContentDialog()
                {
                    Title = "Crypt Key Not Found",
                    Content = "ATTENTION: Cryptography not avaiable",
                    CloseButtonText = "OK",
                };


                await cryptMessag.ShowAsync();
            }

            ContentDialog clockMessage = new ContentDialog()
            {
                Title = "Clock",
                Content = "Please check Date and Time",
                CloseButtonText = "OK",
            };
            
            await clockMessage.ShowAsync();
        }


        private string _activeConverter;
        public string ActiveConverter
        {
            get { return _activeConverter; }
            set
            {
                if (value != _activeConverter)
                {
                    _activeConverter = value;
                    OnPropertyChanged("ActiveConverter");
                }
            }
        }

        private void ComSetup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(ComSetup.SensorOnly)
            {
                ActiveConverter = "Sensor Only";
                Verificator_406.Visibility  = Visibility.Collapsed;
                MC406_HomeBtn.Visibility    = Visibility.Collapsed;
                MC406_CfgBtn.Visibility     = Visibility.Collapsed;
                Shell_Datalog.Visibility    = Visibility.Collapsed;
                Verificator_608.Visibility  = Visibility.Collapsed;
                MC608_HomeBtn.Visibility    = Visibility.Collapsed;
                MC608_CfgBtn.Visibility     = Visibility.Collapsed;
                SensorOnly.Visibility       = Visibility.Visible;
            }
            else if(ComSetup.ConverterSelection.MC406.Connected)
            {
                ActiveConverter = "MC406 Selected";
                Verificator_406.Visibility = Visibility.Visible;

                /*MC406_HomeBtn.Visibility = Visibility.Visible;
                if(ComSetup.ConverterSelection.MC406.ConfigEnabled)
                    MC406_CfgBtn.Visibility = Visibility.Visible;
                else
                    MC406_CfgBtn.Visibility = Visibility.Collapsed;
                Shell_Datalog.Visibility = Visibility.Visible;*/

                Verificator_608.Visibility = Visibility.Collapsed;
                MC608_HomeBtn.Visibility = Visibility.Collapsed;
                /*MC608_CfgBtn.Visibility = Visibility.Collapsed;*/

                SensorOnly.Visibility = Visibility.Collapsed;
            }
            else if(ComSetup.ConverterSelection.MC608.Connected)
            {
                ActiveConverter = "MC608 Selected";
                Verificator_406.Visibility = Visibility.Collapsed;
                /*MC406_HomeBtn.Visibility = Visibility.Collapsed;
                MC406_CfgBtn.Visibility = Visibility.Collapsed;*/

                Verificator_608.Visibility = Visibility.Visible;
                //MC608_HomeBtn.Visibility = Visibility.Visible;
                if(ComSetup.ConverterSelection.MC608.ConfigEnabled)
                    MC608_CfgBtn.Visibility = Visibility.Visible;
                else
                    MC608_CfgBtn.Visibility = Visibility.Collapsed;

                SensorOnly.Visibility = Visibility.Collapsed;
                Shell_Datalog.Visibility = Visibility.Collapsed;
            }
            else
            {
                ActiveConverter = "NO CONVERTER SELECTED";
                Verificator_406.Visibility = Visibility.Collapsed;
                MC406_HomeBtn.Visibility = Visibility.Collapsed;
                MC406_CfgBtn.Visibility = Visibility.Collapsed;

                Verificator_608.Visibility = Visibility.Collapsed;
                MC608_HomeBtn.Visibility = Visibility.Collapsed;
                MC608_CfgBtn.Visibility = Visibility.Collapsed;

                SensorOnly.Visibility = Visibility.Collapsed;
                Shell_Datalog.Visibility = Visibility.Collapsed;
            }
            ComSetup.ConverterSelectionChanged = false;
        }

        private void InitConverterSelection()
        {
            ComSetup.ConverterSelection = new Settings.ConverterModel
            {
                MC406 = new Settings.Converter(),
                MC608 = new Settings.Converter()
            };
            ComSetup.ConverterSelection.MC406.Connected = false;
            ComSetup.ConverterSelection.MC608.Connected = false;

            ActiveConverter = "NO CONVERTER SELECTED";
            Verificator_406.Visibility = Visibility.Collapsed;
            MC406_HomeBtn.Visibility = Visibility.Collapsed;
            MC406_CfgBtn.Visibility = Visibility.Collapsed;

            Verificator_608.Visibility = Visibility.Collapsed;
            MC608_HomeBtn.Visibility   = Visibility.Collapsed;
            MC608_CfgBtn.Visibility    = Visibility.Collapsed;
        }

        private void RefreshTimer_Tick(object sender, object e)
        {
            SysDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        private DeviceWatcher Watcher;
        //private async void Watcher_Added(DeviceWatcher sender, DeviceInformation args)
        //{
            //FileManager.UsbDriveChanged = await FileManager.GetUSBDrives();
        //}

        private async void Watcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            FileManager.UsbDriveChanged = await FileManager.GetUSBDrives();
            ComSetup.getPortNames();
            UpdateComPortCfg();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = navigationView.SettingsItem as NavigationViewItem;
                return;
            }

            Selected = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsPage));
                return;
            }

            var item = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
            var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
            NavigationService.Navigate(pageType);
        }

        private void HideNavViewBackButton()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                navigationView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            }
        }

        #region ConfigFile

        private void UpdateComPortCfg()
        {
            if (FileManager.ConfigList.Count > 0)
            {
                for (int i = 0; i < ComSetup.COMPortsList.Count; i++)
                {
                    if (ComSetup.COMPortsList[i].ID.Equals(FileManager.ConfigList[0].ComPort.ID))
                    { 
                        RAM_Configuration.ComPort = FileManager.ConfigList[0].ComPort;
                        ComSetup.ComPort = FileManager.ConfigList[0].ComPort;
                    }
                }

                for (int i = 0; i < ComSetup.COMPortsList.Count; i++)
                {
                    if (ComSetup.COMPortsList[i].ID.Equals(FileManager.ConfigList[0].ComPort608.ID))
                    { 
                        RAM_Configuration.ComPort608 = FileManager.ConfigList[0].ComPort608;
                        ComSetup.ComPort608 = FileManager.ConfigList[0].ComPort608;
                    }
                }
            }                        
        }

        private async void LoadConfigFile()
        {
            //List<Configuration> ConfigList = new List<Configuration>();

            try
            {
                FileManager.ConfigList = await SerializableStorage<Configuration>.Load(FileManager.ConfigFile, FileManager.MainFolder.Path);

                if (FileManager.ConfigList.Count > 0)
                {

                    RAM_Configuration.ComPort = new Settings.COMPortItem();
                    for (int i=0;i < ComSetup.COMPortsList.Count; i++)
                    {
                        if(FileManager.ConfigList[0].ComPort.ID != null)
                        { 
                            if (ComSetup.COMPortsList[i].ID.Equals( FileManager.ConfigList[0].ComPort.ID) )
                                RAM_Configuration.ComPort = FileManager.ConfigList[0].ComPort;
                        }
                    }

                    RAM_Configuration.Protocol = FileManager.ConfigList[0].Protocol;

                    RAM_Configuration.ComPort608 = new Settings.COMPortItem();
                    for (int i = 0; i < ComSetup.COMPortsList.Count; i++)
                    {
                        if (FileManager.ConfigList[0].ComPort608.ID != null)
                        {
                            if (ComSetup.COMPortsList[i].ID.Equals(FileManager.ConfigList[0].ComPort608.ID))
                                RAM_Configuration.ComPort608 = FileManager.ConfigList[0].ComPort608;
                        }
                    }

                    RAM_Configuration.Protocol608 = FileManager.ConfigList[0].Protocol608;
                    RAM_Configuration.MbAddress = FileManager.ConfigList[0].MbAddress;
                    RAM_Configuration.MbBaudRate = FileManager.ConfigList[0].MbBaudRate;
                    RAM_Configuration.MbDataBits = FileManager.ConfigList[0].MbDataBits;
                    RAM_Configuration.MbParity = FileManager.ConfigList[0].MbParity;
                    RAM_Configuration.MbStopBits = FileManager.ConfigList[0].MbStopBits;
                    RAM_Configuration.MbTimeOut = FileManager.ConfigList[0].MbTimeOut;

                    RAM_Configuration.Operator = FileManager.ConfigList[0].Operator;
                    RAM_Configuration.Company = FileManager.ConfigList[0].Company;
                    RAM_Configuration.Company_Location = FileManager.ConfigList[0].Company_Location;
                    RAM_Configuration.Customer = FileManager.ConfigList[0].Customer;
                    RAM_Configuration.Customer_Location = FileManager.ConfigList[0].Customer_Location;
                    RAM_Configuration.Note = FileManager.ConfigList[0].Note;
                    RAM_Configuration.ViewValim = FileManager.ConfigList[0].ViewValim;

                    ComSetup.ComPort = RAM_Configuration.ComPort;
                    ComSetup.Protocol = RAM_Configuration.Protocol;
                    ComSetup.ComPort608 = RAM_Configuration.ComPort608;
                    ComSetup.Protocol608 = RAM_Configuration.Protocol608;
                    ComSetup.Address = RAM_Configuration.MbAddress;
                    ComSetup.BaudRate = RAM_Configuration.MbBaudRate;
                    ComSetup.DataBits = RAM_Configuration.MbDataBits;
                    ComSetup.Parity = RAM_Configuration.MbParity;
                    ComSetup.StopBits = RAM_Configuration.MbStopBits;
                    ComSetup.TimeOut = RAM_Configuration.MbTimeOut;
                }
            }
            catch
            {

            }            
        }

        #endregion

        #region Verificator Config File

        private string GetVersion()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void LoadVerifConfigFile()
        {

            RAM_VerifConfiguration.SW_Ver_Verificator = GetVersion();

            List<VerificatorConfig> ConfigList = new List<VerificatorConfig>();

            ConfigList = await SerializableStorage<VerificatorConfig>.Load(FileManager.VerificatorConfigFile, FileManager.MainFolder.Path);

            if (ConfigList.Count > 0)
            {
                RAM_VerifConfiguration.TarMode              = ConfigList[0].TarMode;
                RAM_VerifConfiguration.DataCreazioneFile    = ConfigList[0].DataCreazioneFile;
                RAM_VerifConfiguration.DataLastTaratura     = ConfigList[0].DataLastTaratura;
                RAM_VerifConfiguration.DataNextTaratura     = ConfigList[0].DataNextTaratura;
                RAM_VerifConfiguration.SN_Verificator       = ConfigList[0].SN_Verificator;
                RAM_VerifConfiguration.Out4_20mA_Gain       = ConfigList[0].Out4_20mA_Gain;
                RAM_VerifConfiguration.Out4_20mA_Offs       = ConfigList[0].Out4_20mA_Offs;
                RAM_VerifConfiguration.Out4_12mA_Gain       = ConfigList[0].Out4_12mA_Gain;
                RAM_VerifConfiguration.Out4_12mA_Offs       = ConfigList[0].Out4_12mA_Offs;
                RAM_VerifConfiguration.Icoil_Gain           = ConfigList[0].Icoil_Gain;
                RAM_VerifConfiguration.Icoil_Offs           = ConfigList[0].Icoil_Offs;
                RAM_VerifConfiguration.VAlim_Gain           = ConfigList[0].VAlim_Gain;
                RAM_VerifConfiguration.VAlim_Offs           = ConfigList[0].VAlim_Offs;
                RAM_VerifConfiguration.Vbattery0            = ConfigList[0].Vbattery0;
                RAM_VerifConfiguration.Vbattery100          = ConfigList[0].Vbattery100;
            }
            else
            {
                VerificatorConfig DefaultConfig = new VerificatorConfig
                {
                    DataCreazioneFile = DateTime.Now
                };

                RAM_VerifConfiguration.TarMode              = DefaultConfig.TarMode;
                RAM_VerifConfiguration.DataCreazioneFile    = DefaultConfig.DataCreazioneFile;
                RAM_VerifConfiguration.DataLastTaratura     = DefaultConfig.DataLastTaratura;
                RAM_VerifConfiguration.DataNextTaratura     = DefaultConfig.DataNextTaratura;
                RAM_VerifConfiguration.SN_Verificator       = DefaultConfig.SN_Verificator;
                RAM_VerifConfiguration.Out4_20mA_Gain       = DefaultConfig.Out4_20mA_Gain;
                RAM_VerifConfiguration.Out4_20mA_Offs       = DefaultConfig.Out4_20mA_Offs;
                RAM_VerifConfiguration.Out4_12mA_Gain       = DefaultConfig.Out4_12mA_Gain;
                RAM_VerifConfiguration.Out4_12mA_Offs       = DefaultConfig.Out4_12mA_Offs;
                RAM_VerifConfiguration.Icoil_Gain           = DefaultConfig.Icoil_Gain;
                RAM_VerifConfiguration.Icoil_Offs           = DefaultConfig.Icoil_Offs;
                RAM_VerifConfiguration.VAlim_Gain           = DefaultConfig.VAlim_Gain;
                RAM_VerifConfiguration.VAlim_Offs           = DefaultConfig.VAlim_Offs;
                RAM_VerifConfiguration.Vbattery0            = DefaultConfig.Vbattery0;
                RAM_VerifConfiguration.Vbattery100          = DefaultConfig.Vbattery100;

                ConfigList.Add(DefaultConfig);

                SerializableStorage<VerificatorConfig>.Save(FileManager.VerificatorConfigFile, FileManager.MainFolder.Path, ConfigList);
            }

        }

        #endregion

        #region COMPortSettings

        public Settings ComSetup
        {
            get
            {
                return Settings.Instance;
            }
        }

        #endregion

        #region Connection

        public IrCOMPortManager IrConnection
        {
            get
            {
                return IrCOMPortManager.Instance;
            }
        }

        public MbCOMPortManager MbConnection
        {
            get
            {
                return MbCOMPortManager.Instance;
            }
        }

        public Configuration RAM_Configuration
        {
            get
            {
                return Configuration.Instance;
            }
            set { ; }
        }

        public VerificatorConfig RAM_VerifConfiguration
        {
            get
            {
                return VerificatorConfig.Instance;
            }
            set {; }
        }

        private string _connectionState;
        public string ConnectionState
        {
            get { return _connectionState; }
            set
            {
                if (value != _connectionState)
                {
                    _connectionState = value;
                    OnPropertyChanged("ConnectionState");
                }
            }
        }

        private async void IrConnection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (IrConnection.IrConnectionStatus)
                {
                    case IrCOMPortManager.IrConnectionStates.Stop:
                        ConnectionState = "Offline";                        
                        ConnectionStateVis.Fill = new SolidColorBrush(Colors.Red);
                        break;
                    case IrCOMPortManager.IrConnectionStates.Ping:
                        ConnectionState = "Ping...";                        
                        ConnectionStateVis.Fill = new SolidColorBrush(Colors.Yellow);
                        break;
                    case IrCOMPortManager.IrConnectionStates.Retry:
                    case IrCOMPortManager.IrConnectionStates.Working:
                        ConnectionState = "Online";
                        ConnectionStateVis.Fill = new SolidColorBrush(Colors.Green);
                        break;
                    case IrCOMPortManager.IrConnectionStates.Ready:
                        ConnectionState = "Ready";
                        ConnectionStateVis.Fill = new SolidColorBrush(Colors.Yellow);
                        break;
                    case IrCOMPortManager.IrConnectionStates.Connected:
                        ConnectionState = "Connected";
                        ConnectionStateVis.Fill = new SolidColorBrush(Colors.Green);
                        break;
                }
            });
        }

        private async void MbConnection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (MbConnection.MbConnectionStatus)
                {
                    case MbCOMPortManager.MbConnectionStates.Stop:
                        ConnectionState = "Offline";
                        ConnectionStateVis.Fill     = new SolidColorBrush(Colors.Red);
                        break;
                    case MbCOMPortManager.MbConnectionStates.Ready:
                        ConnectionState = "Ready";
                        ConnectionStateVis.Fill     = new SolidColorBrush(Colors.Yellow);
                        break;
                    case MbCOMPortManager.MbConnectionStates.Working:
                        ConnectionState = "Online";
                        ConnectionStateVis.Fill     = new SolidColorBrush(Colors.Green);
                        break;
                }
            });
        }

        #endregion

        #region DataBase

        public DataAccess Verificator
        {
            get
            {
                return DataAccess.Instance;
            }
        }

        private void InitDatabase()
        {
            DataAccess.InitializeDatabase("MC_Suite_DataBase.db");

            ReportLine LastReport = new ReportLine();
            Verificator.ReportList = DataAccess.GetData("MC_Suite_DataBase.db");
            if (Verificator.ReportList.Count != 0)
            {
                Verificator.ReportView.Clear();
                Verificator.ReportList.ForEach(p => Verificator.ReportView.Add(p));
            }
        }
        #endregion

        #region FileManager

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
            }
        }

        public CommonResources CommResources
        {
            get
            {
                return CommonResources.Instance;
            }
        }

        #endregion

        #region SysClock

        private string _sysDateTime;
        public string SysDateTime
        {
            get { return _sysDateTime; }
            set
            {
                if (value != _sysDateTime)
                {
                    _sysDateTime = value;
                    OnPropertyChanged("SysDateTime");
                }
            }
        }

        #endregion

        #region Shutdown

        public ICommand ShutDownCmd
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    ShutDown();
                });
            }
        }
        
        public ICommand RestartCmd
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    Restart();
                });
            }
        }        

        private void ShutDown()
        {
            ShutdownMessage.Text = "Shutting Down...";
            PowerMessage.Visibility = Visibility.Visible;
            Windows.System.ShutdownManager.BeginShutdown(Windows.System.ShutdownKind.Shutdown, TimeSpan.FromSeconds(5)); //Delay is not relevant to shutdown           
        }

        private void Restart()
        {
            ShutdownMessage.Text = "Restart...";
            PowerMessage.Visibility = Visibility.Visible;
            Windows.System.ShutdownManager.BeginShutdown(Windows.System.ShutdownKind.Restart, TimeSpan.FromSeconds(1)); //Delay before restart after shutdown
        }

        #endregion      

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

    }
}
