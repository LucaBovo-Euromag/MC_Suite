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
    class BatteryChartViewModel : INotifyPropertyChanged
    {
        private bool isMaxCountReached = false;
        private int maxNumberOfVisualPoints;
        private double interval;
        private static bool Running;

        private static BatteryChartViewModel _instance;
        public static BatteryChartViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BatteryChartViewModel();
                return _instance;
            }
        }

        public ObservableCollection<BatteryGraphValue> BatteryGraphDataCollection
        {
            get { return GraphData.Instance.BatteryGraph; }
            set
            {
                if (value != GraphData.Instance.BatteryGraph)
                {
                    GraphData.Instance.BatteryGraph = value;
                    OnPropertyChanged("BatteryGraphDataCollection");
                }
            }
        }

        public GraphData BatteryData
        {
            get
            {
                return GraphData.Instance;
            }
        }

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
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

        private string SavingFile;
        private void StartStopRecord()
        {
            if (GraphData.Instance.GraphRecording == GraphData.GraphModes.Recording)
            {
                GraphStatus = "";
                GraphData.Instance.GraphRecording = GraphData.GraphModes.Stop;
            }
            else
            {
                SavingFile = string.Format("{0}_{1}.{2}", "BatteryUsage", DateTime.Now.ToString("MM_dd_yyyy"), "csv");
                GraphStatus = "Recording...";
                GraphData.Instance.GraphRecording = GraphData.GraphModes.Recording;
            }
        }

        public DispatcherTimer SampleTimer { get; set; }

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
                }
            }
        }

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


        private void RestartSampleTimer(double interval)
        {
            this.isMaxCountReached = false;
            this.SampleTimer.Stop();
            this.SampleTimer.Interval = TimeSpan.FromMinutes(this.interval);
            this.BatteryGraphDataCollection.Clear();
            this.SampleTimer.Start();
            Running = true;
        }

        public void BatteryChartInit()
        {
            if (BatteryGraphDataCollection == null)
            {
                this.BatteryGraphDataCollection = new ObservableCollection<BatteryGraphValue>();
                BatteryGraphDataCollection.Add(new BatteryGraphValue() { Value = BatteryData.BatteryPercValue, Time = DateTime.Now });

                this.SampleTimer = new DispatcherTimer();
                this.SampleTimer.Interval = TimeSpan.FromMinutes(5);
                this.MaxNumberOfVisualPoints = 96; //Profondità grafico: 8h
                this.SampleTimer.Tick += SampleTimer_Tick;
                this.SampleTimer.Start();

                GraphData.Instance.GraphRecording = GraphData.GraphModes.Off;   //Non registra su file
                //GraphData.Instance.GraphRecording = GraphData.GraphModes.Stop;   //Registra su file
            }
        }

        public BatteryChartViewModel()
        {

        }

        private async void SampleTimer_Tick(object sender, object e)
        {
            string FileGraphDataStr;
            char[] FileGraphData;
            char NewLine = '\r';

            if (this.isMaxCountReached)
            {
                if (GraphData.Instance.GraphRecording == GraphData.GraphModes.Recording)
                    StartStopRecord();
                this.BatteryGraphDataCollection.RemoveAt(0);
            }
            else
            {
                if (GraphData.Instance.GraphRecording == GraphData.GraphModes.Stop)
                    StartStopRecord();
                this.isMaxCountReached = this.BatteryGraphDataCollection.Count == this.MaxNumberOfVisualPoints;
            }                

            if (BatteryData.Charging)
            {
                BatteryGraphDataCollection.Add(new BatteryGraphValue() { Value = 100, Time = DateTime.Now });
            }
            else
            {
                BatteryGraphDataCollection.Add(new BatteryGraphValue() { Value = BatteryData.BatteryPercValue, Time = DateTime.Now });                        
            }

            if (GraphData.Instance.GraphRecording == GraphData.GraphModes.Recording)
            {
                FileGraphDataStr = DateTime.Now.ToString() + ";" + BatteryData.BatteryPercValue.ToString() +
                                   ";" + AnalogsService.Instance.Analogiche[AnalogsService.VAlim].Misura.ToString(".##") +  NewLine;

                FileGraphData = FileGraphDataStr.ToCharArray();
                await BinaryStorage.Append(SavingFile, FileManager.CurrentFolder.Path, FileGraphData);
            }
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
    }
}
