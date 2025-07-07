using Common;
using create_ppt_app.Command;
using create_ppt_app.GoogleDrive;
using create_ppt_app.Model;
using create_ppt_app.MVVM;
using create_ppt_app.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace create_ppt_app.ViewModel
{
    public class SongViewModel : ViewModelBase
    {
        private Song _song;
        public string Title
        {
            get
            {
                return _song.SongName;
            }
            set
            {
                _song.SongName = value;
                if (songSuggestion != null)
                    songSuggestion.Name = _song.SongName;
                       
                OnPropertyChanged();
            }
        }

        private SongSuggestion songSuggestion;
        public List<String> LanguageSuggestions
        {
            get => songSuggestion?.Languages.Keys.ToList() ?? new List<string> { "Add language: No lyrics available for this song" };
        }

        public string Lang1
        {
            get
            { return _song.Lang1; }
            set
            {
                _song.Lang1 = value;
                OnPropertyChanged();
            }
        }
        public string? Lang2
        {
            get
            { return _song.Lang2; }
            set
            {
                _song.Lang2 = value;
                OnPropertyChanged();
            }
        }
        
        private CancellationTokenSource? _cts;
        private const int GENERATE_PREVIEW_DELAY = 2000;

        public string Text1
        {
            get { return _song.Text1; }
            set
            {
                _song.Text1 = value;
                OnPropertyChanged();
                DebounceTextChanged();
            }
        }
        public string? Text2
        {
            get { return _song.Text2; }
            set
            {
                _song.Text2 = value;
                OnPropertyChanged();
                DebounceTextChanged();
            }
        }

        public Action<string, string> OnLang1Selected => OnLang1Update;
        public Action<string, string> OnLang2Selected => OnLang2Update;
        public SongSettingsViewModel SongSettings { get; set; }
        public ObservableCollection<SongPreviewViewModel> PreviewSlides { get; set; }

        public SongViewModel(Song song)
        {
            _song = song;
            SongSettings = song.settings;
            SongSettings.SettingChanged += (_, _) => SongSettings_PropertyChanged();
            PreviewSlides = song.previewSlides;
        }

        private void SongSettings_PropertyChanged()
        {
            try
            {
                GeneratePreview();
            } catch
            {
                Debug.Print("preview error");
            }
        }

        public void AddSuggestion(SongSuggestion suggestion)
        {
            songSuggestion = suggestion;
            OnPropertyChanged(nameof(LanguageSuggestions));
        }

        public void OnLang1Update(string selectedOption, string? inputText)
        {
            LangUpdate(1, selectedOption, inputText);
        }
        public void OnLang2Update(string selectedOption, string? inputText)
        {
            LangUpdate(2, selectedOption, inputText);
        }
        public async void LangUpdate(int lang, string selectedOption, string? inputText)
        {
            if (selectedOption == null || selectedOption.Length < 1 || selectedOption.StartsWith("Add language:"))
            {
                //show error, keep blank
            }
            else
            {
                if (songSuggestion != null && songSuggestion.Languages.ContainsKey(selectedOption))
                {
                    if (lang == 1)
                    {
                        Lang1 = selectedOption;
                        var res = await DriveService.Instance.GetSongLyricsAsync(songSuggestion.Languages[selectedOption]);
                        Text1 = res;
                    } else if(lang == 2)
                    {
                        Lang2 = selectedOption;
                        var res = await DriveService.Instance.GetSongLyricsAsync(songSuggestion.Languages[selectedOption]);
                        Text2 = res;
                    }
                }
            }
        }

        public void GeneratePreview()
        {
            Debug.Print("generate preview");
            PreviewSlides.Clear();
            if (Enum.TryParse<SlideSeparation>(SongSettings.SeparateSlideBy, ignoreCase: true, out var result))
            {
                List<string> Text1Slides, Text2Slides;
                if (result == SlideSeparation.Lines)
                {
                    Text1Slides = SongSeparator.SeparateSongByLines(Text1, SongSettings.LinesPerSlide);
                    Text2Slides = SongSeparator.SeparateSongByLines(Text2, SongSettings.LinesPerSlide);
                }
                else
                {
                    Text1Slides = SongSeparator.SeparateSongBySymbol(Text1, SongSettings.SlideSeparatorSymbol);
                    Text2Slides = SongSeparator.SeparateSongBySymbol(Text2, SongSettings.SlideSeparatorSymbol);
                }

                for (int i = 0; i < Math.Max(Text1Slides.Count, Text2Slides.Count); i++)
                {
                    string t1 = (Text1Slides.Count > i) ? Text1Slides[i] : "";
                    string t2 = (Text2Slides.Count > i) ? Text2Slides[i] : "";
                    if (t1 == "" && t2 == "")
                        continue;
                    PreviewSlides.Add(new SongPreviewViewModel(
                           t1,
                           t2,
                           SongSettings
                       ));
                }

                _song.previewSlides = PreviewSlides;
            } 
        }

        private async void DebounceTextChanged()
        {
            _cts?.Cancel(); // cancel previous delay if user is still typing
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(GENERATE_PREVIEW_DELAY, _cts.Token); 
                // Delay complete — user has stopped typing
                GeneratePreview();
            }
            catch (TaskCanceledException)
            {
                // Typing resumed — previous delay cancelled
            }
        }
    }
}
