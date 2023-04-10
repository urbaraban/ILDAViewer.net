using ILDA.net;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                if (IldaFile.Open(fileDialog.FileName) is IldaFile file)
                {
                    this.Add(file);
                    this.SelectedFile = this[this.Count - 1];
                }
            }
        });

        public ICommand RemoveFileCommand => new ActionCommand(() => {
            this.Remove(this.SelectedFile);
        });

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
