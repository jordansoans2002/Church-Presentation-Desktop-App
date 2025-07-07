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

            presentationDTO.Settings = new Dictionary<string, string>();
            foreach (var setting in ApplicationSettingsViewModel.Instance.SettingsList)
            {
                presentationDTO.Settings[setting.SettingName] = setting.SettingValue;
            }

            presentationDTO.Songs = new List<SongDTO>();
            foreach (var song in _mainWindowViewModel.SongDetails)
            {
                presentationDTO.Songs.Add(ToDTO(song.song));
            }

            var json = JsonSerializer.Serialize(presentationDTO, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Debug.WriteLine(json);

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.PostAsync("http://127.0.0.1:8080/api/Presentation/create-presentation", content);

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

        public static SongDTO ToDTO(Song song) => new SongDTO
        {
            Title = song.SongName,
            Lang1 = song.Lang1,
            Lang2 = song.Lang2,
            Text1 = song.Text1,
            Text2 = song.Text2,
            Settings = song.settings.SettingsList.ToDictionary(
                kvp => kvp.SettingName,
                kvp => kvp.SettingValue
             )
        };

    }
}
