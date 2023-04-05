using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using ILDAViewer.net.models;

namespace ILDAViewer.net.controls
{
    /// <summary>
    /// Логика взаимодействия для OpenTKFrame.xaml
    /// </summary>
    public partial class OpenTKFrame : UserControl
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private TimeSpan _elapsedTime;

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
            GL.PointSize(10.0f);
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
                if (frameModel.SelectedFrame == null)
                {
                    frameModel.Render(delta);
                }
                else
                {
                    frameModel.DrawFrame(frameModel.SelectedFrame);
                }
                
            }
        }
    }
}
