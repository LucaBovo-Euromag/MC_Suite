using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using System;
    using System.Timers;
    using System.Collections;
    using System.Collections.Generic;

    public class CommandScheduler
    {
        public CommandScheduler()
        {
            waitingCommandComplete = false;
            sendingQueue = new List<StdCommand>();
            timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Tick);
        }

        public Boolean Enabled
        {
            get
            { return timer.Enabled; }
            set
            { timer.Enabled = value; }
        }

        public double Interval
        {
            get
            { return timer.Interval; }
            set
            {
                Boolean enabled = Enabled;
                Enabled = false;
                timer.Interval = value;
                Enabled = enabled;
            }
        }

        public String PortID
        {
            get
            {
                if (portHandler != null)
                    return portHandler.PortID;
                return null;
            }
            set
            {
                if (portHandler == null)
                    portHandler = new commPortHandler(value, 76800, 0, 8, 0, TimeSpan.FromMilliseconds(500));
                else
                {
                    if (PortID != value)
                    {
                        // TODO: fix to avoid changing this during transmission
                        portHandler = new commPortHandler(value, 76800, 0, 8, 0, TimeSpan.FromMilliseconds(500));
                    }
                }
            }
        }

        public void AddOneShot(StdCommand command)
        {
            if (command != null)
            {
                sendingQueue.Add(command);
                processQueue();
            }
        }

        public void AddContinuos(StdCommand command)
        {
            if (continuousSenders == null)
                continuousSenders = new Hashtable();

            Enabled = false;

            continuousSenders.Add(command.GetHashCode(), command);

            Enabled = true;
        }

        public void RemoveContinuos(StdCommand command)
        {
            if (continuousSenders == null)
                return;

            Enabled = false;

            continuousSenders.Remove(command.GetHashCode());

            if (continuousSenders.Count != 0)
                Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (sendingQueue.Count != 0)
                return;

            if (continuousSenders != null)
            {
                foreach (StdCommand cmd in continuousSenders.Values)
                    sendingQueue.Add(cmd);
            }

            processQueue();
        }

        private void processQueue()
        {
            if (sendingQueue.Count == 0)
                return;

            if (waitingCommandComplete)
                return;

            StdCommand cmd = sendingQueue[0];

            if (cmd == null)
            {
                sendingQueue.RemoveAt(0);
                processQueue();
            }

            sendCommand(cmd);

        }

        private void sendCommand(StdCommand cmd)
        {
            if (portHandler == null)
                throw new ArgumentNullException("PortName must be set");
            waitingCommandComplete = true;

            cmd.setPortHandler(portHandler);

            cmd.Completed += new CommandCompletedEventHandler(commandCompleted);

            cmd.send();

        }

        private void commandCompleted(object sender, CommandCompletedEventArgs e)
        {
            sendingQueue[0].Completed -= commandCompleted;

            sendingQueue.RemoveAt(0);

            waitingCommandComplete = false;

            processQueue();
        }

        private List<StdCommand> sendingQueue;
        private bool waitingCommandComplete;
        private commPortHandler portHandler;
        Hashtable continuousSenders;
        private Timer timer;
    }
}
