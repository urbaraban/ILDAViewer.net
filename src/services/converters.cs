using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using ILDA.net;
using ILDAViewer.net.models;
using System.Windows.Media;
using System.Collections;
using System.Linq;

namespace ILDAViewer.net.services
{
    internal class IldaFileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IldaFile file)
            {
                return new FileModel(file);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Hidden;

        }
    }

    internal class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index - 1;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index + 1;
            }
            return value;

        }
    }

    internal class BoolCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b == true)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;

        }
    }

    internal class VersionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = value.ToString();
            if (value is byte v)
            {
               
                switch (v)
                {
                    case (byte)0: result = "3D, palette";
                        break;
                    case (byte)1:
                        result = "2D, palette";
                        break;
                    case (byte)2:
                        result = "palette";
                        break;
                    case (byte)4:
                        result = "3D, RGB";
                        break;
                    case (byte)5:
                        result = "2D, RGB";
                        break;
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index + 1;
            }
            return value;

        }
    }

    internal class IldaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IldaColor color)
            {
                return Color.FromRgb(color.R, color.G, color.B);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    internal class PointColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IldaColor color = new IldaColor(200, 200, 200);

            if (values[0] is FileModel filemodel
                && filemodel.SelectedFrame != null &&
                values[1] is bool blanked)
            {
                if (blanked == false)
                {
                    if (filemodel.SelectedFrame.IldaVersion < 2 && values[2] is byte pindex)
                    {
                        color = filemodel.GetPalette(pindex);
                }
                    else if (values[3] is IldaColor ildaColor)
                    {
                        color = ildaColor;
                    }
                }
            }

            return Color.FromRgb(color.R, color.G, color.B);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    internal class IndexOfConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is FileModel file && values[1] is IldaColor obj)
            {
                int index = -1;
                index = file.Palette.Colors.IndexOf(obj);
                return index.ToString();
            }

            return null;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
