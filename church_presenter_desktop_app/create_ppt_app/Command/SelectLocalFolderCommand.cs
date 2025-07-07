using create_ppt_app.MVVM;
using create_ppt_app.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace create_ppt_app.Command
{
    class SelectLocalFolderCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            OpenFolderDialog folderDialog = new OpenFolderDialog();
            folderDialog.Title = "Select folder to store lyrics";
            bool? success = folderDialog.ShowDialog();
            if(success == true)
            {
                ApplicationSettingsViewModel.Instance.LocalFolder = folderDialog.FolderName;
            }
        }
    }
}
