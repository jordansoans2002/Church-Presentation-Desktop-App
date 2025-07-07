using create_ppt_app.Model.DriveDTO;
using create_ppt_app.MVVM;
using create_ppt_app.ViewModel;
using GoogleDrivePickerWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace create_ppt_app.Command
{
    internal class SelectDriveFolder : CommandBase
    {
        public override void Execute(object? parameter)
        {
            GetLyricsDriveFolder();
        }

        public override bool CanExecute(object? parameter)
        {
            return Authentication.Instance.LoadSavedToken();
        }

        private async void GetLyricsDriveFolder()
        {
            string accessToken = await Authentication.Instance.GetAccessTokenAsync();
            var pickerService = new FolderSelector(accessToken);
            try
            {
                var result = await pickerService.ShowPickerAsync();
                if (result.action == "picked")
                {
                    //ApplicationSettingsViewModel.Instance.DriveFolderName.driveFolderId = result.folderId;
                    ApplicationSettingsViewModel.Instance.DriveFolderId = result.folderId;
                    ApplicationSettingsViewModel.Instance.DriveFolderName = result.folderName;
                    Debug.Print($"Selected folder: {result.folderName} ({result.folderId})");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing picker: {ex.Message}");
            }
        }
    }
}
