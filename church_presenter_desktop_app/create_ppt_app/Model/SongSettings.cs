using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace create_ppt_app.Model
{
    public class SongSettings
    {
        public SlideSeparation SeparateSlideBy { get; set; } = SlideSeparation.Symbol;
        public int LinesPerSlide { get; set; } = 2;
        public string SlideSeparatorSymbol { get; set; } = "\n\n";
        public string Stanzas { get; set; } = "all";
        
        public Orientation Orientation { get; set; } = Orientation.Horizontal;
        public int MarginStart { get; set; } = 12;
        public int MarginTop { get; set; } = 12;
        public int MarginEnd { get; set; } = 0;
        public int MarginBottom { get; set; } = 0;

        public string TitleBackground { get; set; } = "#FFFFFF"; // if path is image if hex then color
        public string Background { get; set; } = ""; // if path is image if hex then color
        //    <Grid.Background>
        //        <ImageBrush ImageSource = "{Binding BackgroundImagePath}" Opacity="{Binding BackgroundImageOpacity}" Stretch="UniformToFill" />
        //    </Grid.Background>
        public int BackgroundOpacity { get; set; } = 100;
        public string Text1Background { get; set; } = "#111111";
        public string Text2Background { get; set; } = "#CDCDCD";
        public FontSettings TitleFontSettings { get; set; } = new();

        public FontSettings Lang1FontSettings { get; set; } = new FontSettings();
        public FontSettings Lang2FontSettings { get; set; } = new FontSettings();
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }
    public enum SlideSeparation
    {
        Lines,
        Symbol
    }
    public class FontSettings
    {
        public int FontSize { get; set; } = 36;
        public string FontName { get; set; } = "Times New Roman";
        public string FontColor { get; set; } = "#FFFFFF";
    }
}
