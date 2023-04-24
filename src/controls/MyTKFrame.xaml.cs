using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using ILDAViewer.net.models;
using System.Windows.Input;

namespace ILDAViewer.net.controls
{
    /// <summary>
    /// Логика взаимодействия для MyTKFrame.xaml
    /// </summary>
    public partial class MyTKFrame : UserControl
    {
        public MyTKFrame()
        {
            InitializeComponent();
            var settings = new GLWpfControlSettings
            {
                MajorVersion = 2,
                MinorVersion = 1
            };
            MyTkControl.Start(settings);
            GL.Enable(EnableCap.ProgramPointSize);
            MyTkControl.SizeChanged += OpenTkControl_SizeChanged;
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
                frameModel.Width = (float)this.ActualWidth;
                frameModel.Height = (float)this.ActualHeight;

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
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    float delta = (e.Delta / Math.Abs(e.Delta)) * 0.01f;
                    file.Scale += delta;
                } 
                else
                {
                    file.SelectedIndex += (e.Delta / Math.Abs(e.Delta));
                }

            }
        }
    }
}
