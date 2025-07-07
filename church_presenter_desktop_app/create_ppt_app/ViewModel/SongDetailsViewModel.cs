using create_ppt_app.Model;
using create_ppt_app.MVVM;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;


namespace create_ppt_app.ViewModel
{
    public class SongDetailsViewModel : ViewModelBase
    {
        public Song song { get; private set; }
        
        public string Title
        {
            get 
            {
             if (song.SongName.Length == 0)
                    return "Untitled song";
                else
                    return song.SongName; 
            }
            set
            {
                song.SongName = value;
                OnPropertyChanged();
            }
        }
        public string Lang1
        {
            get
            { return song.Lang1; }
            set
            {
                song.Lang1 = value;
                OnPropertyChanged();
                OnPropertyChanged("Languages");
            }
        }
        public string? Lang2
        {
            get
            { return song.Lang2; }
            set
           {
                song.Lang2 = value;
                OnPropertyChanged();
                OnPropertyChanged("Languages");
            }
        }
        public string Languages
        {
            get
            {
                if (song.Lang2 != null && song.Lang2.Length > 0)
                {
                    return "Languages: " + song.Lang1 + ", " + song.Lang2;
                }
                else
                {
                    return "Language: " + song.Lang1;
                }
            }
        }
        public string Text1
        {
            get { return song.Text1; }
            set
            {
                song.Text1 = value;
                OnPropertyChanged();
            }
        }
        public bool? isText1Edited { get; set; } = false;
        public string? Text2
        {
            get { return song.Text2; }
            set
            {
                song.Text2 = value;
                OnPropertyChanged();
            }
        }
        public bool? isText2Edited { get; set; } = false;

        public string? Orientation
        {
            get {
                if (song.settings.Orientation != null)
                    return "Orientation: " + song.settings.Orientation.ToString();
                else
                    return "";
            }
        }

        public SongDetailsViewModel(Song song)
        {
            this.song = song;
        }


        //public async void saveText(int text, bool isTemp=true)
        //{
        //    if (text == 2 && Text2 != null)
        //    {
        //        if (song.Text2 == Text2)
        //            return;
        //        using (StreamWriter outputFile = new StreamWriter(Text2Path!))
        //        {
        //            await outputFile.WriteAsync(Text2);
        //        }
        //    }
        //    else
        //    {
        //        if (song.Text1 == Text1)
        //            return;
        //        using (StreamWriter outputFile = new StreamWriter(Text1Path))
        //        {
        //            await outputFile.WriteAsync(Text1);
        //        }
        //    }
        //}
    }
}