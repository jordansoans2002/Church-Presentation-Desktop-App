
using System.Collections.Generic;

namespace Common.Models.Requests
{
    public class CreatePresentationRequest
    {
        public GlobalSettings Settings { get; set; } = new GlobalSettings();
        public List<SongDTO> Songs { get; set; } = new List<SongDTO>();
    }

    public class GlobalSettings
    {
        public string SlideRatio { get; set; }
        public string Unit { get; set; }
        public TextStyle TitleStyle { get; set; } = new TextStyle();
    }

    public class SongDTO
    {
        public string Title { get; set; }
        public SongText Text { get; set; } = new SongText();
        public SongSettings Settings { get; set; } = new SongSettings();
    }

    public class SongText
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
    }

    public class SongSettings
    {
        public Separation Separation { get; set; } = new Separation();
        public string Orientation { get; set; }
        public List<int> Stanzas { get; set; } = new List<int>();
        public Padding Padding { get; set; } = new Padding();
        public TextStyle Text1Style { get; set; } = new TextStyle();
        public TextStyle Text2Style { get; set; } = new TextStyle();
        public Background Background { get; set; } = new Background();
    }

    public class Separation
    {
        public string Symbol { get; set; }
        public int? Lines { get; set; }
    }

    public class Padding
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Gap { get; set; }
    }

    public class TextStyle
    {
        public string FontFamily { get; set; }
        public int? FontSize { get; set; }
        public string FontColor { get; set; }
        public string Align { get; set; }
        public string Valign { get; set; }
    }

    public class Background
    {
        public string Color { get; set; }
        public double Opacity { get; set; }
    }
}