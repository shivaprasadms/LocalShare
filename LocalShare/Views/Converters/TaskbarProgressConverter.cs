using System;
using System.Globalization;
using System.Windows.Data;

namespace LocalShare.Views.Converters
{
    public class TaskbarProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private enum TaskbarItemProgressState
        {
            // Summary:
            //     No progress indicator is displayed in the taskbar button.
            None = 0,
            //
            // Summary:
            //     A pulsing green indicator is displayed in the taskbar button.
            Indeterminate = 1,
            //
            // Summary:
            //     A green progress indicator is displayed in the taskbar button.
            Normal = 2,
            //
            // Summary:
            //     A red progress indicator is displayed in the taskbar button.
            Error = 3,
            //
            // Summary:
            //     A yellow progress indicator is displayed in the taskbar button.
            Paused = 4,
        }
    }


}
