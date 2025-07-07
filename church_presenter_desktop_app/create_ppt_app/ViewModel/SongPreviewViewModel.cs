using create_ppt_app.MVVM;

namespace create_ppt_app.ViewModel
{
    public class SongPreviewViewModel : ViewModelBase
    {
        public ApplicationSettingsViewModel ApplicationSettings => ApplicationSettingsViewModel.Instance;

        public SongSettingsViewModel SongSettings { get; set; }


        public string Text1 { get; set; }
        public string Text2 { get; set; }

        public string BackgroundImagePath
        {
            get
            {
                if (SongSettings.Background.Length > 0)
                    return SongSettings.Background;
                else
                    return "";
            }
        }

        public SongPreviewViewModel(string text1, string text2, SongSettingsViewModel songSettings)
        {
            Text1 = text1;
            Text2 = text2;
            SongSettings = songSettings;
        }
    }
}
