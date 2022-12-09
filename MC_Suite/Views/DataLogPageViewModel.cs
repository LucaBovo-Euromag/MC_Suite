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

namespace MC_Suite.Views
{
    public class DataLogPageViewModel: INotifyPropertyChanged
    {
        private static DataLogPageViewModel _instance;
        public static DataLogPageViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataLogPageViewModel();
                return _instance;
            }
        }


        public DataLogPageViewModel()
        {
            DataLogMessage = "";
            Settings.Instance.UpdateRunning = false;
            Fields.DownloadRingVisibility = Visibility.Collapsed;
            Fields.LogLastRow.PropertyChanged += LogLastRow_PropertyChanged;
        }

        private void LogLastRow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LOG_LAST_ROW cmd = sender as LOG_LAST_ROW;
            UpdateLogs( cmd );
        }

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

        private string _dataLogMessage;
        public string DataLogMessage
        {
            get { return _dataLogMessage; }
            set
            {
                if (value != _dataLogMessage)
                {
                    _dataLogMessage = value;
                    OnPropertyChanged("DataLogMessage");
                }
            }
        }

        private ICommand _moreRecordsCommand;
        public ICommand MoreRecordsCommand
        {
            get
            {
                if (_moreRecordsCommand == null)
                {
                    _moreRecordsCommand = new RelayCommand(
                        param => MoreRecords(),
                        pram => (Fields.RowDatabase.Count != 0) && firstBlock == null && moreRowsBlock == null
                    );
                }
                return _moreRecordsCommand;
            }
        }

        private void MoreRecords()
        {
            uint oldest = (uint)Fields.RowDatabase.Count;
            moreRowsBlock = new LogLinesDownloader<DataLogLine>(oldest, 32, LogLinesDownloader<DataLogLine>.Direction.Older, Fields.RowDatabase, Block_Completed);
            blockSet.Add(moreRowsBlock);
        }

        private ICommand _clearAllRecords;
        public ICommand ClearAllRecords
        {
            get
            {
                if (_clearAllRecords == null)
                {
                    _clearAllRecords = new RelayCommand(
                        param => this.CheckUserConsentForClear(),
                        param => (Fields.RowDatabase.Count != 0)
                    );
                }
                return _clearAllRecords;
            }
        }

        private void CheckUserConsentForClear()
        {
            //StopConnectionManager();
            DataLogMessage = "Erase Data Log...";
            DataLogErase LogErase = new DataLogErase(IrConnection.portHandler);
            LogErase.CommandCompleted += LogErase_CommandCompleted;
            LogErase.send();
        }

        private void LogErase_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            DataLogErase cmd = sender as DataLogErase;
            if(cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
            {
                DataLogMessage = "Data Log Erased";
                Fields.RowDatabase.Clear();
                lock (lastLogRowLock)
                {
                    lastLogRow = 0;
                }
            }
            else
                DataLogMessage = "Data Log Erase Failed";
            //RestartConnectionManager();
        }

        private void StopConnectionManager()
        {
            foreach (var block in blockSet)
            {
                block.Abort();
            }

            IrConnection.Stop();
            Fields.LogLastRow.PropertyChanged -= LogLastRow_PropertyChanged;

            System.Threading.Thread.Sleep(1000);
        }

        private void RestartConnectionManager()
        {
            Fields.LogLastRow.PropertyChanged += LogLastRow_PropertyChanged;
            IrConnection.Start();
        }

        private void Block_Completed(object sender, EventArgs e)
        {
            blockSet.Remove(sender as LogLinesDownloader<DataLogLine>);
            if (sender == firstBlock)
                firstBlock = null;
            if (sender == moreRowsBlock)
                moreRowsBlock = null;

            OnPropertyChanged("ExportAllRecords");
            OnPropertyChanged("ClearAllRecords");
            OnPropertyChanged("MoreRecordsCommand");
        }

        private ICommand _downloadRecords;
        public ICommand DownloadRecords
        {
            get
            {
                if (_downloadRecords == null)
                {
                    _downloadRecords = new RelayCommand(
                        param => DownloadLogs(Fields.LogLastRow)
                            );
                }
                return _downloadRecords;
            }
        }

        private uint lastLogDownloaded;
        private void DownloadLogs(LOG_LAST_ROW cmd)
        {
            if (cmd == null)
                return;

            if(lastLogDownloaded != 0)
            {
                lastLogDownloaded = lastLogDownloaded - 40;
            }
            else
            {
                lastLogDownloaded = cmd.Value + 1;
            }

            firstBlock = new LogLinesDownloader<DataLogLine>(lastLogDownloaded - 1, 40, LogLinesDownloader<DataLogLine>.Direction.Older, Fields.RowDatabase, Block_Completed);
            blockSet.Add(firstBlock);
        }

        private void UpdateLogs(LOG_LAST_ROW cmd)
        {
            if (cmd == null)
                return;

            bool firstTime = false;
            uint lastLogRowCopy = 0;
            lock (lastLogRowLock)
            {
                if (lastLogRow == cmd.Value)
                    return;

                if (lastLogRow == 0)
                    firstTime = true;

                lastLogRow = cmd.Value;
                lastLogRowCopy = lastLogRow;
            }

            if (firstTime)
            {
                firstBlock = new LogLinesDownloader<DataLogLine>(lastLogRowCopy - 1, 40, LogLinesDownloader<DataLogLine>.Direction.Older, Fields.RowDatabase, Block_Completed);
                blockSet.Add(firstBlock);
            }
            else
            {
                blockSet.Add(new LogLinesDownloader<DataLogLine>(lastLogRowCopy - 1, 1, LogLinesDownloader<DataLogLine>.Direction.Newer, Fields.RowDatabase, Block_Completed));
            }
        }

        private LogLinesDownloader<DataLogLine> firstBlock;
        private LogLinesDownloader<DataLogLine> moreRowsBlock;
        private HashSet<LogLinesDownloader<DataLogLine>> blockSet = new HashSet<LogLinesDownloader<DataLogLine>>();
        private object lastLogRowLock = new object();
        private uint lastLogRow = 0;

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
