using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using MC_Suite.Services;
using Windows.UI.Popups;
using Windows.Devices.Enumeration;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class FileBrowser : Page
    {
        public FileBrowser()
        {
            this.InitializeComponent();
            Inizialize();
        }

        private DispatcherTimer RefreshTimer;
        private void Inizialize()
        {
            if (VerificatorConfig.Instance.TarMode == true)
                FileFiltersBox.Visibility = Visibility.Visible;
            else
                FileFiltersBox.Visibility = Visibility.Collapsed;

            FileFiltersBox.ItemsSource = FileFilter;
            FileFiltersBox.SelectedIndex = (int)FileManager.ViewFileFilter;
            UpdatingRing.Visibility = Visibility.Visible;
            RefreshTimer = new DispatcherTimer();
            RefreshTimer.Interval = TimeSpan.FromMilliseconds(2000);
            RefreshTimer.Tick += RefreshTimer_Tick;
            CurrentFolder = FileManager.MainFolder;
            RefreshTimer.Stop();
            RefreshDrives();
        }

        private FolderData CurrentFolder;

        private void RefreshDirBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdatingRing.Visibility = Visibility.Visible;
            RefreshDrives();
        }

        private async void RefreshDrives()
        {
            FileManager.RefreshDrivesListView();

            if (FileManager.UsbFolder != null)
            {
                ChangeDirBtn.Visibility = Visibility.Visible;
                CopyFileBtn.Visibility = Visibility.Visible;
            }
            else
            {
                ChangeDirBtn.Visibility = Visibility.Collapsed;
                CopyFileBtn.Visibility = Visibility.Collapsed;
            }

            await FileManager.UpdateFileList( CurrentFolder );

            FileManager.RefreshFileListView();

            UpdatingRing.Visibility = Visibility.Collapsed;

            if(RefreshTimer.IsEnabled == false)
                RefreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, object e)
        {
           if(FileManager.UsbDriveChanged)
           {
                FileManager.UsbDriveChanged = false;
                RefreshDrives();
            }
        }

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
            }
        }

        private IDictionary<Storage.ViewFilesTypes, String> _fileFilter;
        public IDictionary<Storage.ViewFilesTypes, String> FileFilter
        {
            get
            {
                if (_fileFilter == null)
                {
                    _fileFilter = new Dictionary<Storage.ViewFilesTypes, String>();
                    _fileFilter.Add(Storage.ViewFilesTypes.All, "All");
                    _fileFilter.Add(Storage.ViewFilesTypes.User, "User Files");
                    _fileFilter.Add(Storage.ViewFilesTypes.Cfg, "System Files");
                }
                return _fileFilter;
            }
        }

        private async void DeleteFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if(FileBrowserGrid.SelectedItem != null)
            {
                var dialog = new MessageDialog("Are you sure?");
                dialog.Title = "Delete File";
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
                var res = await dialog.ShowAsync();

                if ((int)res.Id == 0)
                { 
                    FileData SelectedFile = FileBrowserGrid.SelectedItem as FileData;

                    await SerializableStorage<VariableImage>.Delete(SelectedFile.Name, SelectedFile.FullPath, true);

                    await FileManager.UpdateFileList(FileManager.CurrentFolder);
                }
            }
        }

        private async void CryptFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FileBrowserGrid.SelectedItem != null)
            {
                var dialog = new MessageDialog("Are you sure?");
                dialog.Title = "Crypt File";
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
                var res = await dialog.ShowAsync();

                if ((int)res.Id == 0)
                {
                    FileData SelectedFile = FileBrowserGrid.SelectedItem as FileData;

                    FileCrypt.Instance.EncryptFile(SelectedFile.Name, ".enc");

                    await FileManager.UpdateFileList(FileManager.CurrentFolder);
                }
            }
        }

        private async void CopyFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FileBrowserGrid.SelectedItem != null)
            {
                FileData SelectedFile = FileBrowserGrid.SelectedItem as FileData;

                if(SelectedFile.FullPath == FileManager.MainFolder.Path)
                    await SerializableStorage<VariableImage>.Copy(SelectedFile.Name, SelectedFile.FullPath, FileManager.UsbFolder.Path);
                else
                    await SerializableStorage<VariableImage>.Copy(SelectedFile.Name, SelectedFile.FullPath, FileManager.MainFolder.Path);
            }
        }

        private async void ChangeDirBtn_Click(object sender, RoutedEventArgs e)
        {
            if(DriveBrowserGrid.SelectedItem != null)
            {
                CurrentFolder = DriveBrowserGrid.SelectedItem as FolderData;

                await FileManager.UpdateFileList(CurrentFolder);

                FileManager.RefreshFileListView();
            }
        }

        private void FileFiltersBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileManager.ViewFileFilter = (Storage.ViewFilesTypes) FileFiltersBox.SelectedValue;
            FileManager.RefreshFileListView();
        }


    }
}
