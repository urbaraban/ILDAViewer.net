using ILDA.net;
using ILDAViewer.net.dialogs;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ILDAViewer.net.models
{
    internal class FileCollectionModel : ObservableCollection<FileModel>, INotifyPropertyChanged
    {
        public FileModel SelectedFile 
        {
            get => _selectedfile;
            set
            {
                _selectedfile = value;
                NotifyPropertyChanged(nameof(SelectedFile));
            }
        }
        private FileModel _selectedfile;

        public bool PointDraw
        {
            get => Properties.Settings.Default.point_show;
            set
            {
                Properties.Settings.Default.point_show = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged(nameof(PointDraw));
            }
        }
        public bool MultiplierDraw
        {
            get => Properties.Settings.Default.multiplier_show;
            set
            {
                Properties.Settings.Default.multiplier_show = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged(nameof(MultiplierDraw));
            }
        }
        public bool ShowBlanked
        {
            get => Properties.Settings.Default.show_blanked;
            set
            {
                Properties.Settings.Default.show_blanked = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged(nameof(ShowBlanked));
            }
        }

        public FileCollectionModel() 
        {
            this.Add(new FileModel(new IldaFile()));
        }

        public ICommand OpenFileCommand => new ActionCommand(() => {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "ILDA (.ild)|*.ild|All Files (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                OpenFile(fileDialog.FileName);
            }
        });

        public ICommand RemoveFileCommand => new ActionCommand(() => {
            this.Remove(this.SelectedFile);
        });

        public ICommand ShowInfoCommand => new ActionCommand(() => {
            FileInfoDialog fileInfoDialog = new FileInfoDialog()
            {
                DataContext = this.SelectedFile
            };
            fileInfoDialog.Show();
        });

        private void OpenFile(string filepath)
        {
            FileInfo fileInfo = new FileInfo(filepath);
            if (fileInfo.Exists == true)
            {
                if (IldaFile.Open(filepath) is IldaFile file)
                {
                    this.Add(new FileModel(file));
                    if (SelectedFile != null && this.SelectedFile.Location == "empty")
                    {
                        this.Remove(this.SelectedFile);
                    }
                    this.SelectedFile = this[this.Count - 1];
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
