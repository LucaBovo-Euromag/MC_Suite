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
    public class COMPortItemItemConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value as Properties.Settings.COMPortItem;
        }
    }

    public class CfgFileItemConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value as FileData;
        }
    }

    public class DescriptionToUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String val = value as String;

            if (String.IsNullOrEmpty(val))
                return null;

            Regex pattern = new Regex(@"(?<=\[)(.*?)(?=\])");
            Match match = pattern.Match(val);

            return match.Groups[1].Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class IrConnectionStatusToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IrCOMPortManager.IrConnectionStates val = (IrCOMPortManager.IrConnectionStates)value;

            switch (val)
            {
                case IrCOMPortManager.IrConnectionStates.Stop:
                    return "Offline";
                case IrCOMPortManager.IrConnectionStates.Ping:
                    return "Ping...";
                case IrCOMPortManager.IrConnectionStates.Working:
                    return "Online";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
