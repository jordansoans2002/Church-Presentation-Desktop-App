using create_ppt_app.Command;
using create_ppt_app.GoogleDrive;
using create_ppt_app.Model;
using create_ppt_app.Model.DriveDTO;
using create_ppt_app.MVVM;
using create_ppt_app.utils;
using GoogleDrivePickerWpf;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Input;
using static FolderAccess;

namespace create_ppt_app.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string TitleText
        {
            get
            {
                Debug.Print("song title " + Song?.Title);
                return (Song==null)? "" : Song.Title;
            }
        }

        /// <summary>
        /// Dictionary with song name as key
        /// List of song names obtained from the list of files present in the user's lyrics source directory
        /// </summary>
        private Dictionary<string, SongSuggestion> titleSuggestions;
        private List<string> _titleSuggestions;
        public List<string> TitleSuggestions {
            get { return _titleSuggestions; }
            set
            {
                value.Sort();
                _titleSuggestions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Used to display song lyrics, settings and preview for the selected song
        /// </summary>
        public SongViewModel Song { get; set; }

        /// <summary>
        /// Is true whenever the songs are being re-ordered
        /// Prevents Song from continuously being updated during re-ordering
        /// </summary>
        public bool isMoving = false;
        private SongDetailsViewModel _selectedSong;
        public SongDetailsViewModel SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                _selectedSong = value;
                if (_selectedSong != null && !isMoving)
                {
                    Song = new SongViewModel(SelectedSong.song);
                    _settingsViewModel = Song.SongSettings;
                    OnPropertyChanged(nameof(Song));
                    OnPropertyChanged(nameof(TitleText));
                    OnPropertyChanged(nameof(SettingsViewModel));
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Used to display the name, languages and orientation of all the songs in the list
        /// </summary>
        public ObservableCollection<SongDetailsViewModel> SongDetails { get; set; }

        public GridLength SettingsColumnWidth => (_settingsViewModel == null) ? GridLength.Auto : new GridLength(0.2, GridUnitType.Star);
        public int SettingsColMinWidth => (_settingsViewModel == null) ? 0 : 200;
        private object? _settingsViewModel;
        public Visibility IsSettingsOpen => (_settingsViewModel == null) ? Visibility.Collapsed : Visibility.Visible;
        public bool IsApplicationSettingsOpen
        {
            get { return (SettingsViewModel != null && SettingsViewModel is ApplicationSettingsViewModel); }
            set
            {
                if (value)
                {
                    SettingsViewModel = ApplicationSettingsViewModel.Instance;
                    OnPropertyChanged(nameof(IsSongSettingsOpen));
                }
                else if (!value && SettingsViewModel is ApplicationSettingsViewModel)
                {
                    SettingsViewModel = null;
                    OnPropertyChanged(nameof(IsSettingsOpen));
                }
                OnPropertyChanged();
            }
        }
        public bool IsSongSettingsOpen
        {
            get { return (SettingsViewModel != null && SettingsViewModel is SongSettingsViewModel); }
            set
            {
                if (value)
                {
                    SettingsViewModel = Song.SongSettings;
                    OnPropertyChanged(nameof(IsApplicationSettingsOpen));
                }
                else if (!value && SettingsViewModel is SongSettingsViewModel)
                {
                    SettingsViewModel = null;
                    OnPropertyChanged(nameof(IsSettingsOpen));
                }
                OnPropertyChanged();
            }
        }

        public object? SettingsViewModel
        {
            get { return _settingsViewModel; }
            set
            {
                _settingsViewModel = value;
                // TODO the size of the settings row can still be resized to show a blank row
                OnPropertyChanged(nameof(IsSettingsOpen));
                OnPropertyChanged(nameof(SettingsColumnWidth));
                OnPropertyChanged(nameof(SettingsColMinWidth));
                OnPropertyChanged();
            }
        }

        public Action<string> OnOptionsUpdate => UpdateTitleSuggestions;
        public Action<string, string> OnOptionSelected => OnTitleSelected;
        public ICommand AddSongCommand => new AddSongCommand(this);
        public ICommand RemoveSongCommand { get; }
        public ICommand CreatePresentation => new CreatePresentationCommand(this);


        public MainWindowViewModel()
        {
            LoadSongSuggestions();

            Song starterSong = new Song();
            SongDetails = new ObservableCollection<SongDetailsViewModel>();
            SongDetails.Add(new SongDetailsViewModel(starterSong));
            SelectedSong = SongDetails[0];

            //Song testSong1 = new Song(
            //        "The Steadfast Love of the Lord",
            //        "English",
            //        "The steadfast love of the Lord never ceases, \n" +
            //        "His mercies never come to an end \n" +
            //        "They are new every morning, new every morning \n" +
            //        "Great is Thy faithfulness O Lord \n" +
            //        "Great is Thy faithfulness \n",
            //        ApplicationSettingsViewModel.Instance.defaultSongSettings
            //);
            //SongDetails.Add(new SongDetailsViewModel(testSong1));
        }

        private async void LoadSongSuggestions()
        {
            // handle errors
            titleSuggestions = await DriveService.Instance.GetFileListAsync(query: $"'{ApplicationSettingsViewModel.Instance.DriveFolderId}'+in+parents+and+trashed=false");
            TitleSuggestions = titleSuggestions.Keys.ToList();
        }

        public void UpdateTitleSuggestions(string input)
        {
            if (input.Length < 1)
                TitleSuggestions = titleSuggestions.Keys.ToList();
            else
            {
                var suggestions = titleSuggestions.Keys.Where(s => s.IndexOf(input, StringComparison.InvariantCultureIgnoreCase) != -1)
                    .ToList();
                if (suggestions.Count > 0)
                    TitleSuggestions = suggestions;
                else
                {
                    TitleSuggestions = new List<string>
                    {
                        "create song: "+input,
                        "edit title: "+TitleText
                    };
                }
            }
        }


        public async void OnTitleSelected(string selectedOption,string? inputText)
        {
            if (selectedOption==null || selectedOption.Length <1 || selectedOption.StartsWith("create song:"))  
            {
                Song s = new Song();
                s.SongName = inputText ?? "";
                s.Lang1 = Song.Lang1;
                s.Lang2 = Song.Lang2;
                AddSongCommand.Execute(s);
            } 
            else if (selectedOption.StartsWith("edit title:"))
            {
                SelectedSong.Title = inputText ?? "";
            } else
            {
                if (!titleSuggestions.ContainsKey(selectedOption))
                {
                    
                    SelectedSong.Title = inputText ?? "";
                    return;
                }
                SongSuggestion selectedSuggestion = titleSuggestions[selectedOption];
                if (SelectedSong == null)
                {
                    Song s = new();
                    s.SongName = selectedSuggestion.Name;
                    s.Lang1 = Song.Lang1;
                    s.Lang2 = Song.Lang2;
                    AddSongCommand.Execute(new SongDetailsViewModel(s));
                    Song = new SongViewModel(s);
                    Song.AddSuggestion(selectedSuggestion);
                }
                else if (selectedSuggestion.FileId != null)
                {
                    SelectedSong.Title = selectedOption;
                    Song.Title = selectedSuggestion.Name;
                    Song.AddSuggestion(selectedSuggestion);

                    // run in parallel
                    if (selectedSuggestion.Languages.ContainsKey(Song.Lang1))
                    {
                        var res = await DriveService.Instance.GetSongLyricsAsync(selectedSuggestion.Languages[Song.Lang1]);
                        Song.Text1 = res;
                    }
                    if (Song.Lang2 != null && selectedSuggestion.Languages.ContainsKey(Song.Lang2))
                    {
                        var res = await DriveService.Instance.GetSongLyricsAsync(selectedSuggestion.Languages[Song.Lang2]);
                        Song.Text2 = res;
                    }
                }
            }
        }
    }
}
