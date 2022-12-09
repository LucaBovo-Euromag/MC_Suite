using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Runtime.CompilerServices;

using Windows.UI.Popups;
using MC_Suite.Euromag.Protocols;
using MC_Suite.Euromag.Protocols.StdCommands;
using MC_Suite.Properties;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MC_Suite.Services
{
    public class ConnectionManager : INotifyPropertyChanged
    {
        private static ConnectionManager _instance;
        public static ConnectionManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConnectionManager();
                return _instance;
            }
        }

        public async void Start()
        {
            if (Status != ConnectionStatus.offline)
                Stop();

            if ((Settings.Instance.ComPort == null) || (Settings.Instance.ComPort.ID == String.Empty))
            {
                var dialog = new MessageDialog("Please select COM port: OPTIONS->Settings->Connection", "COM Port not selected");
                await dialog.ShowAsync();
            }
            else
            {
                _continueWork = true;
                commThread = new Thread(new ThreadStart(this.ProcessQueue));
                commThread.Start();

                Settings.Instance.portHandler = new commPortHandler(Settings.Instance.ComPort.ID); 

                if  ( await Settings.Instance.portHandler.open() )
                {
                    GoToPing();
                    //var dialog = new MessageDialog(Settings.Instance.ComPort.Name + " Port Opened");
                    //await dialog.ShowAsync();
                }
                else
                {
                    var dialog = new MessageDialog("Error Opening " + Settings.Instance.ComPort.Name + " Port");
                    await dialog.ShowAsync();
                }                
            }
        }

        public void Stop()
        {
            if (Status == ConnectionStatus.offline)
                return;

            LeaveWorking();
            LeavePing();
            GoToOffline();
            _continueWork = false;
            commThread.Abort();
            commThread.Join();
        }

        public void Pause(bool pause)
        {
            paused = pause;
        }

        private ConnectionStatus _status;
        public ConnectionStatus Status
        {
            get { return _status; }
            protected set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public void AddCommand(StdCommand cmd, CommandsIntervals interval)
        {
            cmd.Completed += CmdCompleted;

            if (interval == CommandsIntervals.Once)
            {
                EnqueueCmdForSending(cmd);
            }
            else
            {
                lock (RepeatingCommands)
                {
                    if (!RepeatingCommands.ContainsKey(interval))
                        RepeatingCommands[interval] = new HashSet<StdCommand>();
                    RepeatingCommands[interval].Add(cmd);
                }
            }
        }

        private void EnqueueCmdForSending(StdCommand cmd)
        {
            lock (SendingQueue)
            {
                SendingQueue.Enqueue(cmd);
                Monitor.Pulse(SendingQueue);
            }
        }

        public void RemoveCommand(StdCommand cmd, CommandsIntervals interval)
        {
            lock (RepeatingCommands)
            {
                RepeatingCommands[interval].Remove(cmd);
            }
        }



        public void SendCommands()
        {
            lock (SendingQueue)
            {
                _explicitSendReceived = true;
            }
        }

        protected ConnectionManager()
        {
            Status = ConnectionStatus.offline;
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            RepeatingCommands = new Dictionary<CommandsIntervals, HashSet<StdCommand>>();
            SendingQueue = new Queue<StdCommand>();
            commandRunning = new Object();
            pingCmd = new ReadRAM();
            (pingCmd as ReadRAM).Variable = new FW_REV();
            AddCommand(pingCmd, CommandsIntervals.Fast);
        }

        private void GoToPing()
        {
            timer.Elapsed += PerformPing;
            //timer.Interval = CommonResources.Instance.ConnectionPingInterval; // PING_INTERVAL;
            timer.Interval = 2500;
            timer.Start();
            Status = ConnectionStatus.ping;
        }

        private void LeavePing()
        {
            timer.Stop();
            timer.Elapsed -= PerformPing;
            lock (SendingQueue)
            {
                SendingQueue.Clear();
            }
        }

        private void PerformPing(object sender, ElapsedEventArgs e)
        {
            EnqueueCmdForSending(pingCmd);
        }

        private void GoToWorking()
        {
            timer.Elapsed += PerformWork;
            //timer.Interval = CommonResources.Instance.ConnectionWorkingInterval; //WORKING_INTERVAL;
            timer.Interval = 2500;
            timer.Start();
            Status = ConnectionStatus.working;
        }

        private void LeaveWorking()
        {
            timer.Stop();
            timer.Elapsed -= PerformWork;
            lock (SendingQueue)
            {
                SendingQueue.Clear();
            }
        }

        private void PerformWork(object sender, ElapsedEventArgs e)
        {
            if (paused)
                return;

            /*lock (RepeatingCommands)
            {
                CommandsIntervals currentInterval = GetIntervalToSend();

                foreach (var cmd in RepeatingCommands[currentInterval])
                    EnqueueCmdForSending(cmd);
            }*/
        }

        private CommandsIntervals GetIntervalToSend()
        {
            int ratio = (int)(SLOW_INTERVAL / FAST_INTERVAL);

            if (_explicitSendReceived)
            {
                _explicitSendReceived = false;
                return CommandsIntervals.Explicit;
            }
            else if (_intervalCounter++ % ratio == 0)
            {
                return CommandsIntervals.Slow;
            }
            else
            {
                return CommandsIntervals.Fast;
            }
        }

        private void ProcessQueue()
        {
            try
            {
                while (_continueWork)
                {
                    lock (SendingQueue)
                    {
                        if (SendingQueue.Count == 0)
                        {
                            try
                            {
                                Monitor.Wait(SendingQueue);
                            }
                            catch (SynchronizationLockException e)
                            {
                                Console.WriteLine(e);
                            }
                            catch (ThreadInterruptedException e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        if (paused)
                            continue;
                        StdCommand cmd = SendingQueue.Dequeue();
                        cmd.setPortHandler(Settings.Instance.portHandler);
                        cmd.send();
                    }
                    lock (commandRunning)
                    {
                        try
                        {
                            Monitor.Wait(commandRunning);
                        }
                        catch (SynchronizationLockException e)
                        {
                            Console.WriteLine(e);
                        }
                        catch (ThreadInterruptedException e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {

            }
        }

        private void CmdCompleted(object sender, CommandCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if(e.Result == null)
                {
                    CommandFailed();
                }
                else if (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess)
                {
                    CommandSucceded();
                }
            }
            else
            {
                //if (CommonResources.Instance.CommExcEnable)
                //    System.Windows.MessageBox.Show(e.Error.ToString());
                CommandFailed();
            }
            lock (commandRunning)
            {
                Monitor.Pulse(commandRunning);
            }
        }

        private void CommandFailed()
        {
            if (Status == ConnectionStatus.working)
            {
                LeaveWorking();
                GoToPing();
            }
        }

        private void CommandSucceded()
        {
            if (Status == ConnectionStatus.ping)
            {
                LeavePing();
                GoToWorking();
            }
        }

        private void GoToOffline()
        {
            timer.Stop();
            Status = ConnectionStatus.offline;
        }

        private System.Timers.Timer timer;

        public enum ConnectionStatus
        {
            offline = 0,
            ping,
            working
        }

        public enum CommandsIntervals
        {
            Once = 0,
            Fast,
            Slow,
            AtReset,
            Explicit
        }

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

        private bool paused = false;
        private volatile Boolean _explicitSendReceived = false;
        private int _intervalCounter = 0;
        private volatile bool _continueWork;
        private Thread commThread;
        private const Double PING_INTERVAL = 2500.0;
        private const Double WORKING_INTERVAL = 1000.0;
        private const Double FAST_INTERVAL = 1000.0;
        private const Double SLOW_INTERVAL = 10000.0;
        private Object commandRunning;
        private StdCommand pingCmd;
        private Dictionary<CommandsIntervals, HashSet<StdCommand>> RepeatingCommands;
        private Queue<StdCommand> SendingQueue;
    }
}
