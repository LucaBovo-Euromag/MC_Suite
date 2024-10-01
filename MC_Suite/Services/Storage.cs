using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.Storage;
using MC_Suite.Euromag.Protocols.StdCommands;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using MC_Suite.Properties;

namespace MC_Suite.Services
{
    public class Storage
    {
        private static Storage _instance;
        public static Storage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Storage();
                return _instance;
            }
        }       

        public VariableImage GetVariableParams(IEEPROMvariable ParToSave, UInt32 Value)
        {
            VariableImage variableImage = new VariableImage();
            VariableImage LocalVar = variableImage;
            LocalVar.Address = ParToSave.Address;
            LocalVar.Size = ParToSave.Size;
            LocalVar.DataType = ParToSave.DataType;
            LocalVar.Name = ParToSave.Name;
            LocalVar.ValAsString = ParToSave.ValAsString;
            LocalVar.Value = Value;
            return LocalVar;
        }

        public async Task<bool> UpdateFileList(FolderData Folder)
        {
            int mem_count = AllFileList.Count;

            Windows.Storage.StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(Folder.Path);
            IReadOnlyList<StorageFile> FileList = await _folder.GetFilesAsync();

            if (FileList != null && FileList.Count != 0) 
            {
                if(FileList.Count != mem_count)
                { 
                    this.AllFileList.Clear();
                    this.CfgFileList.Clear();
                    this.UserFileList.Clear();
                    this.ViewFiles.Clear();

                    FileData F;
                    Windows.Storage.FileProperties.BasicProperties Fproperties;
                    foreach (StorageFile File in FileList)
                    {
                        F = new FileData();
                        F.Name = File.Name;
                        F.Path = Folder.Name;
                        F.FullPath = _folder.Path;
                        F.Date = File.DateCreated.DateTime;
                        Fproperties = await File.GetBasicPropertiesAsync();
                        F.Size = Fproperties.Size.ToString();
                        this.AllFileList.Add(F);
                        string path = Path.GetExtension(File.Name);
                        if ((path == ".cfg") || (path == ".xml") || (path == ".db") || (path == ".key"))
                            this.CfgFileList.Add(F);
                        else
                            this.UserFileList.Add(F);
                    }

                    switch(this.ViewFileFilter)
                    {
                        case ViewFilesTypes.All:
                            this.AllFileList.ForEach(p => this.ViewFiles.Add(p));
                            break;
                        case ViewFilesTypes.User:
                            this.UserFileList.ForEach(p => this.ViewFiles.Add(p));
                            break;
                        case ViewFilesTypes.Cfg:
                            this.CfgFileList.ForEach(p => this.ViewFiles.Add(p));
                            break;
                    }
                }
            }
            else
            {
                this.AllFileList.Clear();
                this.CfgFileList.Clear();
                this.UserFileList.Clear();
                this.ViewFiles.Clear();
            }
            return true;
        }

        public void RefreshFileListView()
        {
            this.ViewFiles.Clear();

            switch (this.ViewFileFilter)
            {
                case ViewFilesTypes.All:
                    this.AllFileList.ForEach(p => this.ViewFiles.Add(p));
                    break;
                case ViewFilesTypes.User:
                    this.UserFileList.ForEach(p => this.ViewFiles.Add(p));
                    break;
                case ViewFilesTypes.Cfg:
                    this.CfgFileList.ForEach(p => this.ViewFiles.Add(p));
                    break;
            }
        }

        private string _savedFileName;
        public string SavedFileName
        {
            get { return _savedFileName; }
            set
            {
                if (value != _savedFileName)
                {
                    _savedFileName = value;
                    OnPropertyChanged("SavedFileName");
                }
            }
        }

        private FolderData _currentFolder;
        public FolderData CurrentFolder
        {
            get { return _currentFolder; }
            set
            {
                if (value != _currentFolder)
                {
                    _currentFolder = value;
                    OnPropertyChanged("CurrentFolder");
                }
            }
        }

        private bool _usbDriveChanged;
        public bool UsbDriveChanged
        {
            get { return _usbDriveChanged; }
            set
            {
                if (value != _usbDriveChanged)
                {
                    _usbDriveChanged = value;
                    OnPropertyChanged("UsbDriveChanged");
                }
            }
        }

        public async Task<bool> GetUSBDrives()
        {
            IReadOnlyList<StorageFolder> UsbDrivesList = (await KnownFolders.RemovableDevices.GetFoldersAsync());

            USBDriversList.Clear();
            UsbFolder = null;

            /*foreach (StorageFolder folder in UsbDrivesList)
            {
                FolderData Drive = new FolderData();
                Drive.Name = folder.Name;
                Drive.DisplayName = folder.DisplayName;
                Drive.DisplayType = folder.DisplayType;
                Drive.Path = folder.Path;
                USBDriversList.Add(Drive);
            }*/

            if(UsbDrivesList.Count > 0)
            {
                FolderData Drive = new FolderData();
                Drive.Name = UsbDrivesList[0].Name;
                Drive.DisplayName = UsbDrivesList[0].DisplayName;
                Drive.DisplayType = UsbDrivesList[0].DisplayType;
                Drive.Path = UsbDrivesList[0].Path;
                USBDriversList.Add(Drive);
                UsbFolder = Drive;
            }
                
            return true;
        }


        private FolderData _mainFolder;
        public FolderData MainFolder
        {
            get
            {
                if(_mainFolder == null)
                {
                    _mainFolder = new FolderData();
                    _mainFolder.Name = "C:\\";
                    _mainFolder.DisplayName = "Local Drive (C:)";
                    _mainFolder.DisplayType = "";
                    _mainFolder.Path = ApplicationData.Current.LocalFolder.Path;
                }
                return _mainFolder;
            }
        }

        public string ConfigFile
        {
            get
            {
                return "MC_Suite_Cfg.xml";
            }
        }

        public string VerificatorConfigFile
        {
            get
            {
                return "MC_Suite_VerifCfg.xml";
            }
        }

        private FolderData _usbFolder;
        public FolderData UsbFolder
        {
            get
            {
                return _usbFolder;
            }
            set
            {
                if (value != _usbFolder)
                {
                    _usbFolder = value;
                    OnPropertyChanged("UsbFolder");
                }
            }
        }

        public enum ViewFilesTypes
        {
            All,
            User,
            Cfg
        }

        private ViewFilesTypes _viewFileFilter;
        public ViewFilesTypes ViewFileFilter
        {
            get
            {
                return _viewFileFilter;
            }
            set
            {
                if (value != _viewFileFilter)
                {
                    _viewFileFilter = value;
                    OnPropertyChanged("ViewFileFilter");
                }
            }
        }

        public void RefreshDrivesListView()
        {
            try
            { 
                this.USBDrivers.Clear();
                USBDrivers.Add(this.MainFolder);
                this.USBDriversList.ForEach(p => this.USBDrivers.Add(p));
            }
            catch { }
        }

        public List<FileData> CfgFileList   = new List<FileData>();    
        public List<FileData> UserFileList  = new List<FileData>();
        public List<FileData> AllFileList   = new List<FileData>();
        public ObservableCollection<FileData> ViewFiles = new ObservableCollection<FileData>();

        public List<FolderData> USBDriversList = new List<FolderData>();
        public ObservableCollection<FolderData> USBDrivers = new ObservableCollection<FolderData>();

        public List<Configuration> ConfigList = new List<Configuration>();

        #region ObservableObject

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

    }

    public class VariableImage
    {
        public EEPROMAddresses Address;
        public TargetDataType DataType;
        public string Name;
        public string ValAsString;
        public int Size;
        public UInt32 Value;
    }

    public class FileData
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public string FullPath;        
    }

    public class FolderData
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DisplayType;
        public string Path;
    }

    public class GraphValue
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }

    public class BatteryGraphValue
    {
        public DateTime Time { get; set; }
        public int Value { get; set; }
    }


    public static class BinaryStorage
    {
        /// <summary>
        /// Write Binary File, replace if existing
        /// </summary>
        /// <typeparam name="FileName">File Name</typeparam>
        /// <typeparam name="Folder">Destination Folder</typeparam>
        /// <typeparam name="Content">Data to write</typeparam>
        public static async Task<bool> Write(string FileName, string Folder, byte[] Content)
        {
            Windows.Storage.StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(Folder);
            StorageFile _file = await _folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            try
            { 
                using (Stream fileStream = await _file.OpenStreamForWriteAsync())
                {
                    await fileStream.WriteAsync(Content, 0, Content.Length);
                    await fileStream.FlushAsync();
                    fileStream.Dispose();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Append data to an existing Binary File, if don't exists, create a new file
        /// </summary>
        /// <typeparam name="FileName">File Name</typeparam>
        /// <typeparam name="Folder">Destination Folder</typeparam>
        /// <typeparam name="Content">Data to append</typeparam>
        public static async Task<bool> Append(string FileName, string Folder, char[] Content)
        {
            Windows.Storage.StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(Folder);
            StorageFile _file = await _folder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);

            //Se file non esistente lo creo
            if(_file == null)
                _file = await _folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

            Windows.Storage.FileProperties.BasicProperties Fproperties;
            Fproperties = await _file.GetBasicPropertiesAsync();

            byte[] Data = new byte[Content.Length];
            for(int i=0; i<Content.Length; i++)
            {
                try { 
                    Data[i] = Convert.ToByte(Content[i]);
                }
                catch { }
            }

            try
            {
                using (Stream fileStream = await _file.OpenStreamForWriteAsync())
                {
                    fileStream.Seek((long)Fproperties.Size, SeekOrigin.Begin);
                    await fileStream.WriteAsync(Data, 0, Content.Length);
                    await fileStream.FlushAsync();
                    fileStream.Dispose();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }


    /// <summary>
    /// Provides functions to save and load single object as well as List of 'T' using serialization
    /// </summary>
    /// <typeparam name="T">Type parameter to be serialize</typeparam>
    public static class SerializableStorage<T> where T : new()
    {
        public static async Task<bool> Save(string FileName, string Folder, List<T> _Data)
        {
            try
            { 
                MemoryStream _MemoryStream = new MemoryStream();
                DataContractSerializer Serializer = new DataContractSerializer(typeof(List<T>));
                Serializer.WriteObject(_MemoryStream, _Data);

                Task.WaitAll();

                Windows.Storage.StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(Folder);
                StorageFile _file = await _folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

                using (Stream fileStream = await _file.OpenStreamForWriteAsync())
                {
                    _MemoryStream.Seek(0, SeekOrigin.Begin);
                    await _MemoryStream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                    fileStream.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async void Append(string FileName, string Folder, List<T> _Data)
        {
            MemoryStream _MemoryStream = new MemoryStream();
            DataContractSerializer Serializer = new DataContractSerializer(typeof(List<T>));
            Serializer.WriteObject(_MemoryStream, _Data);

            Task.WaitAll();

            Windows.Storage.StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(Folder);
            StorageFile _file = await _folder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);

            Windows.Storage.FileProperties.BasicProperties Fproperties;
            Fproperties = await _file.GetBasicPropertiesAsync();

            using (Stream fileStream = await _file.OpenStreamForWriteAsync())
            {
                _MemoryStream.Seek(0, SeekOrigin.Begin);
                fileStream.Seek((long)Fproperties.Size, SeekOrigin.Begin);
                await _MemoryStream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Dispose();
            }
        }

        public static async Task<List<T>> Load(string FileName, string Folder)
        {            
            StorageFile _file;
            List<T> Result;

            Windows.Storage.StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(Folder);

            try
            {
                Task.WaitAll();
                _file = await _folder.GetFileAsync(FileName);

                using (Stream stream = await _file.OpenStreamForReadAsync())
                {
                    DataContractSerializer Serializer = new DataContractSerializer(typeof(List<T>));

                    Result = (List<T>)Serializer.ReadObject(stream);                    
                }
                return Result;
            }
            catch
            {
                return new List<T>();
            }
        }

        public static async Task<bool> Copy(string FileName, string Folder, string DestFolder)
        {
            StorageFile _File;

            StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(Folder);
            StorageFolder _destFolder = await StorageFolder.GetFolderFromPathAsync(DestFolder);

            try
            {
                Task.WaitAll();
                _File = await _folder.GetFileAsync(FileName);
                await _File.CopyAsync(_destFolder, FileName, NameCollisionOption.ReplaceExisting);

                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Copy File",
                    Content = "File Copy Success",
                    CloseButtonText = "OK",
                };
                await dialog.ShowAsync();

                return true;
            }
            catch ( Exception e )
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Copy File",
                    Content = "File Copy Error: " + e.Message,
                    CloseButtonText = "OK",
                };
                await dialog.ShowAsync();
                return false;
            }
        }

        public static async Task<bool> Delete(string FileName, string Folder, bool ShowResultDlg)
        {
            StorageFile _file;

            Windows.Storage.StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(Folder);

            try
            {
                Task.WaitAll();
                _file = await _folder.GetFileAsync(FileName);
                await _file.DeleteAsync();

                if(ShowResultDlg)
                { 
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Delete File",
                        Content = "File Deleting Success",
                        CloseButtonText = "OK",
                    };
                    await dialog.ShowAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                if(ShowResultDlg)
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Delete File",
                        Content = "File Deleting Error: " + e.Message,
                        CloseButtonText = "OK",
                    };
                    await dialog.ShowAsync();
                }
                return false;
            }
        }   
    }
}
