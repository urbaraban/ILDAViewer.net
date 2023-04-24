using ILDAViewer.net.models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ILDAViewer.net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FrameSlider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
            {
                var Point = e.GetPosition(slider);
                double prop = Point.X / slider.ActualWidth;
                if (prop != double.NaN)
                {
                    slider.Value = slider.Minimum + Math.Round((slider.Maximum - slider.Minimum) * prop - 1);
                }
            }
        }

        private void TextBox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (this.DataContext is FileCollectionModel filecollectionmodel)
            {
                if (filecollectionmodel.SelectedFile != null)
                {
                    filecollectionmodel.SelectedFile.FramePerSecond += (e.Delta / Math.Abs(e.Delta));
                }
            }
        }
    }
}
