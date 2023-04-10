using ILDA.net;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ILDAViewer.net.models
{
    internal class FileCollectionModel : ObservableCollection<IldaFile>, INotifyPropertyChanged
    {
        public IldaFile SelectedFile 
        {
            get => _selectedfile;
            set
            {
                _selectedfile = value;
                NotifyPropertyChanged(nameof(SelectedFile));
            }
        }
        private IldaFile _selectedfile;

        public FileCollectionModel() 
        {
            this.Add(new IldaFile());
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

        private void OpenFile(string filepath)
        {
            FileInfo fileInfo = new FileInfo(filepath);
            if (fileInfo.Exists == true)
            {
                if (IldaFile.Open(filepath) is IldaFile file)
                {
                    this.Add(file);
                    if (this.SelectedFile.Location == "empty")
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
