using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC_Suite.Euromag.Protocols;
using MC_Suite.Euromag.Protocols.CommunicationFrames;
using MC_Suite.Euromag.Protocols.StdCommands;
using MC_Suite.Properties;
using MC_Suite.Services;
using Windows.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;


namespace MC_Suite.Views
{
    class LogLinesDownloader<T>
            where T : IFrameable, ILogRecord, new()
    {
        public enum Direction
        {
            Newer,
            Older
        }

        public LogLinesDownloader(uint first, uint num, Direction dir, IList<DataLogLine> list, EventHandler completedHandler)
        {
            if (num == 0)
                throw new ArgumentException("num: can't be 0.");

            direction = dir;
            if (dir == Direction.Newer)
            {
                _current = first;
                _last = first + (uint)num - 1;
            }
            else
            {
                _current = (first > num) ? first - num : 0;
                _last = first;
            }
            _list = list;

            Completed += completedHandler;

            if (DownloadTimer == null)
                InitUpdateTimer();

            DownloadTimer.Stop();

            Start();
        }

        public void Abort()
        {
            abortRequest = true;
        }

        protected event EventHandler Completed;

        protected virtual void OnCompleted(EventArgs e)
        {
            if (Completed != null)
                Completed(this, e);
        }

        private void Start()
        {
            TargetVariablesFields.Instance.DownloadRingVisibility = Visibility.Visible;

            cmd = StdCommandLister.GetList().FirstOrDefault(cmd1 => cmd1 is GetLogLines<DataLogLine>) as GetLogLines<DataLogLine>;

            if (cmd == null)
                throw new ArgumentException(string.Format("There's no GetLogLines<{0}> command.", typeof(DataLogLine)));

            cmd.setPortHandler(IrCOMPortManager.Instance.portHandler);
            cmd.CommandCompleted += Cmd_CommandCompleted;
            cmd.StartLine = _current;
            cmd.Lines = GetLines();
            cmd.send();
        }

        private void Continue()
        {
            if (_current >= _last || abortRequest)
            {
                OnCompleted(EventArgs.Empty);
                abortRequest = false;
                TargetVariablesFields.Instance.DownloadRingVisibility = Visibility.Collapsed;
                return;
            }

            cmd.Lines = GetLines();
            cmd.StartLine = _current;
            cmd.send();
        }

        private DispatcherTimer DownloadTimer;
        private void InitUpdateTimer()
        {
            DownloadTimer = new DispatcherTimer();
            DownloadTimer.Interval = TimeSpan.FromSeconds(1);
            DownloadTimer.Tick += DownloadTimer_Tick;
            DownloadTimer.Stop();
        }

        private void DownloadTimer_Tick(object sender, object e)
        {
            DownloadTimer.Stop();
            Continue();
        }

        private IEnumerable<DataLogLine> OrderedList;
        private void Cmd_CommandCompleted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            GetLogLines<DataLogLine> cmd = sender as GetLogLines<DataLogLine>;
            if (cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
            {
                _current = cmd.StartLine;

                foreach(DataLogLine Line in _list)
                {
                    _tmplist.Add(Line);
                }

                foreach (DataLogLine item in cmd.logLines)
                {
                    /*if (CommonResources.Instance.TempUnit)
                        item.PCBtemperature = CommonResources.Instance.CelsiusToFahreneith(item.PCBtemperature);

                    item.TotalPositive = CommonResources.Instance.TotalizerConverter(item.TotalPositive, CommonResources.Instance.LogsUnit);
                    item.TotalNegative = CommonResources.Instance.TotalizerConverter(item.TotalNegative, CommonResources.Instance.LogsUnit);
                    item.Flow = CommonResources.Instance.FlowrateConverter(item.Flow, CommonResources.Instance.LogsFlowUnit,
                                                                                               CommonResources.Instance.LogsFlowTimebase);*/
                    _tmplist.Add(item);                    
                    _current++;
                }

                OrderedList = _tmplist.OrderByDescending(Line => Line.RowNumber);
                _list.Clear();

               foreach(DataLogLine Line in OrderedList)
                {
                    _list.Add(Line);
                }

                _tmplist.Clear();

                /*IrCOMPortManager.Instance.ExtCommandCompleted = IrCOMPortManager.CommandState.WaitForNew;
                IrCOMPortManager.Instance.CommandList.Remove(cmd);
                IrCOMPortManager.Instance.SuccessCounter += 1;*/

                DownloadTimer.Start();               
            }
        }

        private byte GetLines()
        {
            return (byte)(_current + cmd.MaxLines < _last ? cmd.MaxLines : _last - _current + 1); ;
        }

        private bool abortRequest = false;
        private GetLogLines<DataLogLine> cmd;
        private uint _current;
        private uint _last;
        private IList<DataLogLine> _list;
        private ObservableCollection<DataLogLine> _tmplist = new ObservableCollection<DataLogLine>();
        private Direction direction;
    }
}
