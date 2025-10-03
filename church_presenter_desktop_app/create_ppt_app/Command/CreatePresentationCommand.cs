using Common.Models.Requests;
using create_ppt_app.Model;
using create_ppt_app.MVVM;
using create_ppt_app.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace create_ppt_app.Command
{
    internal class CreatePresentationCommand : AsyncCommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public CreatePresentationCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            CreatePresentationRequest presentationDTO = new();

            //presentationDTO.Settings = new GlobalSettings();

            //foreach (var setting in ApplicationSettingsViewModel.Instance.SettingsList)
            //{
            //    switch (setting.SettingName)
            //    {
            //        case "slideRatio":
            //            presentationDTO.Settings.SlideRatio = setting.SettingValue;
            //            break;
            //        case "unit":
            //            presentationDTO.Settings.Unit = setting.SettingValue;
            //            break;
            //        case "titleFontFamily":
            //            if (presentationDTO.Settings.TitleStyle == null)
            //                presentationDTO.Settings.TitleStyle = new TextStyle();
            //            presentationDTO.Settings.TitleStyle.FontFamily = setting.SettingValue;
            //            break;
            //        case "titleFontSize":
            //            if (presentationDTO.Settings.TitleStyle == null)
            //                presentationDTO.Settings.TitleStyle = new TextStyle();
            //            presentationDTO.Settings.TitleStyle.FontSize = int.Parse(setting.SettingValue);
            //            break;
            //        case "titleFontColor":
            //            if (presentationDTO.Settings.TitleStyle == null)
            //                presentationDTO.Settings.TitleStyle = new TextStyle();
            //            presentationDTO.Settings.TitleStyle.FontColor = setting.SettingValue;
            //            break;
            //    }
            //}

            presentationDTO.Songs = new List<SongDTO>();
            foreach (var song in _mainWindowViewModel.SongDetails)
            {
                presentationDTO.Songs.Add(ToDTO(song.song));
            }

            var json = JsonSerializer.Serialize(presentationDTO, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                // For .NET 5+, you can also ignore default values:
                // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            });


            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Debug.WriteLine(json);

            using var httpClient = new HttpClient();
            try
            {
                // make primary call to remote server
                // if remote server is unreachable (5xx) error fallback to local
                var response = await httpClient.PostAsync("http://127.0.0.1:3001/api/generate-pptx", content);

                Debug.WriteLine(response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();

                    string downloadsPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "Downloads"
                    );

                    // Open a SaveFileDialog to prompt the user
                    var saveFileDialog = new SaveFileDialog
                    {
                        FileName = "lyrics_presentation.pptx",
                        InitialDirectory = Directory.Exists(downloadsPath) ? downloadsPath : null,
                        Filter = "PowerPoint Presentation (*.pptx)|*.pptx",
                        DefaultExt = ".pptx"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        await File.WriteAllBytesAsync(saveFileDialog.FileName, fileBytes);
                        Debug.WriteLine($"File saved to: {saveFileDialog.FileName}");
                    }
                    else
                    {
                        Debug.WriteLine("Save cancelled by user.");
                    }
                }
                else
                {
                    Debug.WriteLine("Failed to generate presentation.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        //public static SongDTO ToDTO(Song song) => new SongDTO
        //{
        //    Title = song.SongName,
        //    Lang1 = song.Lang1,
        //    Lang2 = song.Lang2,
        //    Text1 = song.Text1,
        //    Text2 = song.Text2,
        //    Settings = song.settings.SettingsList.ToDictionary(
        //        kvp => kvp.SettingName,
        //        kvp => kvp.SettingValue
        //     )
        //};

        public static SongDTO ToDTO(Song song)
        {
            var s = song.settings;

            return new SongDTO
            {
                Title = song.SongName,
                Text = new SongText
                {
                    Text1 = song.Text1,
                    Text2 = song.Text2
                },
                Settings = new Common.Models.Requests.SongSettings
                {
                    Separation = new Separation
                    {
                        Symbol = s.SlideSeparatorSymbol,
                        Lines = s.LinesPerSlide
                    },
                    Orientation = s.Orientation,
                    Stanzas = ParseStanzas(s.Stanzas),
                    Padding = new Padding
                    {
                        Left = s.MarginStart,
                        Top = s.MarginTop,
                        Right = s.MarginEnd,
                        Bottom = s.MarginBottom,
                        Gap = 12 // or expose from settings if configurable
                    },
                    Text1Style = new TextStyle
                    {
                        FontFamily = s.Text1FontName,
                        FontSize = s.Text1FontSize,
                        FontColor = s.Text1FontColor,
                        Align = "center",   // could be bound from settings
                        Valign = "center"
                    },
                    Text2Style = new TextStyle
                    {
                        FontFamily = s.Text2FontName,
                        FontSize = s.Text2FontSize,
                        FontColor = s.Text2FontColor,
                        Align = "center",
                        Valign = "center"
                    },
                    Background = new Background
                    {
                        Color = s.Background,
                        Opacity = s.BackgroundOpacity / 100.0 // if stored as %
                    }
                }
            };
        }

        private static List<int> ParseStanzas(string stanzas)
        {
            if (string.IsNullOrWhiteSpace(stanzas))
                return new List<int>();

            return stanzas
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n.Value)
                .ToList();
        }

    }
}
