using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using create_ppt_app.Model;
using System.Security.Policy;
using create_ppt_app.Model.DriveDTO;
using create_ppt_app.ViewModel;
using System.Collections;
using System.IO;

namespace create_ppt_app.GoogleDrive
{
    public class DriveService
    {
        private static readonly Lazy<DriveService> _instance =
        new Lazy<DriveService>(() => new DriveService());
        public static DriveService Instance => _instance.Value;
        private DriveService() { _httpClient = new HttpClient(); }

        private static HttpClient _httpClient;
        private const string BaseUrl = "https://www.googleapis.com/drive/v3";


        // call every time user has to log in
        public async Task<UserInfo> GetUserInfoAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", await Authentication.Instance.GetAccessTokenAsync());
                var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UserInfo>(content);

                ApplicationSettingsViewModel.Instance.UserAccount = result;
                return result;
            }
            catch
            {
                throw new NotImplementedException("see what error can be generated and show error toast");
            }
        }

        public async Task<Dictionary<string,SongSuggestion>> GetFileListAsync(string? pageToken = null, string? query = null)
        {
            try
            {
                var url = $"{BaseUrl}/files?fields=files(id,name)";
                if (!string.IsNullOrEmpty(query))
                {
                    url += $"&q={query}";
                }
                if (!string.IsNullOrEmpty(pageToken))
                {
                    url += $"&pageToken={pageToken}";
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", await Authentication.Instance.GetAccessTokenAsync());
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<FileListResponse>(content);

                Dictionary<string,SongSuggestion> songs = new Dictionary<string,SongSuggestion>();
                if (result?.files != null)
                {
                    foreach (var file in result.files)
                    {
                        int pos = file.name.LastIndexOf('_');
                        if(pos != -1)
                        {
                            string nm = file.name.Substring(0,pos);
                            if (!songs.ContainsKey(nm))
                            {
                                songs.Add(nm,new SongSuggestion(file.id, file.name));
                            } else
                            {
                                songs[nm].AddLanguage(file.id, file.name);
                            }
                        }
                    }
                }
                
                return songs;
            }
            catch (Exception ex)
            {
                // TODO probably throw error and show toast
                MessageBox.Show($"Error fetching files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new Dictionary<string, SongSuggestion>();
            }
        }


        public async Task<string> GetSongLyricsAsync(string fileId)
        {
            try
            {
                string accessToken = await Authentication.Instance.GetAccessTokenAsync(); // Your OAuth 2.0 logic

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    string downloadUrl = $"https://www.googleapis.com/drive/v3/files/{fileId}?alt=media";

                    using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                            return null; // Or throw an exception
                        }

                        // 1. Get as byte array (for smaller files):
                        byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                        string text = Encoding.Unicode.GetString(fileBytes);
                        return text;

                        // 2. Get as MemoryStream (for larger files - more efficient):
                        //using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        //using (var memoryStream = new MemoryStream())
                        //{
                        //    await streamToReadFrom.CopyToAsync(memoryStream);
                        //    return memoryStream.ToArray(); // Get the byte array from the MemoryStream
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null; // Or throw an exception
            }
        }
    }
}
