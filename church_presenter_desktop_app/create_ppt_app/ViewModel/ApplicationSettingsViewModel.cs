using create_ppt_app.Command;
using create_ppt_app.Model;
using create_ppt_app.Model.DriveDTO;
using create_ppt_app.MVVM;
using create_ppt_app.utils;
using create_ppt_app.View;
using GoogleDrivePickerWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using ApplicationSettings = create_ppt_app.Model.ApplicationSettings;
using Setting = create_ppt_app.Model.Setting;

namespace create_ppt_app.ViewModel
{
    public class ApplicationSettingsViewModel : ViewModelBase
    {
        private static readonly Lazy<ApplicationSettingsViewModel> _instance = 
            new Lazy<ApplicationSettingsViewModel>(() => new ApplicationSettingsViewModel());
        public static ApplicationSettingsViewModel Instance => _instance.Value;

        private const string ConfigFilePath = "DefaultSettings.json";
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() } // Enables enum <-> string conversion
        };
        private ApplicationSettingsViewModel(){ }
        
        private static Dictionary<string, SettingViewModel> settings = new Dictionary<string, SettingViewModel>();
        public List<SettingViewModel> SettingsList { get; set; }

       public string LyricsSource
        {
            get { return settings["lyricsSource"].SettingValue; }
            set
            {
                settings["lyricsSource"].SettingValue = value;
                OnPropertyChanged();
            }
        }
        public string LocalFolder
        {
            get { return settings["localFolder"].SettingValue; }
            set
            {
                settings["localFolder"].SettingValue = value;
                OnPropertyChanged();
            }
        }

        private UserInfo userAccount;
        public UserInfo UserAccount
        {
            get { return userAccount; }
            set
            {
                userAccount = value;
                DriveAccountName = userAccount.email;
                OnPropertyChanged();
            }
        }
        public string DriveAccountName
        {
            get { return settings["driveAccount"].SettingValue; }
            set
            {
                settings["driveAccount"].SettingValue = value;
                OnPropertyChanged();
            }
        }
        public string DriveFolderId { 
            get { return settings["driveFolderId"].SettingValue; }
            set { settings["driveFolderId"].SettingValue = value; }
        }
        public string DriveFolderName
        {
            get { return settings["driveFolderName"].SettingValue; }
            set
            {
                settings["driveFolderName"].SettingValue = value;
                OnPropertyChanged();
            }
        }
        public string SlideRatio
        {
            get { return settings["slideRatio"].SettingValue; }
        }
        private int height;
        public int Height
        {
            get
            {
                SlideRatio s = (SlideRatio)Enum.Parse(typeof(SlideRatio), settings["slideRatio"].SettingValue);
                var dim = s.GetDimensions();
                return dim.Height;
            }
        }
        private int width;
        public int Width
        {
            get
            {
                SlideRatio s = (SlideRatio)Enum.Parse(typeof(SlideRatio), settings["slideRatio"].SettingValue);
                var dim = s.GetDimensions();
                return dim.Width;
            }
        }

        public SongSettingsViewModel defaultSongSettings;

        public async Task Initialize()
        {
            List<Dictionary<string, Setting>> defaultSettings = await LoadDefaultSettings();
            //foreach (var (k, v) in defaultSettings[0])
            //    settings[k] = new SettingViewModel(v);

            settings["lyricsSource"] = new SettingViewModel(defaultSettings[0]["lyricsSource"]);
            settings["localFolder"] = new SettingViewModel(defaultSettings[0]["localFolder"], new SelectLocalFolderCommand());
            settings["driveFolderId"] = new SettingViewModel(defaultSettings[0]["driveFolderId"]);
            settings["driveAccount"] = new SettingViewModel(defaultSettings[0]["driveAccount"], new AuthenticateCommand());
            settings["driveFolderName"] = new SettingViewModel(defaultSettings[0]["driveFolderName"], new SelectDriveFolder());
            settings["slideRatio"] = new SettingViewModel(defaultSettings[0]["slideRatio"]);

            defaultSongSettings = new SongSettingsViewModel(defaultSettings[1]);
            Debug.Print("Application song setting "+defaultSongSettings.GetHashCode());

            SettingsList = new List<SettingViewModel>()
            {
                settings["lyricsSource"],
                settings["localFolder"],
                settings["driveAccount"],
                settings["driveFolderName"],
                settings["slideRatio"],
            };
            SettingsList.AddRange(defaultSongSettings.SettingsList);
        }


        public static async Task<List<Dictionary<string, Setting>>> LoadDefaultSettings()
        {
            if (File.Exists(ConfigFilePath))
            {
                
                string json = await File.ReadAllTextAsync(ConfigFilePath);
                return JsonSerializer.Deserialize<List<Dictionary<string, Setting>>>(json, jsonOptions);
            }
            return null;
        }

        public async void Save()
        {
            var saveSettings = new List<Dictionary<string, Setting>>();
            Dictionary<string, Setting> t = new Dictionary<string, Setting>();
            foreach (var(k,v) in settings)
            {
                t[k] = v.Setting;
            }
            saveSettings.Add(t);
            saveSettings.Add(defaultSongSettings.Save());
            
            string jsonString = JsonSerializer.Serialize(saveSettings, jsonOptions);
            await File.WriteAllTextAsync(ConfigFilePath, jsonString);
        }
    }
}
