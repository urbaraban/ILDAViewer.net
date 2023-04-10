using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ILDAViewer.net.models;

namespace ILDAViewer.net.controls
{
    /// <summary>
    /// Логика взаимодействия для OpenTKFrame.xaml
    /// </summary>
    public partial class OpenTKFrame : UserControl
    {
        public OpenTKFrame()
        {
            InitializeComponent();
            var settings = new GLWpfControlSettings
            {
                MajorVersion = 2,
                MinorVersion = 1
            };
            OpenTkControl.Start(settings);
            GL.Enable(EnableCap.ProgramPointSize);
            OpenTkControl.SizeChanged += OpenTkControl_SizeChanged;
            // InsetControl.Start(settings);
        }

        private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.DataContext is FileModel frameModel)
            {
                frameModel.Resize(e.NewSize.Width, e.NewSize.Height);
            }
        }

        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            if (this.DataContext is FileModel frameModel)
            {
                if (frameModel.SelectedFrame != null)
                {
                    frameModel.DrawFrame(frameModel.SelectedFrame);
                }                
            }
        }

        private void UserControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (this.DataContext is FileModel file)
            {
                file.SelectedIndex += (e.Delta / Math.Abs(e.Delta));
            }
        }
    }
}
