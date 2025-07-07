using create_ppt_app.Model.DriveDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace create_ppt_app.Model
{
    public class ApplicationSettings
    {
        public int height = 540;
        public int width = 960;
        private SlideRatio _slideRatio = SlideRatio._16x9;
        public SlideRatio SlideRatio
        {
            get
            { return _slideRatio; }
            set
            {
                _slideRatio = value;
                var dimensions = _slideRatio.GetDimensions();
                height = dimensions.Height;
                width = dimensions.Width;
            }
        }

        public LyricsSource lyricsSource;
        public string accountName;
        public string accountEmail;
        public DriveFile driveFile = new DriveFile { id = "1qVfC3niMEr8E4iVxSElikAlBP8G1RXWw", name = "Song Lyrics" };
        public string localFolderPath; // set some default
        public bool editingEnabled;

        public SongSettings DefaultSongSettings = new();

    }
}
