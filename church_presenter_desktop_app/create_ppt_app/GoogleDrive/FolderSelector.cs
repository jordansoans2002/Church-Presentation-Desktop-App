using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using create_ppt_app.GoogleDrive;
using System.Windows;
using create_ppt_app.Model.DriveDTO;

namespace GoogleDrivePickerWpf
{
    public class FolderSelector : IDisposable
    {
        private readonly string accessToken;
        private readonly HttpListener listener;
        private readonly TaskCompletionSource<PickerResult> resultTask;
        private const int PORT = 3333;
        private readonly CancellationTokenSource cts;

        public FolderSelector(string accessToken)
        {
            this.accessToken = accessToken;
            this.listener = new HttpListener();
            this.resultTask = new TaskCompletionSource<PickerResult>();
            this.cts = new CancellationTokenSource();
        }

        public async Task<PickerResult> ShowPickerAsync()
        {
            listener.Prefixes.Add($"http://localhost:{PORT}/");
            listener.Start();

            _ = HandleRequestsAsync(); // Start request handling in the background

            var pickerUrl = $"http://localhost:{PORT}/picker";
            try
            {
                Process.Start(new ProcessStartInfo { FileName = pickerUrl, UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // Handle the case where the default browser is not found
                if (ex.ErrorCode == -2147467259) // ERROR_FILE_NOT_FOUND
                {
                    MessageBox.Show("No default browser found. Please set a default browser.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Error opening browser: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return null; // Or throw the exception if you prefer
            }


            try
            {
                return await resultTask.Task.WaitAsync(TimeSpan.FromMinutes(5), cts.Token);
            }
            catch (TimeoutException)
            {
                cts.Cancel(); // Cancel the request handling if timed out
                throw new TimeoutException("Folder selection timed out.");
            }
            catch (OperationCanceledException)
            {
                return null; // Or throw if you want to propagate the cancellation
            }
        }

        private async Task HandleRequestsAsync()
        {
            try
            {
                while (listener.IsListening && !cts.IsCancellationRequested) // Check for cancellation
                {
                    var context = await listener.GetContextAsync();

                    if (context.Request.Url.LocalPath == "/picker")
                    {
                        ServePickerPage(context);
                    }
                    else if (context.Request.Url.LocalPath == "/result")
                    {
                        await HandlePickerResult(context);
                        break; // Stop listening after receiving result
                    }
                }
            }
            catch (HttpListenerException ex)
            {
                // Handle exceptions that might occur during listener operations.
                if (ex.ErrorCode != 995) // "Operation Aborted" is usually due to listener close.
                {
                    resultTask.TrySetException(ex);
                }
            }
            catch (Exception ex)
            {
                resultTask.TrySetException(ex);
            }
        }

        private void ServePickerPage(HttpListenerContext context)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
<title>Select Google Drive Folder</title>
<script type='text/javascript' src='https://apis.google.com/js/api.js'></script>
</head>
<body>
    <script>
        function loadPicker() {{
            gapi.load('picker', {{ callback: showPicker }});
        }}

        function showPicker() {{
            const view = new google.picker.DocsView(google.picker.ViewId.FOLDERS)
                .setIncludeFolders(true)
                .setSelectFolderEnabled(true)
                .setMode(google.picker.DocsViewMode.LIST);

            const picker = new google.picker.PickerBuilder()
                .addView(view)
                .setOAuthToken('{accessToken}')
                .setDeveloperKey('{Credentials.API_KEY}')
                .setCallback(pickerCallback)
                .setOrigin(window.location.origin)
                .build();

            picker.setVisible(true);
            document.getElementById('status').style.display = 'none';
        }}

        function pickerCallback(data) {{
            if (data.action === google.picker.Action.PICKED) {{
                const folder = data.docs[0];
                sendResult({{
                    action: 'picked',
                    folderId: folder.id,
                    folderName: folder.name
                }});
            }} else if (data.action === google.picker.Action.CANCEL) {{
                sendResult({{ action: 'cancelled' }});
            }}
        }}

        function sendResult(result) {{
            fetch('/result', {{
                method: 'POST',
                headers: {{ 'Content-Type': 'application/json' }},
                body: JSON.stringify(result)
            }})
            .then(response => {{ // Check the response
                    if (!response.ok) {{
                        console.error(""Error sending result:"", response.status, response.statusText); // Log errors
                    }}
                }})
                .catch(error => {{
                    console.error(""Fetch error:"", error); // Catch fetch errors
                }})
                .then(() => window.close());
        }}

        loadPicker();
    </script>
</body>
</html>";

            var buffer = Encoding.UTF8.GetBytes(html);
            var response = context.Response;
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }


        private async Task HandlePickerResult(HttpListenerContext context)
        {
            using var reader = new System.IO.StreamReader(context.Request.InputStream);
            var json = await reader.ReadToEndAsync();

            // Important: Handle potential JSON parsing errors
            try
            {
                PickerResult? result = JsonSerializer.Deserialize<PickerResult>(json);
                if(result != null && result.action == "picked")
                    resultTask.TrySetResult(result);
            }
            catch (JsonException ex)
            {
                // Log the error or handle it appropriately. Perhaps send an error
                // response back to the client if you want to.
                Console.WriteLine($"JSON parsing error: {ex.Message}");
                resultTask.TrySetException(ex); // Set the exception so the caller knows.
            }

            var response = context.Response;
            var closeHtml = "<html><body><script>window.close();</script></body></html>";
            var buffer = Encoding.UTF8.GetBytes(closeHtml);
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        public void Dispose()
        {
            listener?.Close();
            cts?.Dispose(); // Dispose CancellationTokenSource
        }
    }
}