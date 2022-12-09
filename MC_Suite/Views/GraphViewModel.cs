using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;
using MC_Suite.Services;
using MC_Suite.Properties;
using MC_Suite.Modbus;
using System.Runtime.CompilerServices;


namespace MC_Suite.Views
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        private bool isMaxCountReached = false;
        private int maxNumberOfVisualPoints;
        private double interval;

        private static GraphViewModel _instance;
        public static GraphViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GraphViewModel();
                return _instance;
            }
        }



        public ObservableCollection<GraphValue> GraphDataCollection
        {
            get { return GraphData.Instance.Graph; }
            set
            {
                if (value != GraphData.Instance.Graph)
                {
                    GraphData.Instance.Graph = value;
                    OnPropertyChanged("GraphDataCollection");
                }
            }
        }

        public DispatcherTimer Timer { get; set; }

        public Settings ComSetup
        {
            get
            {
                return Settings.Instance;
            }
        }

        public MC608 MC608_Device
        {
            get
            {
                return MC608.Instance;
            }
        }

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
            }
        }

        public MbCOMPortManager MbConnection
        {
            get
            {
                return MbCOMPortManager.Instance;
            }
        }

        public GPIO_Device GPIO_Control
        {
            get
            {
                return GPIO_Device.Instance;
            }
        }


        private ICommand _startStopRecordCmd;
        public ICommand StartStopRecordCmd
        {
            get
            {
                if (_startStopRecordCmd == null)
                {
                    _startStopRecordCmd = new RelayCommand(
                        param => StartStopRecord()
                            );
                }
                return _startStopRecordCmd;
            }
        }

        private void StartStopRecord()
        {
            if (GraphData.Instance.GraphRecording == GraphData.GraphModes.Recording)
            { 
                GraphStatus = "";
                GraphData.Instance.GraphRecording = GraphData.GraphModes.Stop;
            }
            else
            {
                FileManager.SavedFileName = string.Format("{0}_{1}.{2}", "BatteryUsage", DateTime.Now.ToString("MM_dd_yyyy_hh_mm"), "csv");
                GraphStatus = "Recording...";
                GraphData.Instance.GraphRecording = GraphData.GraphModes.Recording;
            }
        }

        private static bool Running;

        private string _graphStatus;
        public string GraphStatus
        {
            get { return _graphStatus; }
            set
            {
                if (value != _graphStatus)
                {
                    _graphStatus = value;
                    OnPropertyChanged("GraphStatus");
                }
            }
        }

        public double Interval
        {
            get
            {
                return this.interval;
            }
            set
            {
                if (this.interval != value)
                {
                    this.interval = value;
                    this.RestartTimer(this.interval);
                }
            }
        }

        public int MaxNumberOfVisualPoints
        {
            get
            {
                return this.maxNumberOfVisualPoints;
            }
            set
            {
                if (this.maxNumberOfVisualPoints != value)
                {
                    this.maxNumberOfVisualPoints = value;
                    this.RestartTimer(this.interval);
                }
            }
        }

        private ICommand _restartGraphCmd;
        public ICommand RestartGraphCmd
        {
            get
            {
                if (_restartGraphCmd == null)
                {
                    _restartGraphCmd = new RelayCommand(
                        param => RestartTimer(this.interval)
                            );
                }
                return _restartGraphCmd;
            }
        }

        private void RestartTimer(double interval)
        {
            this.isMaxCountReached = false;
            this.Timer.Stop();
            this.Timer.Interval = TimeSpan.FromSeconds(this.interval);
            this.GraphDataCollection.Clear();
            this.Timer.Start();
        }

        private ICommand _startGraphCmd;
        public ICommand StartGraphCmd
        {
            get
            {
                if (_startGraphCmd == null)
                {
                    _startGraphCmd = new RelayCommand(
                        param => StartGraph()
                            );
                }
                return _startGraphCmd;
            }
        }

        private void StartGraph()
        {
            Running = true;
            Settings.Instance.UpdateRunning = true;
        }

        private ICommand _stopGraphCmd;
        public ICommand StopGraphCmd
        {
            get
            {
                if (_stopGraphCmd == null)
                {
                    _stopGraphCmd = new RelayCommand(
                        param => StopGraph()
                            );
                }
                return _stopGraphCmd;
            }
        }

        private void StopGraph()
        {
            Running = false;
            Settings.Instance.UpdateRunning = false;
        }

        private string _statusMessageTxt;
        public string StatusMessageTxt
        {
            get { return _statusMessageTxt; }
            set
            {
                if (value != _statusMessageTxt)
                {
                    _statusMessageTxt = value;
                    OnPropertyChanged("StatusMessageTxt");
                }
            }
        }


        private ICommand _setTestModeCmd;
        public ICommand SetTestModeCmd
        {
            get
            {
                if (_setTestModeCmd == null)
                {
                    _setTestModeCmd = new RelayCommand(
                        param => SetTestMode()
                            );
                }
                return _setTestModeCmd;
            }
        }

        private void SetTestMode()
        {
            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;

            GPIO_Control.SetVAux();
            GPIO_Control.ResRL_4_20mA();

            StatusMessageTxt = "Wait...";
            Out420mALevel = "0";
        }

        private void MbConnection_SendCommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;

            if (cmd.SendCommand_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                StatusMessageTxt = "Ok";                
            }
            else
            {
                StatusMessageTxt = "Fail";
            }
        }

        private string _out420mALevel;
        public string Out420mALevel
        {
            get { return _out420mALevel; }
            set
            {
                if (value != _out420mALevel)
                {
                    _out420mALevel = value;
                    OnPropertyChanged("Out420mALevel");
                }
            }
        }

        private string _out420mAMeas;
        public string Out420mAMeas
        {
            get { return _out420mAMeas; }
            set
            {
                if (value != _out420mAMeas)
                {
                    _out420mAMeas = value;
                    OnPropertyChanged("Out420mAMeas");
                }
            }
        }


        private ICommand _set420mALevelCmd;
        public ICommand Set420mALevelCmd
        {
            get
            {
                if (_set420mALevelCmd == null)
                {
                    _set420mALevelCmd = new RelayCommand(
                        param => Set420mALevel()
                            );
                }
                return _set420mALevelCmd;
            }
        }

        private void Set420mALevel()
        {
            byte OutLev = Convert.ToByte(Out420mALevel);

            OutLev += 10;
            if (OutLev > 100)
                OutLev = 0;

            Out420mALevel = OutLev.ToString();

            StatusMessageTxt = "Wait...";

            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, OutLev, 3);
            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
        }


        public GraphViewModel()
        {
            if(GraphDataCollection == null)
            { 
                this.GraphDataCollection = new ObservableCollection<GraphValue>();
                GraphDataCollection.Add(new GraphValue() { Value = AnalogsService.Instance.Analogiche[AnalogsService.Out4_20mA].Misura, DateTime = DateTime.Now });
            }

            if (GraphData.Instance.GraphRecording == GraphData.GraphModes.Recording)
                GraphStatus = "Recording...";

            this.Timer = new DispatcherTimer();
            this.Interval = 2; 
            this.MaxNumberOfVisualPoints = 120;
            this.Timer.Tick += Timer_Tick;
            this.Timer.Start();
            
            Running = true;            
        }

        public char NewLine = '\r';
        async void Timer_Tick(object sender, object e)
        {
            GraphValue NewPoint;
            string FileGraphDataStr;
            char[] FileGraphData;

            if (Running)
            {
                Out420mAMeas = AnalogsService.Instance.Analogiche[AnalogsService.Out4_20mA].Misura.ToString("#.000");

                if (this.isMaxCountReached)
                {
                    this.GraphDataCollection.RemoveAt(0);
                    NewPoint = new GraphValue() { Value = AnalogsService.Instance.Analogiche[AnalogsService.Out4_20mA].Misura, DateTime = DateTime.Now };
                }
                else
                {
                    NewPoint = new GraphValue() { Value = AnalogsService.Instance.Analogiche[AnalogsService.Out4_20mA].Misura, DateTime = DateTime.Now };
                    this.isMaxCountReached = this.GraphDataCollection.Count == this.MaxNumberOfVisualPoints;
                }

                GraphDataCollection.Add(NewPoint);

                if (GraphData.Instance.GraphRecording == GraphData.GraphModes.Recording)
                {
                    FileGraphDataStr = NewPoint.DateTime.ToString() + ";" + NewPoint.Value.ToString() + NewLine;
                    FileGraphData = FileGraphDataStr.ToCharArray();
                    await BinaryStorage.Append(FileManager.SavedFileName, FileManager.CurrentFolder.Path, FileGraphData);
                }
            }
        }


        private ICommand _loadGraphFileCmd;
        public ICommand LoadGraphFileCmd
        {
            get
            {
                if (_loadGraphFileCmd == null)
                {
                    _loadGraphFileCmd = new RelayCommand(
                        param => LoadGraphFile()
                            );
                }
                return _loadGraphFileCmd;
            }
        }

        private async void LoadGraphFile()
        {
            Running = false;

            this.GraphDataCollection.Clear();

            FileGraphData.Clear();

            FileGraphData = await SerializableStorage<GraphValue>.Load(FileManager.SavedFileName, FileManager.CurrentFolder.Path);
        }

        private static List<GraphValue> FileGraphData = new List<GraphValue>();
        private List<GraphValue> AppendList = new List<GraphValue>();

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
