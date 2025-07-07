using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace create_ppt_app.Model
{
    public class SongSuggestion
    {
        /// <summary>
        /// FileId is the unique identifier for the file
        /// For drive files it is the file id
        /// For local files it is the path
        /// </summary>
        public string? FileId;
        public string Name { get; set; }
        public Dictionary<string, string> Languages { get; set; } = new Dictionary<string, string>();
        //optional metadata with song level defaults 


        public SongSuggestion(string FileId,string FileName)
        {
            this.FileId = FileId;
            FileName = FileName.Substring(0, FileName.LastIndexOf('.'));
            // handle exception if filename does not follow pattern
            int pos = FileName.LastIndexOf('_');
            Name = FileName.Substring(0, pos);
            string language = FileName.Substring(pos + 1);
            language = Char.ToUpper(language[0]) + FileName.Substring(pos + 2);
            Languages.Add(language, FileId);
        }

        public void AddLanguage(string FileId, string FileName)
        {
            FileName = FileName.Substring(0, FileName.LastIndexOf('.'));
            int pos = FileName.LastIndexOf('_');
            string language = FileName.Substring(pos + 1);
            language = Char.ToUpper(language[0]) + FileName.Substring(pos + 2);
            Languages.Add(language, FileId);
        }

        public static bool isSameName(string filename1, string filename2)
        {
            if (filename1.Equals(filename2))
                return true;
            
            int p1 = filename1.LastIndexOf("_");
            if (p1 != -1 && filename1.Substring(0, p1).Equals(filename2))
                return true;

            int p2 = filename2.LastIndexOf("_");
            if (p2 != -1 && filename2.Substring(0, p2).Equals(filename1))
                return true;

            if(p1 != -1 && p2 != -1 && filename1.Substring(0,p1).Equals(filename2.Substring(0,p2)))
                return true;

            return false;
        }
    }
}
