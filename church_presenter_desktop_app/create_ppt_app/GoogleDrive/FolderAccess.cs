using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Diagnostics;

public class FolderAccess
{
    private readonly HttpClient httpClient;
    private readonly Authentication auth;

    public enum FolderPermission
    {
        ReadOnly,
        ReadWrite,
        Forbidden,
    }

    public FolderAccess(Authentication auth)
    {
        this.httpClient = new HttpClient();
        this.auth = auth;
    }


    public async Task<FolderPermission> CheckFolderPermissionsAsync(string folderUrl)
    {
       
        string folderId = ExtractFolderId(folderUrl);
        Debug.WriteLine("folder id" + folderId);

        if (auth.LoadSavedToken())
        {
            Debug.Print("Logged in, checking permission");
            // Get authenticated access
            string accessToken = await auth.GetAccessTokenAsync();
            FolderPermission authenticatedPermissions = await CheckAuthenticatedPermissionsAsync(folderId, accessToken);
            return authenticatedPermissions;
        }
       
        Debug.Print("Not Logged in");
        // create an exception for user not logged in
        throw new Exception("Not logged in");
    }


    private async Task<FolderPermission> CheckAuthenticatedPermissionsAsync(string folderId, string accessToken)
    {
        try
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // First verify the file exists and we can access it
            var metadataRequest = new HttpRequestMessage(HttpMethod.Get,
                $"https://www.googleapis.com/drive/v3/files/1qVfC3niMEr8E4iVxSElikAlBP8G1RXWw/permissions");

            var response = await httpClient.GetAsync($"https://www.googleapis.com/drive/v3/files/1qVfC3niMEr8E4iVxSElikAlBP8G1RXWw/permissions");
            string errorJson = await response.Content.ReadAsStringAsync(); // Get the error JSON
            var metadataResponse = await httpClient.SendAsync(metadataRequest);
            Debug.Print(metadataRequest.ToString());
            Debug.Print(metadataResponse.ToString());

            if (metadataResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Debug.WriteLine($"Error: {response.StatusCode} - {errorJson}");
                //throw new Exception($"Folder not found. Please check if the folder ID is correct: {folderId}");
            }

            if (!metadataResponse.IsSuccessStatusCode)
            {
                var errorContent = await metadataResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to access folder: {metadataResponse.StatusCode}\nError: {errorContent}");
            }

            // Now check permissions
            var permRequest = new HttpRequestMessage(HttpMethod.Get,
                $"https://www.googleapis.com/drive/v3/files/{folderId}?fields=capabilities");

            var permResponse = await httpClient.SendAsync(permRequest);

            if (!permResponse.IsSuccessStatusCode)
            {
                var errorContent = await permResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get folder permissions: {permResponse.StatusCode}\nError: {errorContent}");
            }

            var content = await permResponse.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(content);
            var capabilities = data.GetProperty("capabilities");

            bool canView = capabilities.GetProperty("canReadRevisions").GetBoolean();
            bool canEdit = capabilities.GetProperty("canEdit").GetBoolean();

            if (canEdit)
            {
                return FolderPermission.ReadWrite;
            }
            else if (canView)
            {
                return FolderPermission.ReadOnly;
            }
            else
            {
                return FolderPermission.Forbidden;
            }
        }
        catch (JsonException)
        {
            throw new Exception("Failed to parse folder permissions response");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Network error while checking permissions: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking folder permissions: {ex.Message}");
        }
    }

    public string ExtractFolderId(string folderUrl)
    {
        // Handle different URL formats
        string[] patterns = new[]
        {
            @"folders/([a-zA-Z0-9-_]+)",                    // https://drive.google.com/drive/folders/YOUR_FOLDER_ID
            @"id=([a-zA-Z0-9-_]+)",                        // https://drive.google.com/drive/folders?id=YOUR_FOLDER_ID
            @"drive\.google\.com/.*?/([a-zA-Z0-9-_]+)/?$"  // Other possible formats
        };

        foreach (string pattern in patterns)
        {
            var match = Regex.Match(folderUrl, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }

        throw new ArgumentException("Invalid Google Drive folder URL format");
    }
}