using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using MC_Suite.Services;

namespace MC_Suite.Views
{
    public class IrConnectionStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IrCOMPortManager.IrConnectionStates val = (IrCOMPortManager.IrConnectionStates)value;

            switch (val)
            {
                case IrCOMPortManager.IrConnectionStates.Stop:
                case IrCOMPortManager.IrConnectionStates.Working:
                    return Visibility.Collapsed;
                case IrCOMPortManager.IrConnectionStates.Ping:
                    return Visibility.Visible;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }


    public class IrConnectionStatusToOnlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IrCOMPortManager.IrConnectionStates val = (IrCOMPortManager.IrConnectionStates)value;

            switch (val)
            {
                case IrCOMPortManager.IrConnectionStates.Stop:
                case IrCOMPortManager.IrConnectionStates.Ping:
                    return Visibility.Collapsed;
                case IrCOMPortManager.IrConnectionStates.Working:
                    return Visibility.Visible;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

}

