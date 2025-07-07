
using System.Collections.Generic;

namespace Common.Models.Requests
{
    public class CreatePresentationRequest
    {
        public Dictionary<string, string> Settings { get; set; }
        public List<SongDTO> Songs { get; set; }
    }

    public class SongDTO
    {
        public string Title { get; set; }
        public string Lang1 { get; set; }
        public string Lang2 { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public Dictionary<string, string> Settings { get; set; }
    }
}
