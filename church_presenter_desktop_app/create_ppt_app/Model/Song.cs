using create_ppt_app.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Printing.IndexedProperties;

namespace create_ppt_app.Model
{
    public class Song
    {
        public string SongName { get; set; } = "";
        private string lang1 = "English";
        public string Lang1 { 
            get { return lang1; }
            set
            {
                if (value != null || value.Length > 1)
                    lang1 = value;
            }
        }
        public string? Lang2 { get; set; }

        public string Text1 { get; set; } = "";
        public string? Text2 { get; set; }

        public ObservableCollection<SongPreviewViewModel> previewSlides { get; set; }

        public SongSettingsViewModel settings;

        public Song() {
            //this.settings = ApplicationSettingsViewModel.Instance.defaultSongSettings;
            this.settings = new SongSettingsViewModel(ApplicationSettingsViewModel.Instance.defaultSongSettings);
            previewSlides = new ObservableCollection<SongPreviewViewModel>();
        }

        //public Song(string name, string l1, string txt1, SongSettingsViewModel settings)
        //{
        //    SongName = name;
        //    Lang1 = l1;
        //    Text1 = txt1;
        //    this.settings = settings;
        //}
        //public Song(string name, string l1, string txt1, string l2, string txt2,SongSettingsViewModel settings)
        //{
        //    SongName = name;
        //    Lang1 = l1;
        //    Text1 = txt1;
        //    Lang2 = l2;
        //    Text2 = txt2;
        //    this.settings = settings;
        //}
    }
}
