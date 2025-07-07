using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Wpf;
using System.Text.Json;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Web.WebView2.Core;
using static create_ppt_app.GoogleDrive.DriveService;
using create_ppt_app.GoogleDrive;
using System.Diagnostics;

public class Authentication
{
    private static readonly Lazy<Authentication> _instance =
        new Lazy<Authentication>(() => new Authentication());
    public static Authentication Instance => _instance.Value;
    private Authentication()
    {
        httpClient = new HttpClient();
    }

    private const string REDIRECT_URI = "http://localhost:8080/oauth2callback";
    private const string AUTH_ENDPOINT = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TOKEN_ENDPOINT = "https://oauth2.googleapis.com/token";
    private const string SCOPE = "https://www.googleapis.com/auth/drive https://www.googleapis.com/auth/userinfo.email"; // to get name and account dp https://www.googleapis.com/auth/userinfo.profile";

    private string accessToken;
    private string refreshToken;
    private DateTime tokenExpiry;
    private readonly string tokenPath = "token.json";
    private Window authWindow;
    private WebView2 webView;
    private TaskCompletionSource<string> authCodeTask;
    private HttpClient httpClient;

   

    public async Task<string> GetAccessTokenAsync(bool forceNewToken = false)
    {
        // Check if we have a valid cached token
        if (!forceNewToken && !string.IsNullOrEmpty(accessToken) && DateTime.Now < tokenExpiry)
        {
            Debug.Print(accessToken);
            return accessToken;
        }

        // Try to load saved refresh token
        if (!forceNewToken && LoadSavedToken())
        {
            try
            {
                await RefreshAccessTokenAsync();
                Debug.Print(accessToken);
                return accessToken;
            }
            catch
            {
                // If refresh fails, continue with new authentication
            }
        }

        await AuthenticateAsync();
        Debug.Print(accessToken);
        return accessToken;
    }

    public async Task AuthenticateAsync()
    {
        string authCode = await GetAuthorizationCodeAsync();
        await ExchangeCodeForTokensAsync(authCode);
        SaveToken();

        await DriveService.Instance.GetUserInfoAsync();
    }   

    private async Task<string> GetAuthorizationCodeAsync()
    {
        authCodeTask = new TaskCompletionSource<string>();

        Application.Current.Dispatcher.Invoke(() =>
        {
            authWindow = new Window
            {
                Title = "Google Drive Authentication",
                Width = 800,
                Height = 600
            };

            webView = new WebView2();
            webView.NavigationStarting += WebView_NavigationStarting;
            authWindow.Content = webView;

            string authUrl = $"{AUTH_ENDPOINT}?client_id={Credentials.CLIENT_ID}&redirect_uri={REDIRECT_URI}" +
                           $"&response_type=code&scope={SCOPE}&access_type=offline&prompt=consent";

            webView.Source = new Uri(authUrl);
            authWindow.Show();
        });

        string authCode = await authCodeTask.Task;

        Application.Current.Dispatcher.Invoke(() =>
        {
            authWindow.Close();
        });


        return authCode;
    }

    private async void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
    {
        string url = e.Uri;
        if (url.StartsWith(REDIRECT_URI))
        {
            Uri uri = new Uri(url);
            string code = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("code");
            if (!string.IsNullOrEmpty(code))
            {
                e.Cancel = true;
                authCodeTask.SetResult(code);
            }
        }
    }

    private async Task ExchangeCodeForTokensAsync(string authCode)
    {
        var tokenRequest = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("client_id", Credentials.CLIENT_ID),
            new KeyValuePair<string, string>("client_secret", Credentials.CLIENT_SECRET),
            new KeyValuePair<string, string>("redirect_uri", REDIRECT_URI),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        });

        var response = await httpClient.PostAsync(TOKEN_ENDPOINT, tokenRequest);
        var content = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<JsonElement>(content);

        accessToken = tokenData.GetProperty("access_token").GetString();
        refreshToken = tokenData.GetProperty("refresh_token").GetString();
        int expiresIn = tokenData.GetProperty("expires_in").GetInt32();
        tokenExpiry = DateTime.Now.AddSeconds(expiresIn);
    }

    private async Task RefreshAccessTokenAsync()
    {
        var refreshRequest = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", Credentials.CLIENT_ID),
            new KeyValuePair<string, string>("client_secret", Credentials.CLIENT_SECRET),
            new KeyValuePair<string, string>("grant_type", "refresh_token")
        });

        var response = await httpClient.PostAsync(TOKEN_ENDPOINT, refreshRequest);
        var content = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<JsonElement>(content);

        accessToken = tokenData.GetProperty("access_token").GetString();
        int expiresIn = tokenData.GetProperty("expires_in").GetInt32();
        tokenExpiry = DateTime.Now.AddSeconds(expiresIn);
    }

    private void SaveToken()
    {
        var tokenData = new
        {
            refresh_token = refreshToken,
            expiry = tokenExpiry
        };

        string json = JsonSerializer.Serialize(tokenData);
        File.WriteAllText(tokenPath, json);
    }

    public bool LoadSavedToken()
    {
        if (!File.Exists(tokenPath))
            return false;

        try
        {
            string json = File.ReadAllText(tokenPath);
            var tokenData = JsonSerializer.Deserialize<JsonElement>(json);
            refreshToken = tokenData.GetProperty("refresh_token").GetString();
            tokenExpiry = tokenData.GetProperty("expiry").GetDateTime();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
