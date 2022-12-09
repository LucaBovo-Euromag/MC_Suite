using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
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
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Uwp;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MC406MainPage : Page
    {
        public MC406 MC406_Device;

        #region Istanze

        public TargetVariablesFields Fields
        {
            get
            {
                return TargetVariablesFields.Instance;
            }
        }

        public IrCOMPortManager IrConnection
        {
            get
            {
                return IrCOMPortManager.Instance;
            }
        }

        #endregion         

        public MC406MainPage()
        {
            this.InitializeComponent();

            InitReadTimer();

            Settings.Instance.UpdateRunning = true;

            //IrConnection.PropertyChanged                += IrConnection_PropertyChanged;

            Fields.FlowRateTU.PropertyChanged           += FlowRateTU_PropertyChanged;
            Fields.FlowRatePERC.PropertyChanged         += FlowRatePERC_PropertyChanged;
            Fields.FlowRateMS.PropertyChanged           += FlowRateMS_PropertyChanged;
            Fields.TotalPositiveM3.PropertyChanged      += TotalPositiveM3_PropertyChanged;
            Fields.TotalNegativeM3.PropertyChanged      += TotalNegativeM3_PropertyChanged;
            Fields.PartialPositiveM3.PropertyChanged    += PartialPositiveM3_PropertyChanged;
            Fields.PartialNegativeM3.PropertyChanged    += PartialNegativeM3_PropertyChanged;

            Fields.FwRevision.PropertyChanged           += FwRevision_PropertyChanged;
            Fields.FwVersion.PropertyChanged            += FwVersion_PropertyChanged;

            /*if (IrConnection.IrConnectionStatus == IrCOMPortManager.IrConnectionStates.Stop)
                OfflineVisibility();
            else
                OfflineVisibility();*/

            if (IrConnection.IrConnectionStatus == IrCOMPortManager.IrConnectionStates.Connected)
            { 
                OnlineVisibility();
                BundleAutoReadStart();
            }
            else
                OfflineVisibility();
        }

        #region FW Version and revision

        private async void FwVersion_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Fields.FwRelease = Fields.FwVersion.ValAsString + "." + Fields.FwRevision.ValAsString;
            });
        }

        private async void FwRevision_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Fields.FwRelease = Fields.FwVersion.ValAsString + "." + Fields.FwRevision.ValAsString;
            });
        }

        private string _fwVersion_Loc;
        public string FwVersion_Loc
        {
            get { return _fwVersion_Loc; }
            set
            {
                if (value != _fwVersion_Loc)
                {
                    _fwVersion_Loc = value;
                    OnPropertyChanged("FwVersion_Loc");
                }
            }
        }

        private string _fwRevision_Loc;
        public string FwRevision_Loc
        {
            get { return _fwRevision_Loc; }
            set
            {
                if (value != _fwRevision_Loc)
                {
                    _fwRevision_Loc = value;
                    OnPropertyChanged("FwRevision_Loc");
                }
            }
        }
        #endregion

        #region FlowrateTechUnit

        private string _flowrateTechUnit_Loc;
        public string FlowrateTechUnit_Loc
        {
            get { return _flowrateTechUnit_Loc; }
            set
            {
                if (value != _flowrateTechUnit_Loc)
                {
                    _flowrateTechUnit_Loc = value;
                    OnPropertyChanged("FlowrateTechUnit_Loc");
                }
            }
        }

        #endregion

        #region FlowRateTU

        public int FlowMaxDigit = 5;
        private async void FlowRateTU_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                byte ndig = Fields.FlowrateNdigit.Value;
                string StrValue = Convert.ToString(Fields.FlowRateTU.Value, System.Globalization.CultureInfo.InvariantCulture);
                if (Fields.FlowRateTU.Value == 0)
                {
                    switch (ndig)
                    {
                        case 0:
                            Fields.FlowRateTU_str = StrValue;
                            break;
                        case 1:
                            Fields.FlowRateTU_str = StrValue + ".0";
                            break;
                        case 2:
                            Fields.FlowRateTU_str = StrValue + ".00";
                            break;
                        case 3:
                            Fields.FlowRateTU_str = StrValue + ".000";
                            break;
                    }
                }
                else
                {
                    int CommaIndex = StrValue.IndexOf(".");
                    if (ndig == 0)
                    {
                        Fields.FlowRateTU_str = StrValue.Substring(0, CommaIndex);
                    }
                    else
                    {
                        int ValueLenght = StrValue.Length;
                        byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                        if (DecimalDigits < ndig)
                            ndig = DecimalDigits;

                        if (CommaIndex <= FlowMaxDigit)
                            Fields.FlowRateTU_str = StrValue.Substring(0, CommaIndex + 1 + ndig);
                        else
                            Fields.FlowRateTU_str = StrValue.Substring(0, CommaIndex);
                    }
                }

                FlowRateFullScale_Converter();
            });
        }

        #endregion

        #region FlowRatePER

        private async void FlowRatePERC_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                byte ndig = Fields.FlowrateNdigit.Value;
                string StrValue = Convert.ToString(Fields.FlowRatePERC.Value, System.Globalization.CultureInfo.InvariantCulture);
                if (Fields.FlowRatePERC.Value == 0)
                {
                    switch (ndig)
                    {
                        case 0:
                            Fields.FlowRatePERC_str = StrValue;
                            break;
                        case 1:
                            Fields.FlowRatePERC_str = StrValue + ".0";
                            break;
                        case 2:
                            Fields.FlowRatePERC_str = StrValue + ".00";
                            break;
                        case 3:
                            Fields.FlowRatePERC_str = StrValue + ".000";
                            break;
                    }
                }
                else
                {
                    int CommaIndex = StrValue.IndexOf(".");
                    if (ndig == 0)
                    {
                        Fields.FlowRatePERC_str = StrValue.Substring(0, CommaIndex);
                    }
                    else
                    {
                        int ValueLenght = StrValue.Length;
                        byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                        if (DecimalDigits < ndig)
                            ndig = DecimalDigits;

                        if (CommaIndex <= FlowMaxDigit)
                            Fields.FlowRatePERC_str = StrValue.Substring(0, CommaIndex + 1 + ndig);
                        else
                            Fields.FlowRatePERC_str = StrValue.Substring(0, CommaIndex);
                    }
                }
            });
        }

        #endregion

        #region FlowRateMS

        private async void FlowRateMS_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                byte ndig = Fields.FlowrateNdigit.Value;
                string StrValue = Convert.ToString(Fields.FlowRateMS.Value, System.Globalization.CultureInfo.InvariantCulture);
                if (Fields.FlowRateMS.Value == 0)
                {
                    switch (ndig)
                    {
                        case 0:
                            Fields.FlowRateMS_str = StrValue;
                            break;
                        case 1:
                            Fields.FlowRateMS_str = StrValue + ".0";
                            break;
                        case 2:
                            Fields.FlowRateMS_str = StrValue + ".00";
                            break;
                        case 3:
                            Fields.FlowRateMS_str = StrValue + ".000";
                            break;
                    }
                }
                else
                {
                    int CommaIndex = StrValue.IndexOf(".");
                    if (ndig == 0)
                    {
                        Fields.FlowRateMS_str = StrValue.Substring(0, CommaIndex);
                    }
                    else
                    {
                        int ValueLenght = StrValue.Length;
                        byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                        if (DecimalDigits < ndig)
                            ndig = DecimalDigits;

                        if (CommaIndex <= FlowMaxDigit)
                            Fields.FlowRateMS_str = StrValue.Substring(0, CommaIndex + 1 + ndig);
                        else
                            Fields.FlowRateMS_str = StrValue.Substring(0, CommaIndex);
                    }
                }
            });
        }

        #endregion

        #region TotalPositive

        public int TotalizerMaxDigit = 8;

        void TotalPositiveM3_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                string StrValue = Convert.ToString(Fields.TotalPositiveM3.Value, System.Globalization.CultureInfo.InvariantCulture);
                byte ndig = Fields.AccumulatorNdigit.Value;
                if (Fields.TotalPositiveM3.Value == 0)
                {
                    switch (ndig)
                    {
                        case 0:
                            Fields.TotalPositiveM3_str = StrValue;
                            break;
                        case 1:
                            Fields.TotalPositiveM3_str = StrValue + ".0";
                            break;
                        case 2:
                            Fields.TotalPositiveM3_str = StrValue + ".00";
                            break;
                        case 3:
                            Fields.TotalPositiveM3_str = StrValue + ".000";
                            break;
                    }
                }
                else
                {
                    int CommaIndex = StrValue.IndexOf(".");

                    if (ndig == 0)
                    {
                        int ValueLenght = StrValue.Length;
                        int IntValue = (ValueLenght - (ValueLenght - CommaIndex));
                        if (IntValue <= TotalizerMaxDigit)
                            Fields.TotalPositiveM3_str = StrValue.Substring(0, IntValue);
                        else
                            Fields.TotalPositiveM3_str = StrValue.Substring(IntValue - TotalizerMaxDigit, TotalizerMaxDigit);
                    }
                    else
                    {
                        int ValueLenght = StrValue.Length;
                        byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                        if (DecimalDigits < ndig)
                            ndig = DecimalDigits;

                        if (CommaIndex <= TotalizerMaxDigit)
                            Fields.TotalPositiveM3_str = StrValue.Substring(0, CommaIndex + 1 + ndig);
                        else
                            Fields.TotalPositiveM3_str = StrValue.Substring(ValueLenght - 8, CommaIndex);
                    }
                }
            }
        }

        #endregion

        #region TotalNegative

        void TotalNegativeM3_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                string StrValue = Convert.ToString(Fields.TotalNegativeM3.Value, System.Globalization.CultureInfo.InvariantCulture);
                byte ndig = Fields.AccumulatorNdigit.Value;
                if (Fields.TotalNegativeM3.Value == 0)
                {
                    switch (ndig)
                    {
                        case 0:
                            Fields.TotalNegativeM3_str = StrValue;
                            break;
                        case 1:
                            Fields.TotalNegativeM3_str = StrValue + ".0";
                            break;
                        case 2:
                            Fields.TotalNegativeM3_str = StrValue + ".00";
                            break;
                        case 3:
                            Fields.TotalNegativeM3_str = StrValue + ".000";
                            break;
                    }
                }
                else
                {
                    int CommaIndex = StrValue.IndexOf(".");

                    if (ndig == 0)
                    {
                        int ValueLenght = StrValue.Length;
                        int IntValue = (ValueLenght - (ValueLenght - CommaIndex));
                        if (IntValue <= TotalizerMaxDigit)
                            Fields.TotalNegativeM3_str = StrValue.Substring(0, IntValue);
                        else
                            Fields.TotalNegativeM3_str = StrValue.Substring(IntValue - TotalizerMaxDigit, TotalizerMaxDigit);
                    }
                    else
                    {
                        int ValueLenght = StrValue.Length;
                        byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                        if (DecimalDigits < ndig)
                            ndig = DecimalDigits;

                        if (CommaIndex <= TotalizerMaxDigit)
                            Fields.TotalNegativeM3_str = StrValue.Substring(0, CommaIndex + 1 + ndig);
                        else
                            Fields.TotalNegativeM3_str = StrValue.Substring(ValueLenght - 8, CommaIndex);
                    }
                }
            }
        }

        #endregion

        #region PartialPositive

        void PartialPositiveM3_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                string StrValue = Convert.ToString(Fields.PartialPositiveM3.Value, System.Globalization.CultureInfo.InvariantCulture);
                byte ndig = Fields.AccumulatorNdigit.Value;
                if (Fields.PartialPositiveM3.Value == 0)
                {
                    switch (ndig)
                    {
                        case 0:
                            Fields.PartialPositiveM3_str = StrValue;
                            break;
                        case 1:
                            Fields.PartialPositiveM3_str = StrValue + ".0";
                            break;
                        case 2:
                            Fields.PartialPositiveM3_str = StrValue + ".00";
                            break;
                        case 3:
                            Fields.PartialPositiveM3_str = StrValue + ".000";
                            break;
                    }
                }
                else
                {
                    int CommaIndex = StrValue.IndexOf(".");

                    if (ndig == 0)
                    {
                        int ValueLenght = StrValue.Length;
                        int IntValue = (ValueLenght - (ValueLenght - CommaIndex));
                        if (IntValue <= TotalizerMaxDigit)
                            Fields.PartialPositiveM3_str = StrValue.Substring(0, IntValue);
                        else
                            Fields.PartialPositiveM3_str = StrValue.Substring(IntValue - TotalizerMaxDigit, TotalizerMaxDigit);
                    }
                    else
                    {
                        int ValueLenght = StrValue.Length;
                        byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                        if (DecimalDigits < ndig)
                            ndig = DecimalDigits;

                        if (CommaIndex <= TotalizerMaxDigit)
                            Fields.PartialPositiveM3_str = StrValue.Substring(0, CommaIndex + 1 + ndig);
                        else
                            Fields.PartialPositiveM3_str = StrValue.Substring(ValueLenght - 8, CommaIndex);
                    }
                }
            }
        }

        #endregion

        #region PartialNegative

        void PartialNegativeM3_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                string StrValue = Convert.ToString(Fields.PartialNegativeM3.Value, System.Globalization.CultureInfo.InvariantCulture);
                byte ndig = Fields.AccumulatorNdigit.Value;
                if (Fields.PartialNegativeM3.Value == 0)
                {
                    switch (ndig)
                    {
                        case 0:
                            Fields.PartialNegativeM3_str = StrValue;
                            break;
                        case 1:
                            Fields.PartialNegativeM3_str = StrValue + ".0";
                            break;
                        case 2:
                            Fields.PartialNegativeM3_str = StrValue + ".00";
                            break;
                        case 3:
                            Fields.PartialNegativeM3_str = StrValue + ".000";
                            break;
                    }
                }
                else
                {
                    int CommaIndex = StrValue.IndexOf(".");

                    if (ndig == 0)
                    {
                        int ValueLenght = StrValue.Length;
                        int IntValue = (ValueLenght - (ValueLenght - CommaIndex));
                        if (IntValue <= TotalizerMaxDigit)
                            Fields.PartialNegativeM3_str = StrValue.Substring(0, IntValue);
                        else
                            Fields.PartialNegativeM3_str = StrValue.Substring(IntValue - TotalizerMaxDigit, TotalizerMaxDigit);
                    }
                    else
                    {
                        int ValueLenght = StrValue.Length;
                        byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                        if (DecimalDigits < ndig)
                            ndig = DecimalDigits;

                        if (CommaIndex <= TotalizerMaxDigit)
                            Fields.PartialNegativeM3_str = StrValue.Substring(0, CommaIndex + 1 + ndig);
                        else
                            Fields.PartialNegativeM3_str = StrValue.Substring(0, CommaIndex);
                    }
                }
            }
        }

        #endregion

        #region FullScale

        private double _locFlowFullScale;
        public double LocFlowFullScale
        {
            get { return _locFlowFullScale; }
            set
            {
                if (value != _locFlowFullScale)
                {
                    _locFlowFullScale = value;
                    OnPropertyChanged("LocFlowFullScale");
                }
            }
        }

        public double kUT;
        public double kTB;
        public double FullScaleFactor;
        public const double PGRECO = 3.141592653589793;

        void FlowRateFullScale_Converter()
        {
            if ((Fields.FlowrateTimeBase.Value != 0) && (Fields.FlowrateFullscale.Value != 0))
            {
                switch (Fields.FlowrateTechUnit.Value)
                {
                    case (2): // "cubic meter [m³]"
                        kUT = 0.001;
                        break;
                    case (3): // "liter [L]"
                        kUT = 1.0;
                        break;
                    case (4): // "mega liter [ML]"
                        kUT = 0.000001;
                        break;
                    case (5): // "cubic foot [ft³]"
                        kUT = 0.0353147;
                        break;
                    case (6): // "US liquid gallon [gal]"
                        kUT = 0.264172;
                        break;
                    case (7): // "US Oil Barrel [BBL]"
                        kUT = 0.0062898107;
                        break;
                }

                switch (Fields.FlowrateTimeBase.Value)
                {
                    case (1): // "second [s]"
                        kTB = 0.0001;
                        break;
                    case (2): // "minute [m]"
                        kTB = 0.006;
                        break;
                    case (3): // "hour [h]"
                        kTB = 0.36;
                        break;
                    case (4): // "day [d]"
                        kTB = 8.64;
                        break;
                }

                if (Fields.FlowrateTechUnit.Value == 1)
                    FullScaleFactor = kTB / 0.0001; //se voglio i m/s non moltiplico per niente; scalo solo il fattore tempo
                else
                    FullScaleFactor = 2.5 * PGRECO * Fields.SensorDiameter.Value * Fields.SensorDiameter.Value * kUT * kTB;

                LocFlowFullScale = (Fields.FlowrateFullscale.Value / 10) * FullScaleFactor;

                byte ndig = Fields.FlowrateNdigit.Value;
                string StrValue = Convert.ToString(LocFlowFullScale, System.Globalization.CultureInfo.InvariantCulture);
                int CommaIndex = StrValue.IndexOf(".");
                if (ndig == 0)
                {
                    Fields.LocFlowFullScale_Str = StrValue.Substring(0, CommaIndex);
                }
                else
                {
                    int ValueLenght = StrValue.Length;
                    byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                    if (DecimalDigits < ndig)
                        ndig = DecimalDigits;

                    if (CommaIndex <= FlowMaxDigit)
                        Fields.LocFlowFullScale_Str = StrValue.Substring(0, CommaIndex + 1 + ndig);
                    else
                        Fields.LocFlowFullScale_Str = StrValue.Substring(0, CommaIndex);
                }
            }
        }


        #endregion

        #region DateTime

        private ICommand _readClock;
        public ICommand ReadClock
        {
            get
            {
                if (_readClock == null)
                {
                    _readClock = new RelayCommand(
                        param => GetTargetClock()
                            );
                }
                return _readClock;
            }
        }

        private void GetTargetClock()
        {
            GetTargetDateTime getClockCmd = new GetTargetDateTime(IrConnection.portHandler);
            getClockCmd.CommandCompleted += GetClockCmd_CommandCompleted;
            getClockCmd.send();
        }

        private async void GetClockCmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                GetTargetDateTime cmd = sender as GetTargetDateTime;
                if (cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
                {
                    Fields.ConvDateTime = cmd.Date_Time.Date;
                    CheckTargetClock();
                    ReadClockEn = false;
                    BundleReaded = true;
                }
            });
        }

        private void CheckTargetClock()
        {
            DateTime CurrentDateTime = DateTime.Now;

            //Controlla anno
            if (Fields.ConvDateTime.Year.Equals(CurrentDateTime.Year))
            {
                //Controlla Mese
                if (Fields.ConvDateTime.Month.Equals(CurrentDateTime.Month))
                {
                    if (Fields.ConvDateTime.Day.Equals(CurrentDateTime.Day))
                    {
                        if (Fields.ConvDateTime.Hour.Equals(CurrentDateTime.Hour))
                        {
                            ClockToUpdate.Visibility = Visibility.Collapsed;
                            return;
                        }
                    }
                }
            }
            ClockToUpdate.Visibility = Visibility.Visible;
        }

        #endregion

        private void OfflineVisibility()
        {
            Frontalino406.Visibility = Visibility.Collapsed;
            MC406_Display.Visibility = Visibility.Collapsed;

            OfflineMessage.Visibility = Visibility.Visible;
            LoadingMessage.Visibility = Visibility.Collapsed;
            MeasuresAndClock.Visibility = Visibility.Collapsed;
            InfoBox.Visibility = Visibility.Collapsed;
            ConnectionMessage = "Please Connect";
            InfoConnectionBox.Visibility = Visibility.Visible;
        }

        private void PingVisibility()
        {
            ConnectionMessage = "Connecting...";
            InfoConnectionBox.Visibility = Visibility.Collapsed;
        }

        private void LoadingVisibility()
        {
            OfflineMessage.Visibility = Visibility.Collapsed;
            LoadingMessage.Visibility = Visibility.Visible;
            ConnectionMessage = "Loading...";
        }

        private void OnlineVisibility()
        {
            Frontalino406.Visibility = Visibility.Visible;
            MC406_Display.Visibility = Visibility.Visible;
            OfflineMessage.Visibility = Visibility.Collapsed;
            LoadingMessage.Visibility = Visibility.Collapsed;
            MeasuresAndClock.Visibility = Visibility.Visible;
            InfoBox.Visibility = Visibility.Visible;
            Settings.Instance.ConverterSelection.MC406.ConfigEnabled = true;
            Settings.Instance.ConverterSelectionChanged = true;
        }

        #region IrConnection

        private DispatcherTimer ReadTimer;
        private void InitReadTimer()
        {
            ReadTimer = new DispatcherTimer();
            ReadTimer.Interval = TimeSpan.FromMilliseconds(2000);
            ReadTimer.Tick += ReadTimer_Tick;
            ReadTimer.Stop();
            LoadingBar.Value = 0;
        }

        private void BundleAutoReadStart()
        {
            BundleIndex = 0;
            BundleReaded = true;
            Settings.Instance.UpdateRunning = true;
            ReadTimer.Start();
        }

        private void BundleAutoReadStop()
        {
            BundleIndex = 0;
            BundleReaded = false;
            Settings.Instance.UpdateRunning = false;
            ReadTimer.Stop();
        }

        private bool Updated;
        private bool ReadClockEn;
        private byte ReadClockTimer;
        private void ReadTimer_Tick(object sender, object e)
        {
            if ((BundleReaded) && (Settings.Instance.VerificatorRunning == false))
            {
                if (BundleIndex < 6)
                {
                    BundleIndex++;
                    BundleAutoRead();
                    BundleReaded = false;
                    Updated = true;
                    ReadClockEn = true;
                    ReadClockTimer = 0;
                }
                else
                {
                    if (ReadClockTimer++ >= 60)
                    {
                        ReadClockEn = true;
                        ReadClockTimer = 0;
                    }

                    OnlineVisibility();

                    if (ReadClockEn)
                    {
                        GetTargetClock();
                        BundleReaded = false;
                    }
                    else if ((Updated) && (Settings.Instance.UpdateRunning))
                    {
                        if (Settings.Instance.OtherMeasuresChanged)
                            IrCOMPortManager.Instance.ReadBundle(2);
                        else
                            IrCOMPortManager.Instance.ReadBundle(1);
                        IrCOMPortManager.Instance.ReadBundleCompleted += Instance_ReadBundleCompleted;
                        Updated = false;
                    }
                }
            }
        }

        private void Instance_ReadBundleCompleted(object sender, PropertyChangedEventArgs e)
        {
            Settings.Instance.OtherMeasuresChanged = false;
            Updated = true;
        }

        private ICommand _bundleAutoReadCmd;
        public ICommand BundleAutoReadCmd
        {
            get
            {
                if (_bundleAutoReadCmd == null)
                {
                    _bundleAutoReadCmd = new RelayCommand(
                        param => BundleAutoReadStart()
                            );
                }
                return _bundleAutoReadCmd;
            }
        }

        private byte BundleIndex;
        private bool BundleReaded;
        private void BundleAutoRead()
        {
            IrConnection.ReadBundleCompleted += BundleAutoReadCompleted;
            IrConnection.ReadBundle(BundleIndex);
            ReadTimer.Stop();
        }

        private void BundleAutoReadCompleted(object sender, PropertyChangedEventArgs e)
        {
            IrCOMPortManager cmd = sender as IrCOMPortManager;
            BundleReaded = true;
            LoadingBar.Value = BundleIndex;
            ReadTimer.Start();
        }

        /*private IrCOMPortManager.IrConnectionStates memIrConnectionStatus;
        private async void IrConnection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (IrConnection.IrConnectionStatus)
                {
                    case IrCOMPortManager.IrConnectionStates.Stop:
                        BundleAutoReadStop();
                        OfflineVisibility();
                        memIrConnectionStatus = IrConnection.IrConnectionStatus;
                        break;
                    case IrCOMPortManager.IrConnectionStates.Ping:
                        PingVisibility();
                        break;
                    case IrCOMPortManager.IrConnectionStates.Retry:
                    case IrCOMPortManager.IrConnectionStates.Working:
                        OnlineVisibility();
                        if (memIrConnectionStatus == IrCOMPortManager.IrConnectionStates.Stop)
                        {
                            GetTargetClock();
                            memIrConnectionStatus = IrConnection.IrConnectionStatus;
                        }
                        break;
                    case IrCOMPortManager.IrConnectionStates.Ready:
                        LoadingVisibility();
                        if (memIrConnectionStatus == IrCOMPortManager.IrConnectionStates.Stop)
                        {
                            BundleAutoReadStart();
                            memIrConnectionStatus = IrConnection.IrConnectionStatus;
                        }
                        break;
                    case IrCOMPortManager.IrConnectionStates.Connected:
                        break;
                }
            });
        }*/

        private ICommand _loadingBundlesCmd;
        public ICommand LoadingBundlesCmd
        {
            get
            {
                if (_loadingBundlesCmd == null)
                {
                    _loadingBundlesCmd = new RelayCommand(
                        param => LoadingBundles()
                            );
                }
                return _loadingBundlesCmd;
            }
        }

        private void LoadingBundles()
        {
            LoadingVisibility();
            BundleAutoReadStart();
        }

        private string _connectionMessage;
        public string ConnectionMessage
        {
            get { return _connectionMessage; }
            set
            {
                if (value != _connectionMessage)
                {
                    _connectionMessage = value;
                    OnPropertyChanged("ConnectionMessage");
                }
            }
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
