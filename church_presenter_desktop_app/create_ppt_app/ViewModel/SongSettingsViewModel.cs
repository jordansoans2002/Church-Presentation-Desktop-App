using create_ppt_app.Model;
using create_ppt_app.MVVM;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace create_ppt_app.ViewModel
{
    public class SongSettingsViewModel : ViewModelBase
    {
        private Dictionary<string, SettingViewModel> settings = new Dictionary<string, SettingViewModel>();
        public List<SettingViewModel> SettingsList { get; set; }

        public string SeparateSlideBy
        {
            get { return settings["separateSlideBy"].SettingValue; }
        }
        public string SlideSeparatorSymbol
        {
            get { return settings["slideSeparatorSymbol"].SettingValue; }
        }
        public int LinesPerSlide
        {
            get { return int.Parse(settings["linesPerSlide"].SettingValue); }
        }
        public string Stanzas
        {
            get { return settings["stanzas"].SettingValue; }
        }

        // TODO test margin behavior
        public string Margin1
        {
            get
            {
                return MarginStart + ","
                    + MarginTop + ","
                    + "4,"
                    + MarginBottom;
            }
        }
        public string Margin2
        {
            get
            {
                return "4,"
                   + MarginTop + ","
                   + MarginEnd + ","
                   + MarginBottom;
            }
        }
        public int MarginStart
        {
            get { return int.Parse(settings["marginStart"].SettingValue); }
        }
        public int MarginTop
        {
            get { return int.Parse(settings["marginTop"].SettingValue); }
        }

        public int MarginEnd
        {
            get { return int.Parse(settings["marginEnd"].SettingValue); }
        }

        public int MarginBottom
        {
            get { return int.Parse(settings["marginBottom"].SettingValue); }
        }
       
        public string Orientation
        {
            get {
                return settings["orientation"].SettingValue;
            }

        }

        //Grid.Row="{Binding Row1}" Grid.Column="{Binding Col1}"
        //public int Row1 { get; set; } = 0;
        //public int Col1 { get; set; } = 0;
        public int Row2 
        {
            get
            {
                return (settings["orientation"].SettingValue == Model.Orientation.SideBySide.ToString()) ? 0 : 1;
            }
        }
        public int Col2
        {
            get
            {
                return (settings["orientation"].SettingValue == Model.Orientation.SideBySide.ToString()) ? 1 : 0;
            }
        }

        public int RowSpan
        {
            get
            {
                return (settings["orientation"].SettingValue == Model.Orientation.SideBySide.ToString()) ? 2 : 1;
            }
        }
        public int ColSpan
        {
            get
            {
                return (settings["orientation"].SettingValue == Model.Orientation.SideBySide.ToString()) ? 1 : 2;
            }
        }

        public int Text1FontSize 
        {
            get { return int.Parse(settings["text1FontSize"].SettingValue); }
        }
        public string Text1FontColor
        {
            get { return settings["text1FontColor"].SettingValue; }
        }
        public Brush PreviewText1FontColor => (Brush)new BrushConverter().ConvertFromString(Text1FontColor)!;

       public string Text1FontName
        {
            get { return settings["text1FontName"].SettingValue; }
        }

       public int Text2FontSize
        {
            get { return int.Parse(settings["text2FontSize"].SettingValue); }
        }
        public string Text2FontColor
        {
            get { return settings["text2FontColor"].SettingValue; }
        }
        public Brush PreviewText2FontColor => (Brush)new BrushConverter().ConvertFromString(Text2FontColor)!;

        public string Text2FontName
        {
            get { return settings["text2FontName"].SettingValue; }
        }

        public string Background
        {
            get { return settings["background"].SettingValue; }
        }
        public int BackgroundOpacity
        {
            get { return int.Parse(settings["backgroundOpacity"].SettingValue); }
        }
        public string Text1BackgroundColor
        {
            get { return settings["text1BackgroundColor"].SettingValue; }
        }
        public Brush PreviewText1BackgroundColor
        {
            get { return (Brush)new BrushConverter().ConvertFromString(Text1BackgroundColor)!; }
        }

        private SettingViewModel text2BackgroundColor =
            new SettingViewModel(new Setting("Text 2 background", "White", SettingType.ColorPicker));
        public string Text2BackgroundColor
        {
            get { return text2BackgroundColor.SettingValue; }
        }
        public Brush PreviewText2BackgroundColor
        {
            get { return (Brush)new BrushConverter().ConvertFromString("Green")!; }
        }

        public event EventHandler? SettingChanged;

        public SongSettingsViewModel(SongSettingsViewModel model)
        {
            foreach(var (k,v) in model.settings)
            {
                settings[k] = new SettingViewModel(v);
                settings[k].PropertyChanged += (s, e) => change();
            }

            SettingsList = new List<SettingViewModel>()
            {
                this.settings["separateSlideBy"],
                this.settings["slideSeparatorSymbol"],
                this.settings["linesPerSlide"],
                this.settings["stanzas"],
                this.settings["marginStart"],
                this.settings["marginTop"],
                this.settings["marginEnd"],
                this.settings["marginBottom"],
                this.settings["orientation"],
                this.settings["text1FontSize"],
                this.settings["text1FontColor"],
                this.settings["text1FontName"],
                this.settings["text2FontSize"],
                this.settings["text2FontColor"],
                this.settings["text2FontName"],
                this.settings["background"],
                this.settings["backgroundOpacity"],
                this.settings["text1BackgroundColor"],
                this.settings["text2BackgroundColor"]
            };
        }
        private void change()
        {
            Debug.Print("SongSettingsViewModel changing value");
            SettingChanged?.Invoke(this, EventArgs.Empty);
        }
        public SongSettingsViewModel(Dictionary<string,Setting> settings)
        {
            foreach(var (k,v) in settings)
            {
                this.settings[k] = new SettingViewModel(v);
                this.settings[k].PropertyChanged += (s, e) => SettingChanged?.Invoke(this, EventArgs.Empty);

                if(k.Contains("linesPerSlide"))
                {
                    this.settings[k].Validation = SlideLinesValidation;
                }
                if (k.Contains("margin"))
                {
                    this.settings[k].Validation = MarginValidation;
                }
                if (k.Contains("Opacity"))
                {
                    this.settings[k].Validation = OpacityValidation;
                }
                if (k.Contains("FontName"))
                {
                    this.settings[k].Options = new List<KeyValuePair<string, string>> () { new("Calibri", "Calibri") };
                }
                if (k.Contains("Color"))
                {
                    this.settings[k].Options = new List<KeyValuePair<string, string>>() { new("Black", "#000000") };
                }
            }
            
            SettingsList = new List<SettingViewModel>()
            { 
                this.settings["separateSlideBy"],
                this.settings["slideSeparatorSymbol"],
                this.settings["linesPerSlide"],
                this.settings["stanzas"],
                this.settings["marginStart"],
                this.settings["marginTop"],
                this.settings["marginEnd"],
                this.settings["marginBottom"],
                this.settings["orientation"],
                this.settings["text1FontSize"],
                this.settings["text1FontColor"],
                this.settings["text1FontName"],
                this.settings["text2FontSize"],
                this.settings["text2FontColor"],
                this.settings["text2FontName"],
                this.settings["background"],
                this.settings["backgroundOpacity"],
                this.settings["text1BackgroundColor"],
                this.settings["text2BackgroundColor"]
            };
        }

        private static string? SlideLinesValidation(string input)
        {
            if (int.TryParse(input, out int value))
            {
                if (value < 1)
                    return "Input is out of range";
                return null;
            }
            else
            {
                return "Please enter numbers only";
            }

        }

        private static string? MarginValidation(string input)
        {
            if(int.TryParse(input, out int value)){
                if (value < 0 || value > ApplicationSettingsViewModel.Instance.Width || value > ApplicationSettingsViewModel.Instance.Height)
                    return "Input is out of range";
                return null;
            } else
            {
                return "Please enter numbers only";
            }
            
        }

        private static string? OpacityValidation(string input)
        {
            if (int.TryParse(input, out int value))
            {
                if (value < 0 || value > 100)
                    return "Input is out of range";
                return null;
            }
            else
            {
                return "Please enter numbers only";
            }
        }

        public Dictionary<string, Setting> Save()
        {
            Dictionary<string, Setting> t = new Dictionary<string, Setting>();
            foreach (var (k, v) in settings)
            {
                t[k] = v.Setting;
            }
            return t;
        }
    }
}