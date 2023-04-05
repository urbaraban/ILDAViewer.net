using ILDA.net;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ILDAViewer.net.models
{
    internal class FileCollectionModel : ObservableCollection<IldaFile>
    {
        public IldaFile SelectedFile { get; set; }

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
                    this.SelectedFile = file;
                }
            }
        });

        public ICommand RemoveFileCommand => new ActionCommand(() => {
            this.Remove(this.SelectedFile);
        });
    }
}
